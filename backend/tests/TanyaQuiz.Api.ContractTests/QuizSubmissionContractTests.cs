using TanyaQuiz.Api.Contracts;
using TanyaQuiz.Api.Domain;
using TanyaQuiz.Api.Validation;

namespace TanyaQuiz.Api.ContractTests;

public sealed class QuizSubmissionContractTests
{
    [Fact]
    public void Submission_contract_requires_all_ten_answers()
    {
        var errors = new SubmitQuizRequestValidator().Validate(new SubmitQuizRequest());

        Assert.Contains(errors, error => error.Field == "answers");
    }

    [Fact]
    public void Submission_contract_accepts_option_labels_a_through_d()
    {
        var request = new SubmitQuizRequest
        {
            Answers = Enumerable.Range(0, 10).Select(_ => new SubmittedAnswer
            {
                QuestionId = Guid.NewGuid(),
                SelectedOption = OptionLabel.A
            }).ToList()
        };

        Assert.DoesNotContain(new SubmitQuizRequestValidator().Validate(request), error => error.Field == "answers");
    }
}
