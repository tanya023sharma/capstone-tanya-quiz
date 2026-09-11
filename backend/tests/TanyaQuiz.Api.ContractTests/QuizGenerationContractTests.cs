using System.Text.Json;
using TanyaQuiz.Api.Contracts;
using TanyaQuiz.Api.Domain;

namespace TanyaQuiz.Api.ContractTests;

public sealed class QuizGenerationContractTests
{
    [Fact]
    public void Public_quiz_serialization_does_not_expose_answer_keys()
    {
        var response = new QuizResponse
        {
            Id = Guid.NewGuid(),
            Difficulty = Difficulty.Easy,
            Questions =
            [
                new PublicQuestionResponse
                {
                    Id = Guid.NewGuid(),
                    Number = 1,
                    Prompt = "Which option is supported?",
                    Options =
                    [
                        new PublicOptionResponse { Label = OptionLabel.A, Text = "A" },
                        new PublicOptionResponse { Label = OptionLabel.B, Text = "B" },
                        new PublicOptionResponse { Label = OptionLabel.C, Text = "C" },
                        new PublicOptionResponse { Label = OptionLabel.D, Text = "D" }
                    ]
                }
            ]
        };

        var json = JsonSerializer.Serialize(response);

        Assert.DoesNotContain("correctOption", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("explanation", json, StringComparison.OrdinalIgnoreCase);
    }
}
