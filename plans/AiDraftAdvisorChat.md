ckriutz: My application is a app that helps the user with their fantasy football team. There is a DataService that gets a list of players from different sources to use as data. What I would like to do, is on the front page there is a box Coming Soon with nothing in it. What I want to do, is have a button for "AI Analysis" where We send the list of existing players, along with top available players, along with a strategy prompt to an Azure AI Foundry endpoint, where it will return an handful of suggestions for the user, highlighting who they should likely draft next. That would be the initial plan. Can you help me plan this out?

GitHub Copilot: I'll draft a detailed implementation plan to add the AI Analysis feature that sends roster + available players + strategy prompt to Azure AI and returns draft suggestions.

## Overview
Add an “AI Analysis” feature to the Home page that lets an authenticated user request draft recommendations based on their current roster and top available players. The backend will expose a secure endpoint that gathers data, constructs a constrained prompt, calls an Azure AI Foundry (Azure OpenAI) model, validates the structured response, and returns actionable suggestions. The UI will provide a button, strategy input, loading state, results list, and retry capability.

## Requirements
1. Front-End UI: Replace “Coming Soon” card with “AI Draft Advisor” card containing:
   - Strategy selector/input (preset + custom)
   - Button: “Run AI Analysis”
   - Loading state + error handling
   - Results list: recommended players with reason, priority score, risk, fit
   - Ability to re-run with modified strategy
2. Data Sent to AI: 
   - User’s drafted players (already retrieved)
   - Top available players (need backend support)
   - Strategy prompt (default + user override)
3. Backend API: POST /ai/draft-suggestions (auth required)
   - Request: { userId, strategy (optional) }
   - Response: { generatedAt, strategyUsed, recommendations: [ { playerId, sleeperId?, name, position, team, byeWeek, rankEcr, projPoints, reason, priorityScore, riskLevel, fitTags } ], summary }
4. Azure AI Integration:
   - Server-side only; front-end never sees key
   - Environment-driven config: endpoint, key, deployment/model name, API version, temperature, max tokens
