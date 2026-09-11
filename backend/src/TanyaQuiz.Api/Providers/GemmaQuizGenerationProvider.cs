using System.Net.Http.Json;
using System.Text.Json;
using TanyaQuiz.Api.Configuration;

namespace TanyaQuiz.Api.Providers;

public sealed class GemmaQuizGenerationProvider(
    HttpClient httpClient,
    QuizAiOptions options,
    QuizPromptBuilder promptBuilder,
    ILogger<GemmaQuizGenerationProvider> logger) : IQuizGenerationProvider
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<GeneratedQuizCandidate> GenerateAsync(
        QuizGenerationInput input,
        CancellationToken cancellationToken = default)
    {
        var prompt = promptBuilder.Build(input);
        Exception? lastError = null;

        for (var attempt = 0; attempt <= options.MaxRetries; attempt++)
        {
            try
            {
                var payload = new
                {
                    model = options.Model,
                    input = prompt,
                    response_format = new
                    {
                        type = "json_object"
                    }
                };

                using var response = await httpClient.PostAsJsonAsync(string.Empty, payload, JsonOptions, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new QuizProviderException($"The configured AI provider returned {(int)response.StatusCode}.");
                }

                var raw = await response.Content.ReadAsStringAsync(cancellationToken);
                var json = ExtractModelJson(raw);
                var candidate = JsonSerializer.Deserialize<GeneratedQuizCandidate>(json, JsonOptions);
                if (candidate is null)
                {
                    throw new QuizProviderException("The AI provider returned an empty quiz.");
                }

                return candidate;
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                lastError = new QuizProviderException("The AI provider timed out.", 504);
            }
            catch (HttpRequestException exception)
            {
                lastError = new QuizProviderException("The AI provider could not be reached.", 502, exception);
            }
            catch (JsonException exception)
            {
                lastError = new QuizProviderException("The AI provider returned invalid JSON.", 502, exception);
            }
            catch (QuizProviderException exception)
            {
                lastError = exception;
            }

            logger.LogWarning("AI generation attempt {Attempt} failed for model {Model}.", attempt + 1, options.Model);
            if (attempt < options.MaxRetries)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(150), cancellationToken);
            }
        }

        throw lastError ?? new QuizProviderException("The AI provider failed to generate a quiz.");
    }

    private static string ExtractModelJson(string raw)
    {
        using var document = JsonDocument.Parse(raw);
        var root = document.RootElement;
        if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("output_text", out var outputText))
        {
            return outputText.GetString() ?? raw;
        }

        if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("text", out var text))
        {
            return text.GetString() ?? raw;
        }

        return raw;
    }
}
