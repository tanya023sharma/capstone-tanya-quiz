using TanyaQuiz.Api.Domain;
using TanyaQuiz.Api.Providers;

namespace TanyaQuiz.Api.Validation;

public sealed class GeneratedQuizValidator
{
    public IReadOnlyList<string> Validate(GeneratedQuizCandidate candidate, string sourceText)
    {
        var errors = new List<string>();
        if (candidate.Questions.Count != 10)
        {
            errors.Add("The generated quiz must contain exactly 10 questions.");
            return errors;
        }

        var normalizedSource = Normalize(sourceText);
        var questionNumbers = candidate.Questions.Select(question => question.Number).ToArray();
        if (!questionNumbers.SequenceEqual(Enumerable.Range(1, 10)))
        {
            errors.Add("Questions must be numbered from 1 through 10.");
        }

        if (candidate.Questions.Select(question => question.Prompt).Distinct(StringComparer.OrdinalIgnoreCase).Count() != 10)
        {
            errors.Add("Question prompts must be unique.");
        }

        foreach (var question in candidate.Questions)
        {
            var labels = question.Options.Select(option => option.Label).ToArray();
            if (labels.Length != 4 || labels.Distinct(StringComparer.Ordinal).Count() != 4 ||
                !new[] { "A", "B", "C", "D" }.All(labels.Contains))
            {
                errors.Add($"Question {question.Number} must contain unique A-D options.");
            }

            if (question.Options.Select(option => option.Text).Distinct(StringComparer.OrdinalIgnoreCase).Count() != question.Options.Count)
            {
                errors.Add($"Question {question.Number} contains duplicate option text.");
            }

            if (!Enum.TryParse<OptionLabel>(question.CorrectOption, out _))
            {
                errors.Add($"Question {question.Number} has an invalid correct option.");
            }

            foreach (var label in new[] { "A", "B", "C", "D" })
            {
                if (!question.Explanations.TryGetValue(label, out var explanation) || string.IsNullOrWhiteSpace(explanation))
                {
                    errors.Add($"Question {question.Number} is missing an explanation for option {label}.");
                }
            }

            if (question.Evidence.Count == 0 || question.Evidence.Any(evidence =>
                    string.IsNullOrWhiteSpace(evidence.Quote) || !normalizedSource.Contains(Normalize(evidence.Quote), StringComparison.Ordinal)))
            {
                errors.Add($"Question {question.Number} contains evidence that is not present in the source.");
            }
        }

        return errors;
    }

    private static string Normalize(string value) =>
        string.Join(' ', value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
}
