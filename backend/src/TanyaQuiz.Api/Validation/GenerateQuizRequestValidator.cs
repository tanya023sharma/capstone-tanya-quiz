using TanyaQuiz.Api.Contracts;

namespace TanyaQuiz.Api.Validation;

public sealed class GenerateQuizRequestValidator
{
    public IReadOnlyList<FieldError> Validate(GenerateQuizRequest request)
    {
        var errors = new List<FieldError>();
        var nonWhitespaceLength = request.SourceText.Count(character => !char.IsWhiteSpace(character));

        if (nonWhitespaceLength < 100)
        {
            errors.Add(new FieldError
            {
                Field = "sourceText",
                Message = "Provide at least 100 non-whitespace characters."
            });
        }

        if (nonWhitespaceLength > 20000)
        {
            errors.Add(new FieldError
            {
                Field = "sourceText",
                Message = "Source text must not exceed 20,000 non-whitespace characters."
            });
        }

        if (request.TopicContext?.Length > 160)
        {
            errors.Add(new FieldError
            {
                Field = "topicContext",
                Message = "Topic context must not exceed 160 characters."
            });
        }

        if (!Enum.IsDefined(request.Difficulty))
        {
            errors.Add(new FieldError
            {
                Field = "difficulty",
                Message = "Choose Easy, Medium, or Hard."
            });
        }

        return errors;
    }
}
