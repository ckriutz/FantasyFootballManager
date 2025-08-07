# Yahoo Fantasy Football Integration Plan

## FYI for Resources:
https://developer.yahoo.com/fantasysports/guide/#game-resource

## Overview
Add first-phase Yahoo Fantasy Football draft integration: a user supplies a Yahoo League ID, authorizes via OAuth2 (3‑legged), selects their Yahoo team, and the system imports draft results (or roster fallback) to mark drafted players (`isDraftedOnMyTeam`, `isDraftedOnOtherTeam`) to assist draft strategy. Player mapping prioritizes `SleeperPlayer.YahooId` with fuzzy fallback. Architecture is future-ready for multi-league and multi-platform.

## Assumptions / Answers to Open Questions
- Single Yahoo league per user (MVP).
- Drafted flags are global; clearing/reapplying acceptable.
- Fuzzy matching allowed when `YahooId` absent.
- No special compliance constraints now (still encrypt refresh tokens).
- Goal: draft assistance (identify available vs drafted players).
- Future: multi-league, other platforms (Sleeper native, ESPN, etc.).

## Requirements
1. OAuth2 (3‑legged) per user; must store refresh token (app keys alone insufficient).
2. User workflow: Enter League ID → Connect Yahoo → Select Team → Sync Draft.
3. Data fetched: league metadata, teams, draft results (primary) or rosters (fallback).
4. Player mapping order: YahooId → exact (name+pos+team) → fuzzy → unmatched log.
5. Persistence: tokens, league/team identifiers, last sync timestamp, optional mapping & sync logs.
6. Flags: Clear and reapply drafted flags on each sync.
7. Security: encrypted refresh tokens, OAuth state validation, no token exposure to frontend.
8. Error states: Not Connected / Connected / Team Not Selected / Sync In Progress / Sync Error / Pre-Draft.
9. Performance: batch Yahoo API calls; avoid redundant fetch of static draft results.
10. Observability: sync logs, unmatched counts, token refresh logging.
11. Extensibility: abstraction layer for future platforms & multi-league support.
12. Testing: unit (mapping, key parsing, token refresh), integration (OAuth, sync), edge (partial draft, revoked token).

## Data Model Changes (API)
Extend `User`:
- `YahooLeagueId` (string?)
- `YahooLeagueKey` (string?)
- `YahooTeamKey` (string?)
- `YahooConnectedAt` (DateTime?)
- `YahooLastSyncAt` (DateTime?)

New tables:
- `YahooOAuthToken` (UserId FK, EncryptedRefreshToken, AccessToken, ExpiresAt, Scopes, UpdatedAt).
- `YahooLeague` (LeagueKey PK, LeagueId, GameKey, Name, Season, UserId FK).
- `YahooTeam` (Id PK, LeagueKey FK, TeamKey, Name, RawJson).
- `YahooPlayerMap` (YahooPlayerKey PK, InternalPlayerId nullable, MatchType enum, Confidence decimal, NameSnapshot, Position, Team, UpdatedAt).
- `YahooSyncLog` (Id PK, UserId, StartedAt, FinishedAt, Status enum, Matched, Fuzzy, Unmatched, ErrorMessage, DurationMs).

(Option: Defer `YahooLeague`, `YahooTeam`, `YahooPlayerMap` to later; recommended to add now for stability.)

Enums:
- `MatchType`: YahooId, Exact, Fuzzy, Unmatched.
- `SyncStatus`: Success, Partial, Failed.

## Yahoo API Endpoints (JSON format via `?format=json`)
- Game meta: `/fantasy/v2/game/nfl`
- League: `/fantasy/v2/league/{league_key}`
- Teams: `/fantasy/v2/league/{league_key}/teams`
- Draft results: `/fantasy/v2/league/{league_key}/draftresults`
- Team roster (fallback): `/fantasy/v2/team/{team_key}/roster`
- Player (if needed): `/fantasy/v2/player/{player_key}` (batch by comma)

Key forms:
- `league_key = {game_key}.l.{league_id}`
- `team_key = {league_key}.t.{team_id}`
- `player_key = {game_key}.p.{player_id}` (tail numeric likely == Sleeper `YahooId`—verify)

## OAuth Flow
1. Frontend requests `GET /yahoo/auth/url`.
2. Backend builds Yahoo authorize URL with state (persisted or signed).
3. User consents → Yahoo redirects to `/yahoo/oauth/callback?code&state`.
4. Backend validates state, exchanges code for access + refresh token.
5. Store tokens (encrypt refresh), set `YahooConnectedAt`.
6. Redirect frontend (e.g., `/profile?yahoo=connected`).

Token handling:
- Refresh if `ExpiresAt - now <= 60s`.
- On 401: attempt one refresh then retry; failure → mark disconnected.

## API Endpoints (Backend)
- `GET /yahoo/auth/url` → { url }
- `GET /yahoo/oauth/callback` → redirects
- `GET /yahoo/status`
- `POST /yahoo/league` { leagueId }
  - Fetch game_key → compute league_key → fetch/store league + teams
