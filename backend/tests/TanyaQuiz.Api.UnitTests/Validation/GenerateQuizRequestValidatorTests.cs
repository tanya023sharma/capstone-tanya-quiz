using TanyaQuiz.Api.Contracts;
using TanyaQuiz.Api.Domain;
using TanyaQuiz.Api.Validation;

namespace TanyaQuiz.Api.UnitTests.Validation;

public sealed class GenerateQuizRequestValidatorTests
{
    private readonly GenerateQuizRequestValidator validator = new();

    [Fact]
    public void Accepts_valid_source_and_difficulty()
    {
        var request = new GenerateQuizRequest
        {
            SourceText = new string('a', 100),
            Difficulty = Difficulty.Hard
        };

        Assert.Empty(validator.Validate(request));
    }

    [Fact]
    public void Rejects_source_with_fewer_than_one_hundred_non_whitespace_characters()
    {
        var request = new GenerateQuizRequest
        {
            SourceText = "a b c",
            Difficulty = Difficulty.Easy
        };

        var errors = validator.Validate(request);

        Assert.Contains(errors, error => error.Field == "sourceText");
    }

    [Fact]
    public void Rejects_invalid_difficulty_value()
    {
        var request = new GenerateQuizRequest
        {
            SourceText = new string('a', 100),
            Difficulty = (Difficulty)99
        };

        var errors = validator.Validate(request);

        Assert.Contains(errors, error => error.Field == "difficulty");
    }
}
