# AI Quiz Generator PRD
## Product Goal

Help users improve comprehension and retention by turning any pasted text into an interactive quiz with useful feedback.

## Target Users

Students, teachers, trainers, corporate learners, and anyone reviewing content.

## Core User Flow
1. When user starts the chat, prompt the user to enter the topic for the quiz for context where the user pastes at least 100 characters on any topic.
2. User selects a difficulty: **Easy**, **Medium**, or **Hard**.
3. User selects **Generate Quiz**.
4. The system creates exactly 10 multiple-choice questions.
5. User answers and submits the quiz.
6. The system shows the score, correct answers, and explanations. User can regenerate a quiz from the same text.

## Functional Requirements

- Accept pasted text from any subject or domain; require at least 100 characters.
- Generate exactly 10 relevant questions based only on the supplied text.
- Give every question four options labeled A-D and exactly one correct answer.
- Apply the selected difficulty:
	- **Easy:** recall and basic understanding.
	- **Medium:** conceptual understanding and application.
	- **Hard:** analysis, inference, and critical thinking.
- Provide an explanation for the correct answer and a clear explanation for why each of the three incorrect options is wrong.
- Let users answer all questions, submit the quiz, view a score out of 10, and review feedback for every question.

## MVP Success Criteria

- A valid input produces 10 questions within a few seconds.
- Questions are relevant to the source text and reflect the selected difficulty.
- Correct answers and all explanations are supported by the source text.
- Results clearly show the score, correct answer, and explanations for all four options.
