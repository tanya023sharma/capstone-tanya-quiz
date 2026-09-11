using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using TanyaQuiz.Api.Contracts;
using TanyaQuiz.Api.Domain;

namespace TanyaQuiz.Api.IntegrationTests;

public sealed class QuizEndpointIntegrationTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient client = factory.CreateClient();
    private readonly JsonSerializerOptions jsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    [Fact]
    public async Task Create_quiz_returns_public_ten_question_contract()
    {
        var response = await client.PostAsJsonAsync("/api/v1/quizzes", new GenerateQuizRequest
        {
            TopicContext = "Water cycle",
            SourceText = new string('a', 120),
            Difficulty = Difficulty.Easy
        }, jsonOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var quiz = await response.Content.ReadFromJsonAsync<QuizResponse>(jsonOptions);
        Assert.NotNull(quiz);
        Assert.Equal(10, quiz.Questions.Count);
        Assert.All(quiz.Questions, question => Assert.Equal(4, question.Options.Count));
    }

    [Fact]
    public async Task Submit_and_regenerate_use_the_public_http_contract()
    {
        var createResponse = await client.PostAsJsonAsync("/api/v1/quizzes", new GenerateQuizRequest
        {
            SourceText = new string('b', 120),
            Difficulty = Difficulty.Medium
        }, jsonOptions);
        var quiz = await createResponse.Content.ReadFromJsonAsync<QuizResponse>(jsonOptions);
        Assert.NotNull(quiz);

        var submitResponse = await client.PostAsJsonAsync(
            $"/api/v1/quizzes/{quiz.Id}/submissions",
            new SubmitQuizRequest
            {
                Answers = quiz.Questions.Select(question => new SubmittedAnswer
                {
                    QuestionId = question.Id,
                    SelectedOption = OptionLabel.A
                }).ToList()
            }, jsonOptions);

        Assert.Equal(HttpStatusCode.OK, submitResponse.StatusCode);
        var results = await submitResponse.Content.ReadFromJsonAsync<QuizResultsResponse>(jsonOptions);
        Assert.NotNull(results);
        Assert.Equal(10, results.Results.Count);

        var regenerationResponse = await client.PostAsync($"/api/v1/quizzes/{quiz.Id}/regenerations", null);
        Assert.Equal(HttpStatusCode.Created, regenerationResponse.StatusCode);
    }
}
