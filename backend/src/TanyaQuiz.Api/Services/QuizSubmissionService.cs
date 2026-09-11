using TanyaQuiz.Api.Contracts;
using TanyaQuiz.Api.Domain;
using TanyaQuiz.Api.Validation;

namespace TanyaQuiz.Api.Services;

public sealed class QuizSubmissionService(
    IQuizStore quizStore,
    SubmitQuizRequestValidator requestValidator)
{
    public Task<QuizResultsResponse> SubmitAsync(
        Guid quizId,
        SubmitQuizRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var requestErrors = requestValidator.Validate(request);
        if (requestErrors.Count > 0)
        {
            throw new QuizServiceException(
                400,
                "VALIDATION_ERROR",
                "Answer all questions before submitting.",
                requestErrors);
        }

        var lookup = quizStore.Get(quizId);
        if (lookup.Expired)
        {
            throw new QuizServiceException(410, "QUIZ_EXPIRED", "This quiz has expired. Generate a new quiz to continue.");
        }

        if (lookup.Quiz is null)
        {
            throw new QuizServiceException(404, "QUIZ_NOT_FOUND", "The requested quiz could not be found.");
        }

        if (lookup.Quiz.Status == QuizStatus.Submitted)
        {
            throw new QuizServiceException(400, "QUIZ_ALREADY_SUBMITTED", "This quiz has already been submitted.");
        }

        if (lookup.Quiz.Status == QuizStatus.Superseded)
        {
            throw new QuizServiceException(410, "QUIZ_EXPIRED", "This quiz is no longer active. Generate a new quiz to continue.");
        }

        var answerMap = request.Answers.ToDictionary(answer => answer.QuestionId, answer => answer.SelectedOption);
        var questions = lookup.Quiz.Questions.ToDictionary(question => question.Id);
        if (questions.Keys.Except(answerMap.Keys).Any() || answerMap.Keys.Except(questions.Keys).Any())
        {
            throw new QuizServiceException(400, "INCOMPLETE_SUBMISSION", "Submit exactly one answer for every question.");
        }

        var score = questions.Values.Count(question => answerMap[question.Id] == question.CorrectOption);
        lookup.Quiz.Status = QuizStatus.Submitted;
        return Task.FromResult(QuizMapper.ToResults(lookup.Quiz, answerMap, score));
    }
}
