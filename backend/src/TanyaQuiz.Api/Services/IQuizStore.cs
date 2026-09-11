using TanyaQuiz.Api.Domain;

namespace TanyaQuiz.Api.Services;

public sealed record QuizLookupResult(Quiz? Quiz, bool Expired);

public interface IQuizStore
{
    QuizLookupResult Get(Guid quizId);

    void Save(Quiz quiz);
}
