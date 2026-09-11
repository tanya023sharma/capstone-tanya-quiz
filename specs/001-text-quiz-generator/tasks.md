---
description: "Task list template for feature implementation"
---

# Tasks: Text Quiz Generator

**Input**: Design documents from
`/specs/001-text-quiz-generator/`

**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`,
`contracts/`, and `quickstart.md`

**Tests**: Required by the project constitution and feature acceptance criteria.
Use deterministic fake-provider tests; do not call the live AI provider from
automated tests.

**Organization**: Tasks are grouped by user story so each story can be
implemented and validated as an incremental product slice.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel with other tasks in the same phase after its
  stated prerequisites are complete.
- **[Story]**: Maps a task to `US1`, `US2`, or `US3` from `spec.md`.
- Every task includes the concrete file path it creates or updates.

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Create the Angular and ASP.NET Core workspaces and establish local
development tooling.

- [x] T001 [P] Create the Angular workspace and planned feature folders in `frontend/package.json`, `frontend/angular.json`, `frontend/tsconfig.json`, `frontend/src/app/`, and `frontend/e2e/`.
- [x] T002 [P] Create the .NET solution, ASP.NET Core API project, and test projects in `backend/TanyaQuiz.sln`, `backend/src/TanyaQuiz.Api/TanyaQuiz.Api.csproj`, `backend/tests/TanyaQuiz.Api.UnitTests/`, `backend/tests/TanyaQuiz.Api.IntegrationTests/`, and `backend/tests/TanyaQuiz.Api.ContractTests/`.
- [x] T003 [P] Add repository ignore rules for Node modules, .NET build output, local user secrets, and environment files in `.gitignore`.
- [ ] T004 Configure Angular unit-test, Playwright, and development scripts in `frontend/package.json`, `frontend/playwright.config.ts`, and `frontend/e2e/README.md`.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Establish shared domain types, API contracts, provider boundaries,
server-side state, and application plumbing. No user-story implementation can
be accepted until this phase is complete.

- [x] T005 [P] Define quiz domain entities, enums, and lifecycle states in `backend/src/TanyaQuiz.Api/Domain/Difficulty.cs`, `backend/src/TanyaQuiz.Api/Domain/Quiz.cs`, `backend/src/TanyaQuiz.Api/Domain/Question.cs`, and `backend/src/TanyaQuiz.Api/Domain/QuizAttempt.cs`.
- [x] T006 [P] Define versioned request, public-response, result, and error DTOs matching `specs/001-text-quiz-generator/contracts/quiz-api.openapi.yaml` in `backend/src/TanyaQuiz.Api/Contracts/QuizContracts.cs`.
- [x] T007 [P] Add server-side AI configuration binding and startup validation for provider, endpoint, model, timeout, and retry settings in `backend/src/TanyaQuiz.Api/Configuration/QuizAiOptions.cs` and `backend/src/TanyaQuiz.Api/appsettings.example.json`.
- [x] T008 [P] Define the quiz-generation provider interface and deterministic fake provider seam in `backend/src/TanyaQuiz.Api/Providers/IQuizGenerationProvider.cs` and `backend/src/TanyaQuiz.Api/Providers/FakeQuizGenerationProvider.cs`.
- [x] T009 [P] Implement the 60-minute in-memory quiz store with opaque IDs and expiration behavior in `backend/src/TanyaQuiz.Api/Services/IQuizStore.cs` and `backend/src/TanyaQuiz.Api/Services/InMemoryQuizStore.cs`.
- [x] T010 Configure ASP.NET Core dependency injection, CORS, HTTPS, OpenAPI, trace IDs, and structured error responses in `backend/src/TanyaQuiz.Api/Program.cs`, `backend/src/TanyaQuiz.Api/Infrastructure/ExceptionHandlingMiddleware.cs`, and `backend/src/TanyaQuiz.Api/Infrastructure/TraceIdMiddleware.cs`.
- [x] T011 [P] Define Angular shared quiz, question, answer, result, and API-error models in `frontend/src/app/shared/models/quiz.models.ts` and `frontend/src/app/shared/models/api-error.models.ts`.
- [x] T012 Configure Angular routes, feature shell, and quiz-session state boundary in `frontend/src/app/app.routes.ts`, `frontend/src/app/app.ts`, and `frontend/src/app/features/quiz/quiz-session.service.ts`.
- [x] T013 [P] Implement the Angular API client and shared request-state/error handling in `frontend/src/app/core/api/quiz-api.service.ts`, `frontend/src/app/core/api/api-error.interceptor.ts`, and `frontend/src/app/core/state/request-state.ts`.
- [ ] T014 [P] Add reusable backend and end-to-end fixtures with valid source text, deterministic 10-question provider output, and invalid provider cases in `backend/tests/TanyaQuiz.Api.UnitTests/Fixtures/QuizFixtures.cs`, `backend/tests/TanyaQuiz.Api.IntegrationTests/Fixtures/FakeProviderFixtures.cs`, and `frontend/e2e/fixtures/quiz-fixtures.ts`.

**Checkpoint**: The projects build, shared contracts compile, the API starts
without a client-exposed AI key, and deterministic fixtures are available for all
story tests.

---

## Phase 3: User Story 1 - Generate a Source-Grounded Quiz (Priority: P1) MVP

**Goal**: Let a learner enter any valid source text, select Easy, Medium, or
Hard, and receive exactly 10 grounded questions without seeing answer keys.

**Independent Test**: Submit valid source text at each difficulty using the fake
provider and the browser flow. Verify exactly 10 questions, four A-D options,
one server-held answer key, matching source evidence, and clear failures for
invalid or unsupported source material.

### Tests for User Story 1

- [x] T015 [P] [US1] Add request-validation tests for 100-character minimum, 20,000-character maximum, whitespace-only input, topic context length, and the three difficulty values in `backend/tests/TanyaQuiz.Api.UnitTests/Validation/GenerateQuizRequestValidatorTests.cs`.
- [x] T016 [P] [US1] Add generated-quiz validation tests for exactly 10 ordered questions, unique A-D options, one correct option, four explanations, duplicate detection, and matching evidence excerpts in `backend/tests/TanyaQuiz.Api.UnitTests/Validation/GeneratedQuizValidatorTests.cs`.
- [ ] T017 [P] [US1] Add OpenAPI contract and endpoint tests for `POST /api/v1/quizzes`, response redaction, validation errors, insufficient source errors, and provider failures in `backend/tests/TanyaQuiz.Api.ContractTests/QuizGenerationContractTests.cs`.
- [ ] T018 [P] [US1] Add Angular source-entry component tests for minimum length validation, difficulty selection, disabled generation, loading state, and preserved input after failure in `frontend/src/app/features/quiz/source-entry/source-entry.component.spec.ts`.

### Implementation for User Story 1

- [x] T019 [P] [US1] Implement the topic context and source-text form with Easy, Medium, and Hard controls in `frontend/src/app/features/quiz/source-entry/source-entry.component.ts`, `frontend/src/app/features/quiz/source-entry/source-entry.component.html`, and `frontend/src/app/features/quiz/source-entry/source-entry.component.scss`.
- [x] T020 [P] [US1] Implement server-side source-input validation and actionable field errors in `backend/src/TanyaQuiz.Api/Validation/GenerateQuizRequestValidator.cs`.
- [x] T021 [P] [US1] Implement the Gemma 4 26B A4B prompt builder and configured provider adapter with structured JSON requests, timeout, one retry, and startup capability checks in `backend/src/TanyaQuiz.Api/Providers/QuizPromptBuilder.cs` and `backend/src/TanyaQuiz.Api/Providers/GemmaQuizGenerationProvider.cs`.
- [x] T022 [P] [US1] Implement generated-question semantic validation and exact source-evidence matching in `backend/src/TanyaQuiz.Api/Validation/GeneratedQuizValidator.cs`.
- [x] T023 [US1] Implement quiz generation orchestration, provider failure mapping, cache creation, and public response redaction in `backend/src/TanyaQuiz.Api/Services/QuizGenerationService.cs` (depends on T020-T022).
- [x] T024 [US1] Implement `POST /api/v1/quizzes` and map generation, validation, insufficient-source, timeout, and unavailable-provider outcomes in `backend/src/TanyaQuiz.Api/Controllers/QuizzesController.cs` (depends on T023).
- [x] T025 [US1] Implement Angular generation state transitions and the question-only quiz view in `frontend/src/app/features/quiz/quiz-session.service.ts`, `frontend/src/app/features/quiz/quiz-run/quiz-run.component.ts`, `frontend/src/app/features/quiz/quiz-run/quiz-run.component.html`, and `frontend/src/app/features/quiz/quiz-run/quiz-run.component.scss` (depends on T024).
- [x] T026 [US1] Add backend integration coverage using the fake provider for source grounding, difficulty prompts, malformed output retry, provider timeout, and no-partial-quiz persistence in `backend/tests/TanyaQuiz.Api.IntegrationTests/QuizGenerationIntegrationTests.cs` (depends on T023-T024).
- [ ] T027 [US1] Add Playwright coverage for generating Easy, Medium, and Hard quizzes, displaying exactly 10 questions, rendering four A-D options, and confirming answer keys are absent before submission in `frontend/e2e/quiz-generation.spec.ts` (depends on T025).

**Checkpoint**: A learner can independently generate and inspect a complete,
source-grounded 10-question quiz at all three difficulty levels.

---

## Phase 4: User Story 2 - Complete the Quiz and Review Results (Priority: P1)

**Goal**: Let a learner answer every generated question and receive a
server-calculated score with explanations for all four options.

**Independent Test**: Seed a validated quiz with the fake provider, submit ten
known answers, and verify the score, selected answer, correct answer, four
option explanations, evidence, incomplete-submission handling, and resistance to
client-supplied answer keys or scores.

### Tests for User Story 2

- [x] T028 [P] [US2] Add scoring tests for scores from 0 through 10, correct-option matching, duplicate question IDs, unknown IDs, and option-label validation in `backend/tests/TanyaQuiz.Api.UnitTests/Services/QuizScoringServiceTests.cs`.
- [ ] T029 [P] [US2] Add OpenAPI contract tests for complete submissions, missing answers, duplicate answers, unknown question IDs, expired quizzes, and server-owned result fields in `backend/tests/TanyaQuiz.Api.ContractTests/QuizSubmissionContractTests.cs`.
- [ ] T030 [P] [US2] Add Angular answer and results component tests for selection persistence, unanswered-question focus, submit loading, score rendering, correct-answer display, and all four explanations in `frontend/src/app/features/quiz/quiz-run/quiz-run.component.spec.ts` and `frontend/src/app/features/quiz/quiz-results/quiz-results.component.spec.ts`.

### Implementation for User Story 2

- [x] T031 [P] [US2] Implement submission validation for exactly ten known question IDs and one A-D choice per question in `backend/src/TanyaQuiz.Api/Validation/SubmitQuizRequestValidator.cs`.
- [x] T032 [US2] Implement authoritative server-side scoring and result projection from cached answer keys, explanations, and evidence in `backend/src/TanyaQuiz.Api/Services/QuizSubmissionService.cs` (depends on T031).
- [x] T033 [US2] Implement `POST /api/v1/quizzes/{quizId}/submissions` with expiration, incomplete-answer, and tampering responses in `backend/src/TanyaQuiz.Api/Controllers/QuizzesController.cs` (depends on T032).
- [x] T034 [P] [US2] Add submit-quiz API methods and submission state transitions to `frontend/src/app/core/api/quiz-api.service.ts` and `frontend/src/app/features/quiz/quiz-session.service.ts`.
- [x] T035 [US2] Implement accessible answer controls, keyboard navigation, selection persistence, unanswered-question guards, and submit behavior in `frontend/src/app/features/quiz/quiz-run/quiz-run.component.ts`, `frontend/src/app/features/quiz/quiz-run/quiz-run.component.html`, and `frontend/src/app/features/quiz/quiz-run/quiz-run.component.scss` (depends on T034).
- [x] T036 [US2] Implement the results view with score out of 10, selected and correct answers, all four option explanations, evidence excerpts, and retryable errors in `frontend/src/app/features/quiz/quiz-results/quiz-results.component.ts`, `frontend/src/app/features/quiz/quiz-results/quiz-results.component.html`, and `frontend/src/app/features/quiz/quiz-results/quiz-results.component.scss` (depends on T033-T035).
- [ ] T037 [US2] Add backend integration coverage proving server-side score calculation, answer-key secrecy before submission, incomplete submission rejection, and client-score tampering rejection in `backend/tests/TanyaQuiz.Api.IntegrationTests/QuizSubmissionIntegrationTests.cs` (depends on T032-T033).
- [ ] T038 [US2] Add Playwright coverage for answering all ten questions, blocking incomplete submission, showing the score, and reviewing every option explanation and evidence excerpt in `frontend/e2e/quiz-results.spec.ts` (depends on T036).

**Checkpoint**: User Stories 1 and 2 work together, while seeded fixtures allow
the submission and results flow to be tested independently of the live model.

---

## Phase 5: User Story 3 - Regenerate a Quiz (Priority: P2)

**Goal**: Let a learner create a fresh quiz from the same source and difficulty
without re-entering the source, while clearing all prior answers and scores.

**Independent Test**: Seed an active or submitted quiz, invoke regeneration, and
verify a new 10-question quiz with the same source and difficulty, a new ID,
cleared answers, and correct expiration/error behavior.

### Tests for User Story 3

- [ ] T039 [P] [US3] Add OpenAPI contract tests for regeneration from ready and submitted quizzes, new quiz IDs, preserved source/difficulty, and expired-quiz failures in `backend/tests/TanyaQuiz.Api.ContractTests/QuizRegenerationContractTests.cs`.
- [ ] T040 [P] [US3] Add Angular regeneration tests for action visibility, loading, state reset, preserved source context, and retryable failure handling in `frontend/src/app/features/quiz/quiz-results/regeneration.spec.ts`.

### Implementation for User Story 3

- [x] T041 [US3] Implement regeneration orchestration that reuses the cached source and difficulty, supersedes the old quiz, and stores a fresh validated quiz in `backend/src/TanyaQuiz.Api/Services/QuizRegenerationService.cs` (depends on T023 and T032).
- [x] T042 [US3] Implement `POST /api/v1/quizzes/{quizId}/regenerations` and map provider, source, and expiration errors in `backend/src/TanyaQuiz.Api/Controllers/QuizzesController.cs` (depends on T041).
- [x] T043 [US3] Add regeneration API behavior and quiz-session reset that clears answers, score, and results before displaying the new quiz in `frontend/src/app/core/api/quiz-api.service.ts`, `frontend/src/app/features/quiz/quiz-session.service.ts`, and `frontend/src/app/features/quiz/quiz-results/quiz-results.component.ts` (depends on T042).
- [ ] T044 [US3] Add expiration and retry integration coverage for regeneration, including preserved source text and selected difficulty after provider failure, in `backend/tests/TanyaQuiz.Api.IntegrationTests/QuizRegenerationIntegrationTests.cs` and `frontend/src/app/features/quiz/quiz-results/regeneration.spec.ts` (depends on T041-T043).
- [ ] T045 [US3] Add Playwright coverage for regeneration from both quiz and results states, new-question display, cleared selections, and unchanged source/difficulty context in `frontend/e2e/quiz-regeneration.spec.ts` (depends on T043).

**Checkpoint**: All three user stories are available, independently testable
with fixtures, and preserve the fixed quiz contract across regeneration.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Complete security, accessibility, observability, documentation,
contract validation, and release-readiness checks across all stories.

- [ ] T046 [P] Add redacted structured logging, provider latency metrics, trace IDs, and validation outcome metrics without logging source text, answer keys, explanations, or credentials in `backend/src/TanyaQuiz.Api/Diagnostics/QuizMetrics.cs`, `backend/src/TanyaQuiz.Api/Diagnostics/QuizLogging.cs`, and `backend/src/TanyaQuiz.Api/Program.cs`.
- [ ] T047 [P] Add shared Angular loading, timeout, retry, expiration, and responsive error states in `frontend/src/app/shared/components/request-status/`, `frontend/src/app/features/quiz/source-entry/source-entry.component.ts`, and `frontend/src/app/features/quiz/quiz-results/quiz-results.component.ts`.
- [ ] T048 [P] Add accessibility and keyboard-flow end-to-end checks for labels, focus order, option controls, error announcements, and mobile layout in `frontend/e2e/accessibility.spec.ts` and `frontend/src/styles.scss`.
- [ ] T049 [P] Add an OpenAPI lint and contract-validation command for `specs/001-text-quiz-generator/contracts/quiz-api.openapi.yaml` in `scripts/validate-openapi.sh` and `package.json`.
- [ ] T050 [P] Document Angular/.NET setup, provider configuration, secret handling, and links to the feature quickstart in `README.md`.
- [ ] T051 Run the complete validation sequence from `specs/001-text-quiz-generator/quickstart.md`, including backend tests, Angular tests, Playwright tests, OpenAPI linting, and the manual source-grounding review before release.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No feature dependencies; T001-T003 can start in parallel,
  and T004 follows the Angular workspace creation.
- **Foundational (Phase 2)**: Starts after the projects exist and blocks all
  user-story acceptance. Domain, contract, options, provider, cache, and shared
  model tasks can proceed in parallel before pipeline/state integration.
- **User Stories (Phases 3-5)**: Story work starts after the foundation. US1 is
  the first complete product slice. US2 relies on a generated or seeded Quiz;
  US3 relies on the active quiz/session behavior from US1 and the results state
  from US2.
- **Polish (Phase 6)**: Starts after the desired user stories are complete.

### User Story Dependencies

- **US1 (P1)**: Depends only on Phase 2 and delivers the generation MVP.
- **US2 (P1)**: Backend scoring can be developed with seeded fixtures after
  Phase 2, but integrated acceptance depends on US1's generated quiz contract.
- **US3 (P2)**: Depends on US1's generation/session state and US2's result state
  for the full regeneration journey.

### Parallel Opportunities

- T001, T002, and T003 can run in parallel during setup.
- T005-T009, T011, T013, and T014 can run in parallel after project creation.
- US1 test tasks T015-T018 can run in parallel; T019-T022 can also run in
  parallel before T023 orchestration.
- US2 test tasks T028-T030 can run in parallel; T031 and T034 can proceed in
  parallel before the submission endpoint and UI integration are joined.
- US3 test tasks T039-T040 can run in parallel; backend regeneration work and
  frontend regeneration tests can proceed with seeded quiz fixtures.
- T046-T050 are independent final hardening tasks and can run in parallel.

## Parallel Example: User Story 1

```text
Task: T015 request validation tests in backend/tests/TanyaQuiz.Api.UnitTests/Validation/GenerateQuizRequestValidatorTests.cs
Task: T016 generated output tests in backend/tests/TanyaQuiz.Api.UnitTests/Validation/GeneratedQuizValidatorTests.cs
Task: T017 API contract tests in backend/tests/TanyaQuiz.Api.ContractTests/QuizGenerationContractTests.cs
Task: T018 source-entry tests in frontend/src/app/features/quiz/source-entry/source-entry.component.spec.ts

