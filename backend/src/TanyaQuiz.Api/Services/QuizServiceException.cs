using TanyaQuiz.Api.Contracts;

namespace TanyaQuiz.Api.Services;

public sealed class QuizServiceException : Exception
{
    public QuizServiceException(
        int statusCode,
        string code,
        string message,
        IReadOnlyList<FieldError>? fieldErrors = null)
        : base(message)
    {
        StatusCode = statusCode;
        Code = code;
        FieldErrors = fieldErrors ?? [];
    }

    public int StatusCode { get; }

    public string Code { get; }

    public IReadOnlyList<FieldError> FieldErrors { get; }
}
