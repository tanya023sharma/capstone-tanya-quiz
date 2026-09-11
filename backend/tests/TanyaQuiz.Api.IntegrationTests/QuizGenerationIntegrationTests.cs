using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using TanyaQuiz.Api.Configuration;
using TanyaQuiz.Api.Contracts;
using TanyaQuiz.Api.Diagnostics;
using TanyaQuiz.Api.Domain;
using TanyaQuiz.Api.Providers;
using TanyaQuiz.Api.Services;
using TanyaQuiz.Api.Validation;

namespace TanyaQuiz.Api.IntegrationTests;

public sealed class QuizGenerationIntegrationTests
{
    [Fact]
    public async Task Fake_provider_to_generation_service_returns_a_complete_public_quiz()
    {
        var store = new InMemoryQuizStore(new MemoryCache(new MemoryCacheOptions()));
        var service = new QuizGenerationService(
            new FakeQuizGenerationProvider(),
            store,
            new GenerateQuizRequestValidator(),
            new GeneratedQuizValidator(),
            new QuizAiOptions(),
            new QuizMetrics(),
            NullLogger<QuizGenerationService>.Instance);

        var response = await service.GenerateAsync(new GenerateQuizRequest
        {
            TopicContext = "Integration fixture",
            SourceText = new string('a', 120),
            Difficulty = Difficulty.Medium
        });

        Assert.Equal(10, response.Questions.Count);
        Assert.All(response.Questions, question => Assert.Equal(4, question.Options.Count));
        Assert.DoesNotContain("CorrectOption", response.GetType().GetProperties().Select(property => property.Name));
    }
}
