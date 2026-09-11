using TanyaQuiz.Api.Providers;
using TanyaQuiz.Api.Validation;

namespace TanyaQuiz.Api.UnitTests.Validation;

public sealed class GeneratedQuizValidatorTests
{
    [Fact]
    public async Task Accepts_fake_provider_output_with_ten_grounded_questions()
    {
        var source = new string('a', 120);
        var provider = new FakeQuizGenerationProvider();
        var candidate = await provider.GenerateAsync(new QuizGenerationInput(
            null,
            source,
            Domain.Difficulty.Easy));

        var errors = new GeneratedQuizValidator().Validate(candidate, source);

        Assert.Empty(errors);
    }

    [Fact]
    public void Rejects_candidate_with_wrong_question_count()
    {
        var candidate = new GeneratedQuizCandidate();

        var errors = new GeneratedQuizValidator().Validate(candidate, new string('a', 120));

        Assert.Contains(errors, error => error.Contains("exactly 10"));
    }
}
