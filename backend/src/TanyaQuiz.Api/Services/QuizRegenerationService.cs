using TanyaQuiz.Api.Contracts;

namespace TanyaQuiz.Api.Services;

public sealed class QuizRegenerationService(
    IQuizStore quizStore,
    QuizGenerationService generationService)
{
    public async Task<QuizResponse> RegenerateAsync(
        Guid quizId,
        CancellationToken cancellationToken = default)
    {
        var lookup = quizStore.Get(quizId);
        if (lookup.Expired)
        {
            throw new QuizServiceException(410, "QUIZ_EXPIRED", "This quiz has expired. Generate a new quiz to continue.");
        }

        if (lookup.Quiz is null)
        {
            throw new QuizServiceException(404, "QUIZ_NOT_FOUND", "The requested quiz could not be found.");
        }

        var replacement = await generationService.GenerateAsync(new GenerateQuizRequest
        {
            TopicContext = lookup.Quiz.TopicContext,
            SourceText = lookup.Quiz.SourceText,
            Difficulty = lookup.Quiz.Difficulty
        }, cancellationToken);

        lookup.Quiz.Status = Domain.QuizStatus.Superseded;
        return replacement;
    }
}
