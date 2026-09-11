# Tanya Quiz

Tanya Quiz turns pasted source text into a focused 10-question multiple-choice
quiz. It supports Easy, Medium, and Hard difficulty levels, server-side scoring,
source-traceable explanations for every option, and regeneration from the same
source.

## Stack

- Frontend: Angular 21
- Backend: ASP.NET Core on .NET 10
- AI provider: configurable Gemma 4 26B A4B adapter
- MVP state: 60-minute in-memory server cache; no accounts or long-term history

## Local Development

Install Node.js 22.12+ and the .NET 10 SDK. The backend uses the deterministic
fake provider by default, so a live model credential is not needed for local
tests.

Start the API:

```bash
export PATH="$HOME/.dotnet:$PATH"
dotnet run --project backend/src/TanyaQuiz.Api/TanyaQuiz.Api.csproj --urls http://127.0.0.1:5000
```

Start the Angular client:

```bash
npm --prefix frontend start -- --host 127.0.0.1 --port 4200
```

Open <http://127.0.0.1:4200>.

## Configure A Live Provider

Keep credentials on the server. Do not put them in Angular environment files,
committed settings, or browser requests.

```bash
dotnet user-secrets --project backend/src/TanyaQuiz.Api/TanyaQuiz.Api.csproj \
  set "QuizAi:Provider" "configured-provider"
dotnet user-secrets --project backend/src/TanyaQuiz.Api/TanyaQuiz.Api.csproj \
  set "QuizAi:Endpoint" "https://your-model-provider.example"
dotnet user-secrets --project backend/src/TanyaQuiz.Api/TanyaQuiz.Api.csproj \
  set "QuizAi:ApiKey" "$GEMINI_API_KEY"
dotnet user-secrets --project backend/src/TanyaQuiz.Api/TanyaQuiz.Api.csproj \
  set "QuizAi:Model" "gemma-4-26b-a4b"
```

See [the feature quickstart](specs/001-text-quiz-generator/quickstart.md) for
the provider contract, API examples, and failure-path validation.

## Validation

```bash
dotnet test backend/TanyaQuiz.sln
npm --prefix frontend test -- --watch=false
npm --prefix frontend run build
npm --prefix frontend run e2e
npm --prefix frontend run validate:openapi
```

The Playwright suite mocks the provider for deterministic browser tests. The
live provider is used only for an explicit manual smoke test.
