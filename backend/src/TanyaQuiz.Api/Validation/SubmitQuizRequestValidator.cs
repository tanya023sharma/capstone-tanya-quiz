using TanyaQuiz.Api.Contracts;
using TanyaQuiz.Api.Domain;

namespace TanyaQuiz.Api.Validation;

public sealed class SubmitQuizRequestValidator
{
    public IReadOnlyList<FieldError> Validate(SubmitQuizRequest request)
    {
        var errors = new List<FieldError>();
        if (request.Answers.Count != 10)
        {
            errors.Add(new FieldError
            {
                Field = "answers",
                Message = "Answer all 10 questions before submitting."
            });
        }

        if (request.Answers.Select(answer => answer.QuestionId).Distinct().Count() != request.Answers.Count)
        {
            errors.Add(new FieldError
            {
                Field = "answers",
                Message = "Each question may be answered only once."
            });
        }

        if (request.Answers.Any(answer => !Enum.IsDefined(answer.SelectedOption)))
        {
            errors.Add(new FieldError
            {
                Field = "answers",
                Message = "Every answer must use option A, B, C, or D."
            });
        }

        return errors;
    }
}
