using TanyaQuiz.Api.Contracts;
using TanyaQuiz.Api.Domain;

namespace TanyaQuiz.Api.Services;

public static class QuizMapper
{
    public static QuizResponse ToResponse(Quiz quiz) => new()
    {
        Id = quiz.Id,
        TopicContext = quiz.TopicContext,
        Difficulty = quiz.Difficulty,
        ExpiresAt = quiz.ExpiresAt,
        Questions = quiz.Questions
            .OrderBy(question => question.Number)
            .Select(question => new PublicQuestionResponse
            {
                Id = question.Id,
                Number = question.Number,
                Prompt = question.Prompt,
                Options = question.Options
                    .OrderBy(option => option.Label)
                    .Select(option => new PublicOptionResponse
                    {
                        Label = option.Label,
                        Text = option.Text
                    })
                    .ToList()
            })
            .ToList()
    };

    public static QuizResultsResponse ToResults(
        Quiz quiz,
        IReadOnlyDictionary<Guid, OptionLabel> answers,
        int score) => new()
    {
        QuizId = quiz.Id,
        Score = score,
        Results = quiz.Questions
            .OrderBy(question => question.Number)
            .Select(question =>
            {
                var selectedOption = answers[question.Id];
                return new QuestionResultResponse
                {
                    QuestionId = question.Id,
                    Number = question.Number,
                    Prompt = question.Prompt,
                    SelectedOption = selectedOption,
                    CorrectOption = question.CorrectOption,
                    IsCorrect = selectedOption == question.CorrectOption,
                    Options = question.Options
                        .OrderBy(option => option.Label)
                        .Select(option => new ResultOptionResponse
                        {
                            Label = option.Label,
                            Text = option.Text,
                            IsCorrect = option.Label == question.CorrectOption,
                            Explanation = question.Explanations[option.Label]
                        })
                        .ToList(),
                    Evidence = question.Evidence
                        .Select(evidence => new EvidenceExcerptResponse
                        {
                            Quote = evidence.Quote,
                            SourceStart = evidence.SourceStart,
                            SourceEnd = evidence.SourceEnd
                        })
                        .ToList()
                };
            })
            .ToList()
    };
}
