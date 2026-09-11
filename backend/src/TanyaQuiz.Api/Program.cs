using System.Text.Json.Serialization;
using TanyaQuiz.Api.Configuration;
using TanyaQuiz.Api.Diagnostics;
using TanyaQuiz.Api.Infrastructure;
using TanyaQuiz.Api.Providers;
using TanyaQuiz.Api.Services;
using TanyaQuiz.Api.Validation;

var builder = WebApplication.CreateBuilder(args);

var aiOptions = new QuizAiOptions
{
    Provider = builder.Configuration["QuizAi:Provider"] ?? "fake",
    Endpoint = builder.Configuration["QuizAi:Endpoint"] ?? string.Empty,
    ApiKey = builder.Configuration["QuizAi:ApiKey"] ?? string.Empty,
    Model = builder.Configuration["QuizAi:Model"] ?? "gemma-4-26b-a4b",
    TimeoutSeconds = int.TryParse(builder.Configuration["QuizAi:TimeoutSeconds"], out var timeout)
        ? timeout
        : 4,
    MaxRetries = int.TryParse(builder.Configuration["QuizAi:MaxRetries"], out var retries)
        ? retries
        : 1
};

if (!string.Equals(aiOptions.Provider, "fake", StringComparison.OrdinalIgnoreCase) &&
    (!Uri.TryCreate(aiOptions.Endpoint, UriKind.Absolute, out _) || string.IsNullOrWhiteSpace(aiOptions.ApiKey)))
{
    throw new InvalidOperationException("QuizAi:Endpoint and QuizAi:ApiKey are required for a live AI provider.");
}

builder.Services.AddSingleton(aiOptions);
builder.Services.AddSingleton<QuizMetrics>();
builder.Services.AddSingleton<QuizPromptBuilder>();
builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();
builder.Services.AddCors(options => options.AddPolicy("frontend", policy =>
    policy.WithOrigins("http://localhost:4200", "http://127.0.0.1:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()));

builder.Services.AddHttpClient<GemmaQuizGenerationProvider>((_, client) =>
{
    if (Uri.TryCreate(aiOptions.Endpoint, UriKind.Absolute, out var endpoint))
    {
        client.BaseAddress = endpoint;
    }

    client.Timeout = TimeSpan.FromSeconds(Math.Max(1, aiOptions.TimeoutSeconds));
    if (!string.IsNullOrWhiteSpace(aiOptions.ApiKey))
    {
        client.DefaultRequestHeaders.Add("x-goog-api-key", aiOptions.ApiKey);
    }
});

if (string.Equals(aiOptions.Provider, "fake", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddSingleton<IQuizGenerationProvider, FakeQuizGenerationProvider>();
}
else
{
    builder.Services.AddTransient<IQuizGenerationProvider>(serviceProvider =>
        serviceProvider.GetRequiredService<GemmaQuizGenerationProvider>());
}

builder.Services.AddSingleton<IQuizStore, InMemoryQuizStore>();
builder.Services.AddSingleton<GenerateQuizRequestValidator>();
builder.Services.AddSingleton<GeneratedQuizValidator>();
builder.Services.AddSingleton<SubmitQuizRequestValidator>();
builder.Services.AddScoped<QuizGenerationService>();
builder.Services.AddScoped<QuizSubmissionService>();
builder.Services.AddScoped<QuizRegenerationService>();

var app = builder.Build();

app.UseMiddleware<TraceIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("frontend");
app.MapOpenApi();
app.MapControllers();

app.Run();

public partial class Program;
