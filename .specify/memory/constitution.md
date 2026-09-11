<!--
Sync Impact Report
- Version change: uninitialized -> 1.0.0
- Modified principles: template placeholders -> five initial project principles
- Added sections: Technology and Architecture Constraints; Development Workflow
- Removed sections: none
- Follow-up TODOs: none; the scaffold had no prior adopted constitution, so the
	ratification date is set to 2026-09-11.
-->

# Tanya Quiz Constitution

## Core Principles

### I. Source-Grounded Quiz Contract

The system MUST accept at least 100 characters of user-provided text on any
topic and generate exactly 10 multiple-choice questions from that source. Each
question MUST contain four options labeled A-D, exactly one correct answer, and
explanations for the correct answer and each incorrect option. Questions,
answers, and explanations MUST be supported by the supplied text. The selected
difficulty MUST control the cognitive demand: Easy covers recall, Medium covers
understanding and application, and Hard covers analysis and inference.

Rationale: Source grounding and a fixed output contract make the quiz useful,
reviewable, and predictable for learners.

### II. Testable Product Behavior

Every feature MUST have clear acceptance criteria before implementation. Tests
MUST cover input validation, the 10-question output contract, difficulty
behavior, answer scoring, explanation completeness, and failure states. Changes
to shared contracts MUST include regression coverage.

Rationale: Testable behavior prevents silent regressions in the learning flow
and keeps requirements verifiable across the frontend and backend.

### III. Angular User Experience

The frontend MUST be implemented in Angular. It MUST guide the user from topic
or context entry and source-text paste through difficulty selection, quiz
answering, submission, results review, and regeneration. Controls MUST expose
clear validation, preserve answer state until submission, and support keyboard
navigation and accessible labels.

Rationale: A consistent, accessible flow reduces friction during repeated study
sessions and makes the product usable across supported devices.

### IV. .NET Core Service Boundary

The backend MUST be implemented in .NET Core and own input validation, quiz
generation orchestration, scoring, and result contracts. Frontend and backend
communication MUST use explicit, versioned JSON API contracts with structured
error responses. The backend MUST never trust client-provided scores or answer
keys.

Rationale: A clear service boundary protects the quiz contract and keeps
validation and scoring authoritative.

### V. Secure and Focused Delivery

Secrets, model credentials, and service configuration MUST remain server-side
and outside source control. User text MUST be handled as untrusted input and
validated at the service boundary. Implementation MUST prefer the smallest
design that satisfies the product contract, with structured logs for failures
and generation latency.

Rationale: Privacy, diagnosability, and simplicity are essential for a tool that
processes arbitrary user content and must respond within a few seconds.

## Technology and Architecture Constraints

- The application MUST use Angular for the frontend and .NET Core for the
	backend.
- The backend MUST expose the APIs required by the Angular client for source
	text, difficulty, quiz questions, submissions, results, and regeneration.
- API schemas, validation rules, and error shapes MUST be documented alongside
	the relevant feature specification.
- API keys and other secrets MUST be supplied through environment or deployment
	configuration, never committed to the repository or sent to the browser.

## Development Workflow

- Work MUST begin from a feature specification with user-visible acceptance
	criteria.
- A change MUST include focused unit tests and integration tests when it crosses
	the Angular/.NET Core boundary or changes a shared contract.
- Reviews MUST verify the source-grounding rule, exact question count, complete
	explanations, selected difficulty behavior, accessibility, and security
	constraints.
- Performance-sensitive changes MUST measure quiz-generation latency and record
	failures in a way that supports diagnosis without logging secrets or raw
	sensitive content.

## Governance

This constitution defines the non-negotiable engineering rules for the Tanya
Quiz project. It takes precedence over conflicting local practices. Every
feature specification, implementation plan, task list, and review MUST account
for the applicable principles and constraints.

Amendments MUST document the reason, affected principles, compatibility impact,
and any required migration work. The change MUST update the Sync Impact Report,
the version, and the Last Amended date in this file.

Versioning follows semantic versioning. A MAJOR release removes or changes a
non-negotiable rule, a MINOR release adds a principle or materially expands
governance, and a PATCH release clarifies wording without changing obligations.

Compliance MUST be reviewed at feature planning and code review. Exceptions
MUST be explicit, time-bounded, approved by the project owner, and recorded with
their risk and removal plan.

**Version**: 1.0.0 | **Ratified**: 2026-09-11 | **Last Amended**: 2026-09-11
