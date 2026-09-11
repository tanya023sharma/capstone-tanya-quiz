# Implementation Plan: Text Quiz Generator

**Branch**: `001-text-quiz-generator` | **Date**: 2026-09-11 | **Spec**:
[spec.md](./spec.md)

**Input**: Feature specification from
`/specs/001-text-quiz-generator/spec.md`

## Summary

Build an anonymous web application that turns user-provided text into exactly
10 source-grounded multiple-choice questions. An Angular client collects topic
context, source text, and difficulty; it displays questions, preserves answers,
and presents results. An ASP.NET Core API validates requests, calls a configured
Gemma 4 26B A4B provider, validates the generated structure and evidence,
scores submissions server-side, and supports regeneration from the retained
source. Correct answers and explanations remain on the server until submission.

The user-provided `rangular` label is interpreted as Angular. The published
`rangular` package is an unrelated legacy AngularJS package and conflicts with
the project constitution's Angular requirement.

## Technical Context

**Language/Version**: TypeScript 5.x with Angular current stable release;
C# with .NET 10 LTS and ASP.NET Core

**Primary Dependencies**: Angular standalone components, Reactive Forms,
HttpClient, and the Angular CLI; ASP.NET Core Web API, `IHttpClientFactory`,
`System.Text.Json`, OpenAPI, `IMemoryCache`, and xUnit test tooling

**Storage**: MVP uses server-side in-memory cache with a 60-minute expiration
for active quizzes and answer keys; no database, accounts, or quiz history

**Testing**: Angular unit tests through `ng test`, ASP.NET Core xUnit unit and
integration tests, OpenAPI contract validation, and Playwright end-to-end tests

**Target Platform**: Modern desktop and mobile browsers; Linux-hosted ASP.NET
Core service behind HTTPS

**Project Type**: Web application with a single-page Angular frontend and a
versioned REST API

**Performance Goals**: At least 95% of valid generation requests complete in
5 seconds or less under normal service conditions; scoring and results render
within 1 second after a complete submission

**Constraints**: Source text must contain 100 to 20,000 non-whitespace
characters; every quiz has exactly 10 questions, 4 options per question, and 1
correct answer; all explanations need source evidence; API credentials stay on
the server; raw source text and credentials are excluded from logs

**Scale/Scope**: MVP supports anonymous users with one active quiz per browser
session, one selected difficulty per quiz, no persistent history, and no
external research beyond the supplied text

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Gate | Design Evidence | Status |
| --- | --- | --- |
| Source-grounded quiz contract | Provider output includes source evidence; backend validates 10 questions, A-D options, one answer, and four explanations before a quiz is released. | PASS |
| Testable product behavior | Acceptance paths map to unit, integration, contract, and end-to-end test layers. | PASS |
| Angular user experience | The frontend is a standard Angular single-page application with accessible form and answer controls. | PASS |
| .NET Core service boundary | ASP.NET Core owns validation, generation orchestration, cache state, scoring, and the public JSON contract. | PASS |
| Secure and focused delivery | The AI key is server-only; quiz IDs are opaque; source text is short-lived and not logged. | PASS |

**Initial gate evaluation**: PASS. No constitutional violations require a
complexity exception.

**Post-design re-check**: PASS. The data model and API contract keep answer keys
off the client before submission, enforce the fixed quiz structure, preserve
source grounding evidence, and use a server-side provider adapter for all AI
credentials.

## Project Structure

### Documentation (this feature)

```text
specs/001-text-quiz-generator/
|- plan.md
|- research.md
|- data-model.md
|- quickstart.md
|- contracts/
|  `- quiz-api.openapi.yaml
|- checklists/
|  `- requirements.md
`- tasks.md                  # Created by /speckit-tasks, not this command
```

### Source Code (repository root)

```text
frontend/
|- src/
|  |- app/
|  |  |- core/
|  |  |  `- api/
|  |  |- features/
|  |  |  `- quiz/
|  |  |     |- source-entry/
|  |  |     |- quiz-run/
|  |  |     `- quiz-results/
|  |  `- shared/
|  |     `- models/
|  `- environments/
`- e2e/

backend/
|- TanyaQuiz.sln
|- src/
|  `- TanyaQuiz.Api/
|     |- Controllers/
|     |- Contracts/
|     |- Domain/
|     |- Providers/
|     |- Services/
|     `- Validation/
`- tests/
   |- TanyaQuiz.Api.UnitTests/
   |- TanyaQuiz.Api.IntegrationTests/
   `- TanyaQuiz.Api.ContractTests/
```

**Structure Decision**: Use a two-project web application. The Angular project
owns user interaction and transient client state. The ASP.NET Core project owns
business rules, correct-answer state, provider access, and scoring. The boundary
is defined by [quiz-api.openapi.yaml](./contracts/quiz-api.openapi.yaml).
