using TanyaQuiz.Api.Contracts;
using TanyaQuiz.Api.Services;

namespace TanyaQuiz.Api.Infrastructure;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (QuizServiceException exception)
        {
            await WriteErrorAsync(context, exception.StatusCode, exception.Code, exception.Message, exception.FieldErrors);
        }
        catch (OperationCanceledException) when (!context.RequestAborted.IsCancellationRequested)
        {
            await WriteErrorAsync(context, 504, "QUIZ_GENERATION_TIMED_OUT", "Quiz generation took too long. Try again with the same text.");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled API error.");
            await WriteErrorAsync(context, 500, "INTERNAL_ERROR", "Something went wrong. Try again shortly.");
        }
    }

    private static async Task WriteErrorAsync(
        HttpContext context,
        int statusCode,
        string code,
        string message,
        IReadOnlyList<FieldError>? fieldErrors = null)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new ErrorResponse
        {
            Code = code,
            Message = message,
            TraceId = context.TraceIdentifier,
            FieldErrors = fieldErrors ?? []
        });
    }
}