- `GET /yahoo/league` → league + teams
- `POST /yahoo/team/select` { teamKey } → store & trigger sync
- `POST /yahoo/sync` (manual) → triggers sync
- `GET /yahoo/unmatched` (optional) → list (paged) unmatched mappings

## Sync Workflow
1. Validate tokens (refresh if needed).
2. Ensure league/team present (otherwise exit with status).
3. Fetch draft results:
   - If empty (pre-draft) → record status, stop.
4. Parse picks: (player_key, team_key).
5. Extract numeric player_id from each player_key.
6. Player mapping:
   - If numeric == any `SleeperPlayer.YahooId` → MatchType=YahooId.
   - Else exact normalized name+pos(+team).
   - Else fuzzy similarity (≥ threshold 0.90) → Fuzzy.
   - Else Unmatched.
   - Upsert `YahooPlayerMap`.
7. Partition mapped player IDs into my vs others (team_key comparison).
8. Transaction:
   - Clear `isDraftedOnMyTeam` & `isDraftedOnOtherTeam` globally (or only affected players).
   - Apply new flags.
   - Update `YahooLastSyncAt`.
9. Record `YahooSyncLog`.
10. Return summary (matched, fuzzy, unmatched counts).

Fallback if no draft:
- Iterate team rosters; treat all rostered players similarly (skips pick order).

## Player Name Normalization (for fallback)
- Uppercase.
- Strip punctuation (`.,'` etc.).
- Remove suffixes (` JR`, ` SR`, ` III`, ` II`).
- Collapse whitespace.
- Map common variations (e.g., “DJ” vs “D.J.”).
Fuzzy algorithm: Levenshtein or Dice; accept best candidate with score ≥ 0.90 and gap > 0.05 from next candidate.

## Frontend Changes
Profile Page:
- Inputs: League ID, buttons: Connect / Reconnect, Select Team (dropdown after league fetch), Sync Now.
- Status badge: Connected / Team Selected / Last Sync / Unmatched count.
- Unmatched modal (optional phase 2).

Player / List Pages:
- Reuse existing flags for drafted indicators (no extra column changes needed).

## Security
- Encrypt refresh tokens (AES-256 with env-provided key).
- HMAC or stored state token for OAuth.
- Avoid logging tokens or full raw Yahoo responses (truncate).
- Rate limit handling (429) with backoff (optional phase 2).

## Observability
Log fields:
- Sync start/end, userId, league_key, counts, duration, error.
- Token refresh events (info).
Metrics (future): unmatched ratio gauge, sync duration histogram.

## Edge Cases
- Pre-draft: return status “PRE_DRAFT” (no flags applied).
- Partial draft: incremental flags update; next sync completes.
- Revoked consent: refresh fails → clear tokens, status becomes Not Connected.
- Player dataset lag: some Yahoo players missing in Sleeper → unmatched list.
- DST / Team defenses naming divergence (special-case normalization).

## Incremental Delivery
Milestone 1 (MVP):
- DB migration (User + Token + League + Team).
- OAuth flow + league/team selection.
- Draft sync using YahooId only.
- Basic profile UI (connect, league ID, team select, sync).
- Logs & minimal tests.

Milestone 2:
- Fuzzy mapping + `YahooPlayerMap` + unmatched endpoint/UI.
- Sync log table + status endpoint enhancements.
- Roster fallback & partial draft handling.

Milestone 3:
- Background periodic sync worker.
- Metrics and richer observability.
- Platform abstraction layer.

## Verification of YahooId
Procedure:
1. Capture a `player_key` from draftresults (e.g., `423.p.12345`).
2. Extract tail id `12345`.
3. Query SleeperPlayers where `YahooId == 12345`.
4. If mismatch rate low → proceed; else require player detail endpoint to pull official id field.

## Testing Plan
Unit:
- Player key parsing → numeric id.
- Name normalization cases.
- Fuzzy similarity thresholds.
- Token refresh trigger timing.
- Mapping precedence (YahooId > Exact > Fuzzy).

Integration (with mocked Yahoo client):
- OAuth callback persists tokens.
- League key derivation from leagueId + game meta.
- Team selection triggers sync job.
- Draft sync idempotent (re-run unchanged).
- Mixed mapping (YahooId + Fuzzy + Unmatched).

Edge:
- Pre-draft (no draftresults).
- Revoked token (401 + refresh failure).
- Rate limit (simulate 429 → handled gracefully).
- Transaction rollback on mapping error.

Performance:
- Simulated 300+ picks → sync under target (e.g., <3s).
- Batched player detail fetch (if needed) reduces calls.

Security:
- Invalid state rejected.
- No tokens in API responses or logs.

Manual:
- End-to-end flow with real test league.
- Visual confirmation of drafted markers.

## Commands (Dev Setup)
```bash
export YAHOO_CLIENT_ID=...
export YAHOO_CLIENT_SECRET=...
export YAHOO_REDIRECT_URI=https://localhost:5001/yahoo/oauth/callback
dotnet ef migrations add AddYahooIntegration
dotnet ef database update