# Research: Text Quiz Generator

## Decision 1: Use Angular, Not `rangular`

**Decision**: Build the frontend with the standard Angular framework and its
current stable release compatible with the selected TypeScript version.

**Rationale**: The project constitution requires Angular. The published
`rangular` package is an unrelated, legacy AngularJS package and does not meet
that requirement.

**Alternatives considered**:

- `rangular`: Rejected because it is not the Angular framework required by the
  project and would add an obsolete dependency.
- A different frontend framework: Rejected because it violates the constitution.

## Decision 2: Use ASP.NET Core on .NET 10 LTS

**Decision**: Implement the backend as an ASP.NET Core Web API on .NET 10 LTS.

**Rationale**: This fulfills the required .NET Core service boundary and offers
typed contracts, dependency injection, managed outbound HTTP clients, and
first-class test support.

**Alternatives considered**:

- A Node.js backend: Rejected because the constitution requires .NET Core.
- Older .NET versions: Rejected because a current LTS version reduces support
  risk for a new project.

## Decision 3: Configure Gemma 4 26B A4B Behind a Provider Adapter

**Decision**: Treat `gemma-4-26b-a4b` as the requested Gemma 4 26B A4B model
identifier. The backend exposes an `IQuizGenerationProvider` boundary and loads
the endpoint, credential, and model identifier from server-side configuration.
Startup must verify that the configured provider can access the configured model
and return structured JSON before accepting generation requests.

**Rationale**: Google documents Gemma 4 26B A4B as an open-weight model suited
to high-throughput reasoning, while Google AI Studio keys are documented for the
Gemini API. The public documentation does not establish that a Google AI Studio
key alone calls a hosted Gemma 4 26B A4B endpoint. Configuration plus an
availability probe preserves the requested model without assuming an unsupported
endpoint. If the Google AI Studio account exposes that model, the provider may
use `GEMINI_API_KEY`; otherwise the Gemma deployment must supply a compatible
server-side credential and endpoint.

**Alternatives considered**:

- Hardcode a Gemini model: Rejected because it changes the requested model.
- Send a Google AI Studio key to the browser: Rejected because it exposes a
  billable credential and violates the constitution.
- Hardcode an undocumented Gemma endpoint: Rejected because deployment would
  fail silently or produce an unusable integration.

## Decision 4: Require Structured Output and Server Validation

**Decision**: Ask the generation provider for JSON matching an internal quiz
schema, then validate it before creating a quiz. Validation requires exactly 10
questions, four A-D options per question, one correct option, non-empty
explanations for all options, unique identifiers, and matching source evidence.

**Rationale**: Structured output makes the client contract predictable. Model
output can still be structurally or semantically incomplete, so the API must
validate all business invariants and retry once with corrective instructions.
It returns an actionable generation error if the second result remains invalid.

**Alternatives considered**:

- Parse free-form text in the browser: Rejected because parsing and answer-key
  validation would be fragile and expose correct answers.
- Trust valid JSON without business validation: Rejected because valid JSON can
  still contain 9 questions, duplicate options, or unsupported statements.

## Decision 5: Make Source Evidence Part of Every Generated Question

**Decision**: The internal model output requires one or more verbatim evidence
excerpts for every question. The API verifies that each excerpt occurs in the
submitted source text before accepting the quiz. Evidence is returned with
results to support review.

**Rationale**: Exact excerpt matching is an enforceable baseline for the
source-grounding requirement. It does not prove every inference automatically,
so acceptance testing also reviews a representative source-grounding set.

**Alternatives considered**:

- Use external web search to fill gaps: Rejected because the feature is limited
  to the supplied source material.
- Provide explanations without evidence: Rejected because reviewers could not
  trace feedback to the user's text.

## Decision 6: Use Short-Lived Server-Side Quiz State for the MVP

**Decision**: Store each generated quiz, source text, answer key, explanations,
evidence, and provider metadata in `IMemoryCache` for 60 minutes. The browser
holds only its active quiz ID and selected answers. Expired quizzes return a
clear expiration error.

**Rationale**: The MVP explicitly excludes accounts and long-term history, so a
database is unnecessary. Keeping answer keys server-side prevents score
tampering and allows regeneration from the original source.

**Alternatives considered**:

- Send answers and explanations with the generated quiz: Rejected because it
  reveals answers before the learner submits.
- Persist quizzes in a database: Deferred until history, accounts, analytics,
  or multi-instance deployment is required.

## Decision 7: Handle Provider Failures Explicitly

**Decision**: Configure a request timeout below the 5-second user-visible goal,
use at most one retry for malformed provider output or retryable provider
failure, and return a structured error while retaining source input on the
client. Log only a trace ID, timing, provider outcome, and validation category.

**Rationale**: The feature requires completion within a few seconds and
preservation of user input after a failure. Limiting retries prevents long waits
and duplicate costs.

**Alternatives considered**:

- Retry indefinitely: Rejected because it breaks the latency target.
- Return partial quizzes: Rejected because the 10-question contract is fixed.

## Decision 8: Test at the Boundary Where Each Risk Lives

**Decision**: Use Angular unit tests for form, answer, and result states;
ASP.NET Core unit tests for validation and scoring; integration tests with a
fake provider; OpenAPI contract tests; and Playwright tests for the full user
flow.

**Rationale**: This aligns verification with the constitution's requirements for
input validation, question count, difficulty, explanation completeness, shared
contracts, and user-facing workflows.

**Alternatives considered**:

- Test only the UI: Rejected because server-side scoring and model-output
  validation would remain unverified.
- Call the live model in every automated test: Rejected because tests would be
  slow, costly, and nondeterministic.

## Resolved Configuration

| Area | Decision |
| --- | --- |
| Frontend | Angular |
| Backend | ASP.NET Core on .NET 10 LTS |
| Requested model | Gemma 4 26B A4B, configured as `gemma-4-26b-a4b` |
| AI access | Server-side provider adapter with endpoint and credential configuration |
| Google AI Studio credential | `GEMINI_API_KEY` only when the configured provider accepts it; never client-side |
| Session state | In-memory server cache for 60 minutes |
| Public interface | Versioned REST API defined in `contracts/quiz-api.openapi.yaml` |
