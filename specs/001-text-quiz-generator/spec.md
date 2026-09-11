# Feature Specification: Text Quiz Generator

**Feature Branch**: `001-text-quiz-generator` (planned)

**Created**: 2026-09-11

**Status**: Draft

**Input**: User description: Allow a user to paste at least 100 characters on any topic, choose Easy, Medium, or Hard difficulty, generate exactly 10 multiple-choice questions, answer them, review a score and explanations for every option, and regenerate a quiz from the same text.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Generate a Source-Grounded Quiz (Priority: P1)

As a learner, I want to paste text about any topic and choose a difficulty so
that I can practice understanding the material.

**Why this priority**: Quiz generation is the primary product value and is
required before any other journey can provide value.

**Independent Test**: Paste valid source text, choose each difficulty in turn,
and verify that each request produces a complete 10-question quiz grounded in
the supplied text.

**Acceptance Scenarios**:

1. **Given** the user has pasted at least 100 non-whitespace characters, **When**
   the user selects Easy, Medium, or Hard and chooses Generate Quiz, **Then**
   the system creates exactly 10 multiple-choice questions.
2. **Given** a generated quiz, **When** the user reviews any question, **Then**
   it has four options labeled A-D, exactly one correct answer, an explanation
   for the correct answer, and an explanation for each incorrect option.
3. **Given** the same source text, **When** the user generates quizzes at
   different difficulty levels, **Then** the questions reflect the selected
   level: recall for Easy, understanding and application for Medium, and
   analysis or inference for Hard.

---

### User Story 2 - Complete the Quiz and Review Results (Priority: P1)

As a learner, I want to answer the questions and see meaningful feedback so
that I can identify what I understand and what I need to review.

**Why this priority**: Scoring and explanations turn generated questions into
an effective learning experience.

**Independent Test**: Complete all 10 questions with known choices, submit the
quiz, and verify the score and question-by-question feedback.

**Acceptance Scenarios**:

1. **Given** a generated quiz, **When** the user selects one option for each
   question and submits, **Then** the system displays a score out of 10 and the
   correct answer for every question.
2. **Given** submitted answers, **When** the user reviews the results, **Then**
   the system shows the explanation for the correct option and why each of the
   three incorrect options is wrong, regardless of whether the user's answer
   was correct.
3. **Given** one or more unanswered questions, **When** the user tries to
   submit, **Then** the system identifies the unanswered questions and does not
   present an incomplete score.

---

### User Story 3 - Regenerate a Quiz (Priority: P2)

As a learner, I want to generate another quiz from the same text so that I can
practice again without re-entering the source material.

**Why this priority**: Regeneration supports repeated practice while keeping
the MVP focused on one source and one active quiz session.

**Independent Test**: Generate and complete a quiz, choose Regenerate, and
verify that a fresh quiz is presented using the same source and difficulty.

**Acceptance Scenarios**:

1. **Given** a generated quiz or its results, **When** the user chooses
   Regenerate Quiz, **Then** the system creates a new set of exactly 10
   questions from the same source and selected difficulty.
2. **Given** a regenerated quiz, **When** it is displayed, **Then** previous
   answers and scores are cleared and the new quiz is ready to answer.

### Edge Cases

- Text with fewer than 100 non-whitespace characters MUST be rejected with a
  clear validation message, including text containing only spaces or line breaks.
- If the source does not contain enough reliable information for 10 questions,
  the system MUST explain that the source is insufficient and MUST NOT invent
  unsupported answers.
- If generation fails or takes too long, the system MUST preserve the source
  text and selected difficulty and offer a retry.
- Pasted text containing line breaks, punctuation, or common special characters
  MUST be accepted when it meets the minimum length.
- A user leaving the quiz before submission MUST be able to return to the
  current unanswered quiz without the score being shown prematurely.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST present a clear prompt for the user to provide
  topic context and paste source text.
- **FR-002**: The system MUST accept source text on any topic and MUST require
  at least 100 non-whitespace characters before quiz generation.
- **FR-003**: The system MUST provide exactly three difficulty choices: Easy,
  Medium, and Hard.
- **FR-004**: The system MUST generate exactly 10 questions based only on the
  supplied source text and selected difficulty.
- **FR-005**: Every question MUST contain four options labeled A-D and exactly
  one correct answer.
- **FR-006**: Every question MUST include an explanation for the correct answer
  and a separate explanation for each incorrect option.
- **FR-007**: Easy questions MUST emphasize recall and basic understanding;
  Medium questions MUST emphasize conceptual understanding and application; and
  Hard questions MUST emphasize analysis, inference, and critical thinking.
- **FR-008**: The system MUST allow the user to select one answer for each
  question and MUST preserve those selections until submission or regeneration.
- **FR-009**: The system MUST calculate and display a score out of 10 after a
  complete submission.
- **FR-010**: Results MUST display the user's answer, the correct answer, and
  explanations for all four options for every question.
- **FR-011**: The system MUST identify unanswered questions and prevent an
  incomplete submission from producing a score.
- **FR-012**: The system MUST allow the user to regenerate a quiz from the same
  source text and selected difficulty.
- **FR-013**: Regeneration MUST clear prior answers and scores before displaying
  the new quiz.
- **FR-014**: User-facing validation and generation errors MUST be clear,
  actionable, and MUST preserve the user's source text when retry is possible.
- **FR-015**: The system MUST avoid presenting questions, answers, or
  explanations that cannot be supported by the supplied source text.

### Key Entities *(include if feature involves data)*

- **Source Text**: The topic context and pasted material provided by the user.
- **Difficulty Selection**: The user's chosen level: Easy, Medium, or Hard.
- **Quiz**: A generated set of exactly 10 questions tied to one source and one
  difficulty selection.
- **Question**: A prompt with four answer options, one correct answer, and four
  explanations.
- **Quiz Attempt**: The user's selected answers, submission state, score, and
  reviewable results for one quiz.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: At least 95% of valid generation requests produce a complete quiz
  within 5 seconds under normal service conditions.
- **SC-002**: 100% of quizzes accepted by the system contain exactly 10
  questions, four options per question, and exactly one correct answer per
  question.
- **SC-003**: 100% of questions in the acceptance-test set include a correct
  answer explanation and explanations for all three incorrect options.
- **SC-004**: In acceptance review, 100% of questions, answers, and
  explanations can be traced to the supplied source text.
- **SC-005**: At least 90% of first-time test users can generate a quiz and
  submit a complete attempt without assistance.
- **SC-006**: At least 90% of test users can identify their score and locate the
  explanation for any selected question after submission.

## Assumptions

- The MVP does not require user accounts or long-term quiz history; one active
  quiz session is sufficient.
- Source text is provided by the user and is available during the current quiz
  session; the system does not perform external research to fill gaps.
- The application provides an English-language user interface for the MVP;
  support for additional interface languages is outside this feature.
- Users have a stable connection while generating and submitting a quiz.
- If a source is too short, repetitive, or lacks enough reliable information,
  showing an actionable failure message is preferable to generating unsupported
  content.
