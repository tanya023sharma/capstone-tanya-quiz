namespace TanyaQuiz.Api.Domain;

public sealed class EvidenceExcerpt
{
    public string Quote { get; init; } = string.Empty;

    public int SourceStart { get; init; }

    public int SourceEnd { get; init; }
}
