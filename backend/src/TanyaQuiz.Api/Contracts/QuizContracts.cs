using TanyaQuiz.Api.Domain;

namespace TanyaQuiz.Api.Contracts;

public sealed class GenerateQuizRequest
{
    public string? TopicContext { get; init; }

    public string SourceText { get; init; } = string.Empty;

    public Difficulty Difficulty { get; init; }
}

public sealed class QuizResponse
{
    public Guid Id { get; init; }

    public string? TopicContext { get; init; }

    public Difficulty Difficulty { get; init; }

    public IReadOnlyList<PublicQuestionResponse> Questions { get; init; } = [];

    public DateTimeOffset ExpiresAt { get; init; }
}

public sealed class PublicQuestionResponse
{
    public Guid Id { get; init; }

    public int Number { get; init; }

    public string Prompt { get; init; } = string.Empty;

    public IReadOnlyList<PublicOptionResponse> Options { get; init; } = [];
}

public sealed class PublicOptionResponse
{
    public OptionLabel Label { get; init; }

    public string Text { get; init; } = string.Empty;
}

public sealed class SubmitQuizRequest
{
    public IReadOnlyList<SubmittedAnswer> Answers { get; init; } = [];
}

public sealed class SubmittedAnswer
{
    public Guid QuestionId { get; init; }

    public OptionLabel SelectedOption { get; init; }
}

public sealed class QuizResultsResponse
{
    public Guid QuizId { get; init; }

    public int Score { get; init; }

    public int TotalQuestions { get; init; } = 10;

    public IReadOnlyList<QuestionResultResponse> Results { get; init; } = [];
}

public sealed class QuestionResultResponse
{
    public Guid QuestionId { get; init; }

    public int Number { get; init; }

    public string Prompt { get; init; } = string.Empty;

    public OptionLabel SelectedOption { get; init; }

    public OptionLabel CorrectOption { get; init; }

    public bool IsCorrect { get; init; }

    public IReadOnlyList<ResultOptionResponse> Options { get; init; } = [];

    public IReadOnlyList<EvidenceExcerptResponse> Evidence { get; init; } = [];
}

public sealed class ResultOptionResponse
{
    public OptionLabel Label { get; init; }

    public string Text { get; init; } = string.Empty;

    public bool IsCorrect { get; init; }

    public string Explanation { get; init; } = string.Empty;
}

public sealed class EvidenceExcerptResponse
{
    public string Quote { get; init; } = string.Empty;

    public int SourceStart { get; init; }

    public int SourceEnd { get; init; }
}

public sealed class ErrorResponse
{
    public string Code { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;

    public string TraceId { get; init; } = string.Empty;

    public IReadOnlyList<FieldError> FieldErrors { get; init; } = [];
}

public sealed class FieldError
{
    public string Field { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;
}
