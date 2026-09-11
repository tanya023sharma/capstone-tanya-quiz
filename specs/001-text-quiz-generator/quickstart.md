# Quickstart: Text Quiz Generator

This guide defines the end-to-end validation path after
`/speckit-implement` creates the planned Angular and ASP.NET Core projects.

## Prerequisites

- Node.js 22 or later and npm. The current workspace has Node.js 22.22.2.
- .NET 10 SDK. It is not installed in the current workspace.
- A server-side AI provider endpoint that can access the requested
  `gemma-4-26b-a4b` model and return structured JSON.
- A provider credential stored outside source control. A Google AI Studio key is
  exposed as `GEMINI_API_KEY` only when the configured provider accepts that
  credential.

## Configure the Backend

Set local development secrets in the backend project. Do not add a credential to
an Angular environment file, `appsettings*.json`, or a committed `.env` file.

```bash
export QUIZ_AI_PROVIDER="configured-provider"
export QUIZ_AI_ENDPOINT="https://your-model-provider.example"
export QUIZ_AI_MODEL="gemma-4-26b-a4b"
export GEMINI_API_KEY="your-google-ai-studio-key"

cd backend
dotnet user-secrets set --project src/TanyaQuiz.Api "QuizAi:Provider" "$QUIZ_AI_PROVIDER"
dotnet user-secrets set --project src/TanyaQuiz.Api "QuizAi:Endpoint" "$QUIZ_AI_ENDPOINT"
dotnet user-secrets set --project src/TanyaQuiz.Api "QuizAi:Model" "$QUIZ_AI_MODEL"
dotnet user-secrets set --project src/TanyaQuiz.Api "QuizAi:ApiKey" "$GEMINI_API_KEY"
```

Before starting the application, verify the chosen endpoint exposes the requested
model and supports the structured-output requirements recorded in
[research.md](./research.md). If it does not, correct the server-side provider
configuration rather than substituting a model in client code.

## Run Locally

Start the API in one terminal:

```bash
cd backend
dotnet restore TanyaQuiz.sln
dotnet run --project src/TanyaQuiz.Api/TanyaQuiz.Api.csproj
```

Start the Angular application in a second terminal:

```bash
cd frontend
npm install
npm start
```

Open the URL shown by the Angular CLI. The client must call the local API only;
it must never call the AI provider directly.

## Validate the Main Flow

1. Enter an optional topic context and paste at least 100 non-whitespace
   characters of source text.
2. Select Easy and generate a quiz. Verify that exactly 10 questions appear and
   each has four A-D options with no correct-answer feedback shown yet.
3. Answer all questions, submit, and verify the score is from 0 through 10.
4. For every result, verify that the selected answer, correct answer, all four
   explanations, and source evidence are visible.
5. Repeat with Medium and Hard. Verify the intended progression from recall to
   application to analysis or inference.
6. Select Regenerate Quiz. Verify that the same source and difficulty create a
   new 10-question quiz and clear prior selections and score.

## Validate API Behavior

Create a quiz using a valid source payload. The generated response must not
contain `correctOption`, `explanation`, or an answer key.

```bash
curl --fail-with-body --request POST "http://localhost:5000/api/v1/quizzes" \
  --header "Content-Type: application/json" \
  --data '{
    "topicContext": "Water cycle",
    "difficulty": "easy",
    "sourceText": "The water cycle moves water through evaporation, condensation, precipitation, and collection. Heat from the sun causes liquid water to evaporate into water vapor. As the vapor cools, it condenses into clouds. Water returns to Earth as precipitation and collects in bodies of water."
  }'
```

Then use the returned quiz and question IDs to submit exactly one A-D answer for
each question as defined in
[quiz-api.openapi.yaml](./contracts/quiz-api.openapi.yaml). The response must
contain 10 results, a server-calculated score, four explained options per result,
and at least one evidence excerpt per question.

Test a source with fewer than 100 non-whitespace characters. The API must return
`400` with `SOURCE_TEXT_TOO_SHORT`; the frontend must retain the pasted text and
show an actionable message.

## Run Automated Checks

```bash
cd backend
dotnet test TanyaQuiz.sln

cd ../frontend
npm test -- --watch=false
npx playwright test
```

The automated suite must cover input validation, the fixed 10-question contract,
difficulty behavior, answer scoring, all explanations, regeneration, expired
quiz handling, provider failure, and keyboard-accessible answer selection.

## Expected Failure Handling

- A source that cannot support 10 grounded questions returns
  `INSUFFICIENT_SOURCE_MATERIAL`, not invented questions.
- Provider timeout or unavailability returns a retryable error and leaves the
  pasted source and selected difficulty intact in the client.
- A partial, duplicate, or altered answer submission returns a validation error
  and never receives a score.
- An expired quiz returns `QUIZ_EXPIRED` and requires a new quiz generation.