Task: T019 source-entry UI in frontend/src/app/features/quiz/source-entry/
Task: T020 request validator in backend/src/TanyaQuiz.Api/Validation/GenerateQuizRequestValidator.cs
Task: T021 Gemma provider adapter in backend/src/TanyaQuiz.Api/Providers/GemmaQuizGenerationProvider.cs
Task: T022 generated quiz validator in backend/src/TanyaQuiz.Api/Validation/GeneratedQuizValidator.cs
```

## Parallel Example: User Story 2

```text
Task: T028 scoring tests in backend/tests/TanyaQuiz.Api.UnitTests/Services/QuizScoringServiceTests.cs
Task: T029 submission contract tests in backend/tests/TanyaQuiz.Api.ContractTests/QuizSubmissionContractTests.cs
Task: T030 Angular answer/results tests in frontend/src/app/features/quiz/

Task: T031 submission validator in backend/src/TanyaQuiz.Api/Validation/SubmitQuizRequestValidator.cs
Task: T034 submit API client/state in frontend/src/app/core/api/quiz-api.service.ts
```

## Parallel Example: User Story 3

```text
Task: T039 regeneration contract tests in backend/tests/TanyaQuiz.Api.ContractTests/QuizRegenerationContractTests.cs
Task: T040 regeneration UI tests in frontend/src/app/features/quiz/quiz-results/regeneration.spec.ts
Task: T041 regeneration service in backend/src/TanyaQuiz.Api/Services/QuizRegenerationService.cs
```

## Implementation Strategy

### MVP First

1. Complete Phase 1 setup.
2. Complete Phase 2 foundation and verify both projects build.
3. Complete Phase 3 User Story 1.
4. Stop and validate generation with the fake provider and the manual
   source-grounding review.
5. Add Phase 4 User Story 2 for the learner-visible MVP: answer, score, and
   explanations.

### Incremental Delivery

1. Deliver source-grounded quiz generation as the first demonstrable slice.
2. Add server-authoritative scoring and complete feedback without exposing keys
   before submission.
3. Add regeneration and expiration handling.
4. Complete cross-cutting hardening, accessibility, documentation, and release
   checks.

### Notes

- Every task must remain unchecked until its file changes and focused validation
  are complete.
- Tests must use the fake provider unless a manual smoke test explicitly targets
  the configured Gemma deployment.
- Do not commit Google AI Studio keys, provider credentials, raw source text, or
  generated answer keys.
- Stop at each story checkpoint and verify that earlier stories still pass.
