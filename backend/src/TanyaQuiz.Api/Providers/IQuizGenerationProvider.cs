using TanyaQuiz.Api.Domain;

namespace TanyaQuiz.Api.Providers;

public sealed record QuizGenerationInput(
    string? TopicContext,
    string SourceText,
    Difficulty Difficulty);

public sealed class GeneratedQuizCandidate
{
    public List<GeneratedQuestionCandidate> Questions { get; init; } = [];
}

public sealed class GeneratedQuestionCandidate
{
    public int Number { get; init; }

    public string Prompt { get; init; } = string.Empty;

    public List<GeneratedOptionCandidate> Options { get; init; } = [];

    public string CorrectOption { get; init; } = string.Empty;

    public Dictionary<string, string> Explanations { get; init; } = [];

    public List<GeneratedEvidenceCandidate> Evidence { get; init; } = [];
}

public sealed class GeneratedOptionCandidate
{
    public string Label { get; init; } = string.Empty;

    public string Text { get; init; } = string.Empty;
}

public sealed class GeneratedEvidenceCandidate
{
    public string Quote { get; init; } = string.Empty;

    public int SourceStart { get; init; }

    public int SourceEnd { get; init; }
}

public interface IQuizGenerationProvider
{
    Task<GeneratedQuizCandidate> GenerateAsync(
        QuizGenerationInput input,
        CancellationToken cancellationToken = default);
}

public sealed class QuizProviderException : Exception
{
    public QuizProviderException(string message, int statusCode = 502, Exception? innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }

    public int StatusCode { get; }
}
