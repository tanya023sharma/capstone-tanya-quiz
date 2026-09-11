using System.Diagnostics.Metrics;

namespace TanyaQuiz.Api.Diagnostics;

public sealed class QuizMetrics : IDisposable
{
    private readonly Meter meter = new("TanyaQuiz.Api");
    private readonly Counter<long> generationRequests;
    private readonly Counter<long> generationFailures;
    private readonly Histogram<double> generationDuration;

    public QuizMetrics()
    {
        generationRequests = meter.CreateCounter<long>("quiz.generation.requests");
        generationFailures = meter.CreateCounter<long>("quiz.generation.failures");
        generationDuration = meter.CreateHistogram<double>("quiz.generation.duration_ms");
    }

    public void GenerationStarted() => generationRequests.Add(1);

    public void GenerationFailed(string code) => generationFailures.Add(1, new KeyValuePair<string, object?>("code", code));

    public void GenerationCompleted(double durationMilliseconds) => generationDuration.Record(durationMilliseconds);

    public void Dispose() => meter.Dispose();
}
