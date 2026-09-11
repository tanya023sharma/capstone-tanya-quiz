using TanyaQuiz.Api.Domain;

namespace TanyaQuiz.Api.Providers;

public sealed class FakeQuizGenerationProvider : IQuizGenerationProvider
{
    public Task<GeneratedQuizCandidate> GenerateAsync(
        QuizGenerationInput input,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var source = Normalize(input.SourceText);
        var quoteLength = Math.Min(90, source.Length);
        var quote = source[..quoteLength];
        var sourceStart = source.IndexOf(quote, StringComparison.Ordinal);
        var difficulty = input.Difficulty.ToString().ToLowerInvariant();

        var questions = Enumerable.Range(1, 10)
            .Select(number => new GeneratedQuestionCandidate
            {
                Number = number,
                Prompt = $"Which statement is directly supported by the {difficulty} source material (question {number})?",
                Options =
                [
                    new GeneratedOptionCandidate { Label = "A", Text = quote },
                    new GeneratedOptionCandidate { Label = "B", Text = $"The source states the opposite of item {number}." },
                    new GeneratedOptionCandidate { Label = "C", Text = $"The source gives no information about item {number}." },
                    new GeneratedOptionCandidate { Label = "D", Text = $"An unrelated detail is the main point of item {number}." }
                ],
                CorrectOption = "A",
                Explanations = new Dictionary<string, string>
                {
                    ["A"] = $"This option is supported by the source excerpt: {quote}",
                    ["B"] = "The source excerpt does not state the opposite.",
                    ["C"] = "The source does contain information represented by the excerpt.",
                    ["D"] = "The option introduces a detail that is not supported by the source."
                },
                Evidence =
                [
                    new GeneratedEvidenceCandidate
                    {
                        Quote = quote,
                        SourceStart = sourceStart,
                        SourceEnd = sourceStart + quote.Length
                    }
                ]
            })
            .ToList();

        return Task.FromResult(new GeneratedQuizCandidate { Questions = questions });
    }

    private static string Normalize(string sourceText) =>
        string.Join(' ', sourceText.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
}
