namespace TanyaQuiz.Api.Domain;

public sealed class Question
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public int Number { get; init; }

    public string Prompt { get; init; } = string.Empty;

    public List<AnswerOption> Options { get; init; } = [];

    public OptionLabel CorrectOption { get; init; }

    public Dictionary<OptionLabel, string> Explanations { get; init; } = [];

    public List<EvidenceExcerpt> Evidence { get; init; } = [];
}
