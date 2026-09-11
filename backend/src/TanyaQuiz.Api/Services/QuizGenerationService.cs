using System.Diagnostics;
using TanyaQuiz.Api.Configuration;
using TanyaQuiz.Api.Contracts;
using TanyaQuiz.Api.Diagnostics;
using TanyaQuiz.Api.Domain;
using TanyaQuiz.Api.Providers;
using TanyaQuiz.Api.Validation;

namespace TanyaQuiz.Api.Services;

public sealed class QuizGenerationService(
    IQuizGenerationProvider provider,
    IQuizStore quizStore,
    GenerateQuizRequestValidator requestValidator,
    GeneratedQuizValidator generatedQuizValidator,
    QuizAiOptions options,
    QuizMetrics metrics,
    ILogger<QuizGenerationService> logger)
{
    public async Task<QuizResponse> GenerateAsync(
        GenerateQuizRequest request,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        metrics.GenerationStarted();
        var requestErrors = requestValidator.Validate(request);
        if (requestErrors.Count > 0)
        {
            metrics.GenerationFailed("VALIDATION_ERROR");
            throw new QuizServiceException(
                400,
                "VALIDATION_ERROR",
                "Review the highlighted fields and try again.",
                requestErrors);
        }

        GeneratedQuizCandidate candidate;
        try
        {
            candidate = await provider.GenerateAsync(
                new QuizGenerationInput(request.TopicContext, request.SourceText, request.Difficulty),
                cancellationToken);
        }
        catch (QuizProviderException exception)
        {
            metrics.GenerationFailed(exception.StatusCode == 504 ? "QUIZ_GENERATION_TIMED_OUT" : "QUIZ_GENERATION_UNAVAILABLE");
            logger.LogWarning(exception, "Quiz generation provider failed with status {StatusCode}.", exception.StatusCode);
            throw new QuizServiceException(
                exception.StatusCode,
                exception.StatusCode == 504 ? "QUIZ_GENERATION_TIMED_OUT" : "QUIZ_GENERATION_UNAVAILABLE",
                exception.StatusCode == 504
                    ? "Quiz generation took too long. Try again with the same text."
                    : "Quiz generation is temporarily unavailable. Try again shortly.");
        }

        var generatedErrors = generatedQuizValidator.Validate(candidate, request.SourceText);
        if (generatedErrors.Count > 0)
        {
            metrics.GenerationFailed("INVALID_GENERATION_OUTPUT");
            throw new QuizServiceException(
                502,
                "INVALID_GENERATION_OUTPUT",
                "The generation provider returned an incomplete quiz. Try again.");
        }

        var createdAt = DateTimeOffset.UtcNow;
        var quiz = new Quiz
        {
            TopicContext = request.TopicContext,
            SourceText = request.SourceText,
            Difficulty = request.Difficulty,
            CreatedAt = createdAt,
            ExpiresAt = createdAt.AddMinutes(60),
            ProviderModel = options.Model,
            TraceId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString("N"),
            Questions = candidate.Questions.Select(MapQuestion).ToList()
        };

        quizStore.Save(quiz);
        metrics.GenerationCompleted(stopwatch.Elapsed.TotalMilliseconds);
        return QuizMapper.ToResponse(quiz);
    }

    private static Question MapQuestion(GeneratedQuestionCandidate candidate)
    {
        return new Question
        {
            Number = candidate.Number,
            Prompt = candidate.Prompt,
            Options = candidate.Options.Select(option => new AnswerOption
            {
                Label = Enum.Parse<OptionLabel>(option.Label),
                Text = option.Text
            }).ToList(),
            CorrectOption = Enum.Parse<OptionLabel>(candidate.CorrectOption),
            Explanations = candidate.Explanations.ToDictionary(
                explanation => Enum.Parse<OptionLabel>(explanation.Key),
                explanation => explanation.Value),
            Evidence = candidate.Evidence.Select(evidence => new EvidenceExcerpt
            {
                Quote = evidence.Quote,
                SourceStart = evidence.SourceStart,
                SourceEnd = evidence.SourceEnd
            }).ToList()
        };
    }
}