5. Prompt Engineering:
   - System + user prompt with JSON schema instructions
   - Token control (limit # available players, strip extraneous fields)
6. Validation & Error Handling:
   - Parse & validate JSON (fallback: attempt JSON repair)
   - Handle Azure timeouts, rate limits, malformed output
   - Return 502/424 style errors gracefully to UI
7. Logging & Observability:
   - Persist each request/response (sanitized) in new table DraftAnalysisLog (optional first pass: in-memory or file) with userId, requestHash, duration, success flag
8. Security:
   - Endpoint requires authenticated identity mapped to userId
   - Rate limit per user (simple in-memory counter initial)
9. Performance:
   - Limit available players to top N (e.g., 40 overall + highest positional needs)
   - Timeout ≤ 15s end-to-end
10. Extensibility:
    - Support future features: trade suggestions, positional need weighting, dynamic roster scoring
11. Testing:
    - Unit tests for prompt builder, parser, service orchestration
    - Integration test with mocked Azure client
    - Front-end component tests (loading, success, error)
12. Documentation:
    - README section + .env.example additions
13. Failure UI States:
    - Show retry button, human-readable message, optional raw debug toggle (dev only)

## Implementation Steps

### 1. Backend Data Foundations
- Confirm existing data model: how drafted players are stored (currently served by /players/drafted/{userId}).
- Add repository/service method: GetTopAvailablePlayers(int limitOverall, perPositionLimit) excluding user’s drafted players.
  - Ranking logic: use rankEcr ascending; fallback by projPoints.
  - Derive positional needs: count roster by position vs typical fantasy roster template (QB1, RB2, WR3, TE1, FLEX2, etc.) to inform prompt.

### 2. New DTOs
- Request DTO: AiDraftSuggestionsRequest { string UserId; string? Strategy; int? AvailableLimitOverride }
- Internal model: PlayerSnapshot (subset fields)
- Response DTO: AiDraftSuggestionsResponse { DateTime generatedAt; string strategyUsed; string model; string summary; List<DraftRecommendation> recommendations }
- DraftRecommendation { string playerId; string sleeperId; string name; string position; string team; int? byeWeek; int? rankEcr; double? projPoints; double priorityScore; string riskLevel; string reason; string[] fitTags }

### 3. Table (Optional MVP or Phase 2)
- DraftAnalysisLog: Id (guid), UserId, RequestedAt, DurationMs, Strategy, RosterCount, AvailableCount, Model, Success, ErrorType, RawPrompt (truncated), RawResponse (truncated), ParsedRecommendationCount.

### 4. Azure AI Client Abstraction
- Interface IAzureAiClient with method Task<string> CompleteJsonAsync(AiPromptRequest).
- Implementation uses HttpClient:
  - POST {endpoint}/openai/deployments/{deployment}/chat/completions?api-version=...
  - Body: { model/deployment, messages[], temperature, max_tokens, response_format (if supported for JSON) }
- Add resilience: timeout, retry (exponential backoff for 429/5xx up to 2 retries).
- Configuration class AzureAiOptions bound from config.

### 5. Prompt Builder
- Inputs: roster (PlayerSnapshot[]), availablePlayers (PlayerSnapshot[]), strategy (default if null).
- Strategy defaults: "Balanced upside with manageable risk; prioritize positional scarcity and bye week balance."
- Produce:
  - System message: role=system with instructions, JSON schema spec, deterministic output, no extra prose.
  - User message: includes roster summary (counts + table), available players table (limited), positional needs estimate, strategy instructions.
- Include explicit JSON schema example:
  {
    "summary": "...",
    "recommendations": [
      {
        "playerId": "...",
        "name": "...",
        "position": "...",
        "team": "...",
        "priorityScore": 0-100,
        "riskLevel": "Low|Medium|High",
        "fitTags": ["Upside","PositionalNeed"],
        "reason": "..."
      }
    ]
  }
- Provide guardrails: If insufficient data, return empty recommendations with explanation.

### 6. Response Parsing & Validation
- Parse JSON strictly (System.Text.Json).
- If parse fails: attempt regex extraction of JSON substring, reparse.
- Validate required fields; drop malformed entries; if none valid, return error status to client.
- Normalize scores (clamp 0–100); dedupe players; enforce max recommendations (e.g., 5).

### 7. Service Orchestration
- AiDraftAdvisorService:
  - Fetch roster (existing).
  - Fetch available players list with ranking logic & positional weighting.
  - Build prompt.
  - Call Azure AI client.
  - Parse & validate.
  - Persist log entry.
  - Return DTO.

### 8. API Controller
- Route: POST /ai/draft-suggestions
- Auth: Use existing auth middleware; map to userId.
- Body optional strategy; if userId in body must match auth principal (else 403).
- Returns 200 with response DTO; 429 if rate-limited; 502 upstream errors; 422 malformed model response.

### 9. Configuration & Secrets
- appsettings.json keys:
  "AzureAi": {
     "Endpoint": "",
     "ApiKey": "",
     "Deployment": "",
     "ApiVersion": "2024-05-01-preview",
     "Temperature": 0.2,
     "MaxOutputTokens": 800,
     "MaxAvailablePlayers": 40
  }
- Add to Dockerfile environment injection; document in README.
- Front-end: add REACT_APP_API_URL if not already; no AI keys.

### 10. Rate Limiting
- Simple in-memory SlidingWindow per user (e.g., max 5 analyses / hour).
- Return clear UI message when exceeded.
- Future: Redis distributed limiter.

### 11. Front-End Changes
- Replace "Coming Soon" card with:
  - Title: AI Draft Advisor
  - Strategy select (dropdown): Balanced (default), Zero RB, Hero RB, WR Heavy, Upside Focus, Custom
  - If Custom selected: multiline textarea
  - Button: Run AI Analysis
  - While loading: spinner + “Analyzing roster and board...”
  - On success: list of Recommendation cards:
    - Rank order number
    - Player (position • team)
    - Priority score bar (visual)
    - Risk badge color-coded
    - Fit tags (chips)
    - Reason (collapsible after 2 lines)
  - Summary panel (short overall guidance)
  - Retry / Adjust Strategy button
  - Error state with retry.
- API call:
  POST `${apiUrl}/ai/draft-suggestions`
  Body: { strategy }
  Headers: Authorization Bearer (existing token if available).
- Add local component state: analysis, loading, error, selectedStrategy, customStrategy.
- Defensive: if players not yet loaded, disable button.

### 12. Token Management (Auth0)
- Ensure existing auth hook exposes access token; if not, add getAccessTokenSilently to fetch token and set Authorization header.

### 13. Accessibility & UX
- Announce loading via aria-live region.
- Keyboard accessible strategy selection.
- Color contrast for risk badges.

### 14. Logging & Metrics (Phase 1 Light)
- Console log server timings.
- Measure prompt token size (approx) and truncate if beyond threshold.

### 15. Future Enhancements (Document Only)
- Position scarcity weighting slider.
- Multi-model ensemble comparison.
- Draft simulation for next N picks.
- Cache last analysis to avoid repeat cost.

## Testing

### Backend Unit Tests
1. PromptBuilderTests:
   - Given roster + available + strategy -> contains required sections.
   - Truncates when exceeding max players.
2. ResponseParserTests:
   - Valid JSON parses into DTO.
   - Malformed JSON with extra prose still extracts.
   - Invalid entries (missing playerId) filtered.
3. PositionalNeedsCalculatorTests:
   - Correctly identifies shortage vs surplus.
4. RateLimiterTests:
   - Exceeding calls returns limited status.

### Backend Integration Tests
1. AiDraftSuggestionsEndpoint_Success (mock Azure client returns fixed JSON).
2. AiDraftSuggestionsEndpoint_ModelMalformed (mock returns junk -> 422).
3. AiDraftSuggestionsEndpoint_RateLimited.
4. AiDraftSuggestionsEndpoint_AuthMismatch (userId spoof attempt -> 403).
5. AvailablePlayersSelection_LimitsRespected (ensures cap).

### Front-End Tests (React)
1. Renders AI Advisor card when authenticated.
2. Strategy dropdown changes state; custom strategy field appears.
3. Button disabled while loading; spinner shown.
4. Successful response renders recommendations list.
5. Error path renders retry button.
6. Re-run updates results (mock different response).

### Contract / Schema Tests
- JSON schema validation for DraftRecommendation using a lightweight validator (optional).
- Snapshot test for typical model output mapping.

### Performance / Resilience
- Simulated large roster & available list ensures truncation.
- Timeout in Azure client returns clean error.

### Manual Smoke
- Run local with mock Azure (feature flag AZURE_AI_FAKE=true).
- Real call with minimal roster.

## Completion Criteria
- New endpoint operational with mocked & real Azure AI.
- Front-end UI replaces placeholder with fully functional analysis feature.
- Errors & rate limits handled gracefully.
- Documentation updated with config instructions.
- Tests passing (unit + key integration + front-end component).
- No secrets exposed client-side.

Let me know if you’d like a trimmed MVP variant or want me to proceed to draft specific code stubs next.

ckriutz: This is perfect, can you write out this plan to the plans folder?

GitHub Copilot: Adding the implementation plan as a Markdown document you can save under plans/AiDraftAdvisorPlan.md.

```markdown
# AI Draft Advisor Implementation Plan

## Overview
Introduce an “AI Draft Advisor” feature that analyzes a user’s current fantasy roster plus top available players and returns draft recommendations using an Azure AI Foundry (Azure OpenAI) model. The feature replaces the placeholder “Coming Soon” card on the authenticated Home page. It includes a strategy selector, invokes a new secure backend endpoint, and returns structured recommendations (players + rationale + priority).

## Requirements
1. Front-End UI
   - Replace placeholder card with AI Draft Advisor.
   - Strategy selector (presets + Custom).
   - Run Analysis button (disabled while loading or no roster).
   - Loading, error, empty, and success states.
   - Display recommendations (ordered) with: player, position, team, priority score, risk level, fit tags, reason (collapsible).
   - Summary section + rerun option.
2. Backend Endpoint: POST /ai/draft-suggestions
   - Auth required (user identity from token).
   - Request: { strategy? } (userId inferred).
   - Response: { generatedAt, strategyUsed, model, summary, recommendations[] }.
3. Data Inputs to Model
   - User roster (drafted players).
   - Top available players (capped, filtered).
   - Derived positional needs.
   - Strategy instructions (default or user-supplied).
4. Azure AI Integration
   - Server-side only; configurable via appsettings/env.
   - Supports deterministic(ish) output; enforces JSON schema.
5. Prompt Engineering
   - System + user messages.
   - Roster summary (counts).
   - Available player list (rank-sorted, truncated).
   - JSON schema + strict formatting instructions.
6. Output Structure
   - summary (string)
   - recommendations: max 5–7 entries each with:
     - playerId / sleeperId
     - name, position, team
     - rankEcr?, projPoints?, byeWeek?
     - priorityScore (0–100)
     - riskLevel (Low|Medium|High)
     - fitTags[]
     - reason (concise)
7. Validation & Error Handling
   - Strict JSON parse; fallback extraction & repair attempt.
   - Filter malformed recommendations.
   - Return 422 if irreparable; 502 for upstream failure.
8. Performance
   - Limit available players (e.g., 40 overall + positional supplements).
   - End-to-end timeout target ≤15s; model call ≤8–10s.
9. Security
   - AuthZ: user cannot spoof another userId.
   - Rate limit per user (e.g., 5/hour initial, in-memory).
10. Observability
   - Log duration, success/failure, truncated prompt/response.
   - Optional persistence (DraftAnalysisLog table future phase).
11. Configuration
   - AzureAi: Endpoint, ApiKey, Deployment, ApiVersion, Temperature, MaxOutputTokens, MaxAvailablePlayers.
12. Extensibility
   - Future: trade advice, draft simulation, dynamic weighting, caching previous runs.
13. Documentation
   - README additions + .env.example updates.
14. Accessibility
   - Proper ARIA live region for loading and error.
15. Testing Coverage
   - Unit (prompt builder, parser, limiter).
   - Integration (endpoint success/failure).
   - Front-end component tests.

## Implementation Steps

### 1. Backend Data Layer
- Add method to fetch top available players excluding user roster:
  - Inputs: limitOverall, perPosLimit.
  - Sorting: rankEcr ascending fallback projPoints.
- Implement positional needs calculator:
  - Compare current counts to template roster (QB1, RB2, WR3, TE1, FLEX2, DEF1, K1 optional).
  - Produce need tags (e.g., “HighNeed: RB”, “Depth: WR”).

### 2. DTOs
- AiDraftSuggestionsRequest { string? Strategy }
- PlayerSnapshot (subset fields used in prompt)
- DraftRecommendation { playerId, sleeperId, name, position, team, byeWeek?, rankEcr?, projPoints?, priorityScore, riskLevel, reason, fitTags[] }
- AiDraftSuggestionsResponse { DateTime generatedAt; string strategyUsed; string model; string summary; List<DraftRecommendation> recommendations }

### 3. Azure AI Client
- IAzureAiClient.CompleteJsonAsync(AiPromptRequest)
- Implementation:
  - HttpClient with base endpoint + deployment.
  - POST chat/completions (api-version from config).
  - Messages: system + user.
  - temperature from config (e.g., 0.2).
  - max_tokens (e.g., 800).
  - Optional response_format: json_object (if supported).
- Resilience: cancellation token, timeout, retry (429/5xx up to 2 attempts with jitter).

### 4. Prompt Builder
- Input: roster snapshots, available players (trim), positional needs, strategy (default if null).
- System message: role guidance, JSON schema instructions, no extra prose, forbid disclaimers.
- User message:
  - Strategy text.
  - Roster summary table (Position | Count | Key Names).
  - Available players table (Rank | Pos | Name | Team | Proj | Bye).
  - Positional needs list.
  - Output schema + rules (priorityScore 0–100, riskLevel enumeration).
- Provide minimal example JSON (one recommendation).
- Instruct to avoid players already on roster.

### 5. Service Orchestration (AiDraftAdvisorService)
- Fetch roster and transform.
- Fetch available players; apply caps.
- Derive needs.
- Build prompt.
- Call AI client.
- Parse & validate response.
- Log metrics.
- Return response DTO.

### 6. Response Parsing
- Attempt direct JSON parse.
- If fail: locate first balanced { ... } block via brace scanning; retry.
- Validate each recommendation:
  - Required: playerId OR sleeperId, name, position, priorityScore, reason.
  - Clamp priorityScore, dedupe players.
  - Limit recommendations (≤7).
- If zero valid → error (422).

### 7. Rate Limiting
- In-memory store: Dictionary<userId, SlidingWindowState>.
- Enforce count/time window; emit remaining quota in response header (X-RateLimit-Remaining).
- On exceed → 429 with JSON error.

### 8. API Controller
- POST /ai/draft-suggestions
  - Auth principal → userId.
  - Body strategy optional.
  - Calls service; maps result to response.
  - Error mapping:
    - Rate limit → 429
    - Model malformed → 422
    - Timeout / upstream → 502
    - Unexpected → 500 generic (no sensitive details)

### 9. Config & Secrets
- appsettings.json / appsettings.Development.json stub keys.
- Environment variable overrides (Docker).
- README: add required vars; sample .env snippet.

### 10. Front-End Integration
- Replace second card in Home.js.
- Component state:
  - strategyPreset, customStrategy
  - loading, error, result, lastRanAt
- Preset strategies list:
  - Balanced, Zero RB, Hero RB, WR Heavy, Upside Focus, Safe Floor, Custom
- Derived strategy text = preset mapping unless Custom.
- API call:
  - Acquire token (getAccessTokenSilently) if needed.
  - POST JSON { strategy }.
- UI states:
  - Idle: form + button.
  - Loading: spinner & “Analyzing roster and board...”
  - Success: summary + recommendation cards.
  - Error: message + retry button.
- Recommendation card:
  - Index number
  - Player position • name • team
  - Priority bar (width = priorityScore%)
  - Risk badge color-coded (Low green, Medium amber, High red)
  - Fit tags as rounded chips
  - Reason collapsible (line clamp)
- Accessibility: aria-busy, role="status" for loading.

### 11. Styling & Utilities
- Tailwind classes consistent with existing style.
- Reusable PriorityBar component (optional).
- Risk level color mapping function.

### 12. Logging / Dev Aids
- Console log request latency (Dev only).
- Optional feature flag to bypass real AI call (e.g., ?mockAi=true or env override) returning canned JSON.

### 13. Documentation
- Update README: feature description, enabling instructions, environment setup, rate limits.
- plans/AiDraftAdvisorPlan.md (this file).

### 14. Future Enhancements (Not in MVP)
- Cache last analysis per user (TTL 2m) to prevent rapid repeats.
- Advanced weighting (user chooses positional emphasis).
- Draft simulation (predict next N picks).
- Persist logs table + analytics dashboard.
- Multi-model fallback.

## Testing

### Backend Unit Tests
1. PositionalNeedsCalculatorTests
   - Empty roster → all needs flagged.
   - Fully balanced roster → minimal high-need flags.
2. PromptBuilderTests
   - Includes schema section & strategy text.
   - Truncates available players to configured count.
3. ResponseParserTests
   - Valid JSON → success.
   - JSON with leading prose → extraction success.
   - Invalid risk level → defaults or filtered.
4. RateLimiterTests
   - N rapid calls exceed limit.
5. ScoreClampTests
   - Out-of-range priorityScore clamps to [0,100].

### Backend Integration Tests (Mock AI Client)
1. Success path returns expected DTO shape.
2. Malformed model output → 422.
3. Rate limit exceeded → 429.
4. Timeout from AI client → 502.
5. Auth missing → 401.

### Front-End Tests (Jest/React Testing Library)
1. Renders AI card when authenticated.
2. Preset selection toggles custom textarea visibility.
3. Button disabled when loading or no players yet.
4. Displays spinner & status during loading.
5. Renders recommendations list & summary after success.
6. Error state shows retry.
7. Re-run replaces previous results.

### Contract Tests
- Snapshot of minimal valid AI JSON.
- Schema validation (optional lightweight).

### Performance Tests (Light)
- Large available list truncated.
- Latency measurement under mock (ensures parsing logic efficient).

### Manual QA
1. Run with mock mode (env AZURE_AI_FAKE=true) to validate UI flow.
2. Switch to real model; verify structured JSON (inspect network).
3. Strategy differences produce distinct outputs.

## Default Strategy Text
“Balanced upside with manageable risk; prioritize positional scarcity, roster balance, and bye week diversification. Avoid duplicating covered positions unless value is exceptional.”

## JSON Output Schema (Model Instruction)
{
  "summary": "string - high-level drafting guidance",
  "recommendations": [
    {
      "playerId": "string",
      "sleeperId": "string",
      "name": "string",
      "position": "QB|RB|WR|TE|K|DST|FLEX",
      "team": "string",
      "byeWeek": number?,
      "rankEcr": number?,
      "projPoints": number?,
      "priorityScore": number (0-100),
      "riskLevel": "Low"|"Medium"|"High",
      "fitTags": ["string", ...],
      "reason": "string concise justification"
    }
  ]
}

## Error JSON Format (API)
{ "error": "string-code", "message": "Human-readable explanation" }

## Completion Criteria
- Endpoint deployed & callable.
- Front-end card fully functional.
- Proper error and loading states.
- Tests passing (core units + integration).
- Docs updated; environment variables listed.
- No secrets exposed client-side.

---
```

Let me know if you want this trimmed or expanded further; save it as `plans/AiDraftAdvisorPlan.md`.