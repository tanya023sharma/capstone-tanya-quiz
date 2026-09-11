using Microsoft.Extensions.Caching.Memory;
using TanyaQuiz.Api.Domain;

namespace TanyaQuiz.Api.Services;

public sealed class InMemoryQuizStore(IMemoryCache cache) : IQuizStore
{
    private readonly HashSet<Guid> _expiredQuizIds = [];
    private readonly Lock _lock = new();

    public QuizLookupResult Get(Guid quizId)
    {
        if (cache.TryGetValue(quizId, out Quiz? quiz) && quiz is not null)
        {
            return new QuizLookupResult(quiz, false);
        }

        lock (_lock)
        {
            return new QuizLookupResult(null, _expiredQuizIds.Contains(quizId));
        }
    }

    public void Save(Quiz quiz)
    {
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = quiz.ExpiresAt
        };
        cacheOptions.RegisterPostEvictionCallback((key, _, reason, _) =>
        {
            if (key is Guid quizId && reason == EvictionReason.Expired)
            {
                lock (_lock)
                {
                    _expiredQuizIds.Add(quizId);
                }
            }
        });

        cache.Set(quiz.Id, quiz, cacheOptions);
    }
}
