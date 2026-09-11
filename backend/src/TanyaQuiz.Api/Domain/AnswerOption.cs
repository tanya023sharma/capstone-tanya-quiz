namespace TanyaQuiz.Api.Domain;

public sealed class AnswerOption
{
    public OptionLabel Label { get; init; }

    public string Text { get; init; } = string.Empty;
}
