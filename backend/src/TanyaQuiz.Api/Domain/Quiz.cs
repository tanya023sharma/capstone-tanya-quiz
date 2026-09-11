namespace TanyaQuiz.Api.Domain;

public sealed class Quiz
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string? TopicContext { get; init; }

    public string SourceText { get; init; } = string.Empty;

    public Difficulty Difficulty { get; init; }

    public List<Question> Questions { get; init; } = [];

    public QuizStatus Status { get; set; } = QuizStatus.Ready;

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    public DateTimeOffset ExpiresAt { get; init; }

    public string ProviderModel { get; init; } = string.Empty;

    public string TraceId { get; init; } = string.Empty;
}
