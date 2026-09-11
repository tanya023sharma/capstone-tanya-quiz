using Microsoft.AspNetCore.Mvc;
using TanyaQuiz.Api.Contracts;
using TanyaQuiz.Api.Services;

namespace TanyaQuiz.Api.Controllers;

[ApiController]
[Route("api/v1/quizzes")]
public sealed class QuizzesController(
    QuizGenerationService generationService,
    QuizSubmissionService submissionService,
    QuizRegenerationService regenerationService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(QuizResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<QuizResponse>> CreateQuiz(
        GenerateQuizRequest request,
        CancellationToken cancellationToken)
    {
        var response = await generationService.GenerateAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPost("{quizId:guid}/submissions")]
    [ProducesResponseType(typeof(QuizResultsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<QuizResultsResponse>> SubmitQuiz(
        Guid quizId,
        SubmitQuizRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await submissionService.SubmitAsync(quizId, request, cancellationToken));
    }

    [HttpPost("{quizId:guid}/regenerations")]
    [ProducesResponseType(typeof(QuizResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<QuizResponse>> RegenerateQuiz(
        Guid quizId,
        CancellationToken cancellationToken)
    {
        var response = await regenerationService.RegenerateAsync(quizId, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }
}
