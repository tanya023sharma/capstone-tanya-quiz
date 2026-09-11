using TanyaQuiz.Api.Contracts;
using TanyaQuiz.Api.Domain;

namespace TanyaQuiz.Api.ContractTests;

public sealed class QuizRegenerationContractTests
{
    [Fact]
    public void Regeneration_response_keeps_the_public_quiz_shape()
    {
        var response = new QuizResponse
        {
            Id = Guid.NewGuid(),
            Difficulty = Difficulty.Hard,
            Questions = Enumerable.Range(1, 10).Select(number => new PublicQuestionResponse
            {
                Id = Guid.NewGuid(),
                Number = number,
                Prompt = $"Question {number}",
                Options = Enum.GetValues<OptionLabel>().Select(label => new PublicOptionResponse
                {
                    Label = label,
                    Text = label.ToString()
                }).ToList()
            }).ToList()
        };

        Assert.Equal(10, response.Questions.Count);
        Assert.All(response.Questions, question => Assert.Equal(4, question.Options.Count));
    }
}
