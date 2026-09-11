using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using TanyaQuiz.Api.Configuration;
using TanyaQuiz.Api.Contracts;
using TanyaQuiz.Api.Diagnostics;
using TanyaQuiz.Api.Domain;
using TanyaQuiz.Api.Providers;
using TanyaQuiz.Api.Services;
using TanyaQuiz.Api.Validation;

namespace TanyaQuiz.Api.UnitTests.Services;

public sealed class QuizServiceTests
{
    [Fact]
    public async Task Generation_redacts_answer_key_and_returns_ten_questions()
    {
        var store = CreateStore();
        var service = CreateGenerationService(store);
        var response = await service.GenerateAsync(new GenerateQuizRequest
        {
            SourceText = new string('a', 120),
            Difficulty = Difficulty.Medium
        });

        Assert.Equal(10, response.Questions.Count);
        Assert.All(response.Questions, question => Assert.Equal(4, question.Options.Count));
        Assert.DoesNotContain("CorrectOption", typeof(QuizResponse).GetProperties().Select(property => property.Name));
    }

    [Fact]
    public async Task Submission_scores_from_server_owned_answer_keys()
    {
        var store = CreateStore();
        var quiz = CreateQuiz();
        store.Save(quiz);
        var service = new QuizSubmissionService(store, new SubmitQuizRequestValidator());

        var results = await service.SubmitAsync(quiz.Id, new SubmitQuizRequest
        {
            Answers = quiz.Questions.Select(question => new SubmittedAnswer
            {
                QuestionId = question.Id,
                SelectedOption = OptionLabel.A
            }).ToList()
        });

        Assert.Equal(10, results.Score);
        Assert.All(results.Results, result => Assert.True(result.IsCorrect));
        Assert.All(results.Results, result => Assert.Equal(4, result.Options.Count));
    }

    [Fact]
    public async Task Submission_rejects_missing_answers()
    {
        var store = CreateStore();
        var quiz = CreateQuiz();
        store.Save(quiz);
        var service = new QuizSubmissionService(store, new SubmitQuizRequestValidator());

        await Assert.ThrowsAsync<QuizServiceException>(() => service.SubmitAsync(
            quiz.Id,
            new SubmitQuizRequest { Answers = [] }));
    }

    private static QuizGenerationService CreateGenerationService(IQuizStore store) =>
        new(
            new FakeQuizGenerationProvider(),
            store,
            new GenerateQuizRequestValidator(),
            new GeneratedQuizValidator(),
            new QuizAiOptions(),
            new QuizMetrics(),
            NullLogger<QuizGenerationService>.Instance);

    private static IQuizStore CreateStore() => new InMemoryQuizStore(new MemoryCache(new MemoryCacheOptions()));

    private static Quiz CreateQuiz()
    {
        var now = DateTimeOffset.UtcNow;
        return new Quiz
        {
            SourceText = new string('a', 120),
            Difficulty = Difficulty.Easy,
            ExpiresAt = now.AddMinutes(60),
            ProviderModel = "fake",
            Questions = Enumerable.Range(1, 10).Select(number => new Question
            {
                Number = number,
                Prompt = $"Question {number}",
                CorrectOption = OptionLabel.A,
                Options =
                [
                    new AnswerOption { Label = OptionLabel.A, Text = "Correct" },
                    new AnswerOption { Label = OptionLabel.B, Text = "Wrong B" },
                    new AnswerOption { Label = OptionLabel.C, Text = "Wrong C" },
                    new AnswerOption { Label = OptionLabel.D, Text = "Wrong D" }
                ],
                Explanations = new Dictionary<OptionLabel, string>
                {
                    [OptionLabel.A] = "Correct explanation",
                    [OptionLabel.B] = "Wrong explanation",
                    [OptionLabel.C] = "Wrong explanation",
                    [OptionLabel.D] = "Wrong explanation"
                },
                Evidence =
                [
                    new EvidenceExcerpt
                    {
                        Quote = "a",
                        SourceStart = 0,
                        SourceEnd = 1
                    }
                ]
            }).ToList()
        };
    }
}
