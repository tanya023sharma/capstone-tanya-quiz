using TanyaQuiz.Api.Domain;

namespace TanyaQuiz.Api.Providers;

public sealed class QuizPromptBuilder
{
    public string Build(QuizGenerationInput input) => $"""
        Generate exactly 10 source-grounded multiple-choice questions.
        Difficulty: {input.Difficulty}.
        Every question must contain four options labeled A, B, C, and D, one correct option,
        explanations for all four options, and a verbatim evidence excerpt from the source.
        Return JSON with a questions array only. Never use facts outside the source text.

        Source text:
        {input.SourceText}
        """;
}
