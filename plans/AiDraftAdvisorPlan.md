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