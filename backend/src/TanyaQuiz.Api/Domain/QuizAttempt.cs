namespace TanyaQuiz.Api.Domain;

public sealed class QuizAttempt
{
    public Guid QuizId { get; init; }

    public Dictionary<Guid, OptionLabel> Answers { get; init; } = [];

    public int? Score { get; set; }

    public DateTimeOffset? SubmittedAt { get; set; }
}
