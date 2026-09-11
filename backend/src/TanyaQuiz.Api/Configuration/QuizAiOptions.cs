namespace TanyaQuiz.Api.Configuration;

public sealed class QuizAiOptions
{
    public string Provider { get; init; } = "fake";

    public string Endpoint { get; init; } = string.Empty;

    public string ApiKey { get; init; } = string.Empty;

    public string Model { get; init; } = "gemma-4-26b-a4b";

    public int TimeoutSeconds { get; init; } = 4;

    public int MaxRetries { get; init; } = 1;
}
