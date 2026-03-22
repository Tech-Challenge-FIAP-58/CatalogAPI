using FCG.Catalog.Domain.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace FCG.Catalog.WebApi.Controllers
{
	[ApiController]
    [Route("[controller]")]
    public class StandardController : ControllerBase
    {
        protected async Task<IActionResult> TryMethodAsync<TResult>(
            Func<Task<IApiResponse<TResult>>> serviceMethod,
            ILogger logger)
        {
            try
            {
                var result = await serviceMethod();

                // 204 must not have a body
                if (result.StatusCode == HttpStatusCode.NoContent)
                    return StatusCode((int)HttpStatusCode.NoContent);

                if (!result.IsSuccess)
                    return StatusCode((int)result.StatusCode, result);

                return StatusCode((int)result.StatusCode, result);
            }
            // ✅ maps known errors with the real message
            catch (ValidationException ex)
            {
                logger.LogWarning(ex, ex.Message);
                return CreateProblem(HttpStatusCode.BadRequest, ex);
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, ex.Message);
                return CreateProblem(HttpStatusCode.BadRequest, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.LogWarning(ex, ex.Message);
                return CreateProblem(HttpStatusCode.Unauthorized, ex);
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning(ex, ex.Message);
                return CreateProblem(HttpStatusCode.NotFound, ex);
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, ex.Message);
                // Integrity conflict, unique key, FK, etc.
                return CreateProblem(HttpStatusCode.Conflict, ex);
            }
            // ✅ generic fallback keeps the current behavior
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in TryMethodAsync");
                return CreateProblem(HttpStatusCode.InternalServerError, ex, genericOnProd: true);
            }
        }

        protected IActionResult TryMethod<TResult>(
            Func<IApiResponse<TResult>> serviceMethod,
            ILogger logger)
        {
            try
            {
                var result = serviceMethod();

                if (result.StatusCode == HttpStatusCode.NoContent)
                    return StatusCode((int)HttpStatusCode.NoContent);

                if (!result.IsSuccess)
                    return StatusCode((int)result.StatusCode, result);

                return StatusCode((int)result.StatusCode, result);
            }
            catch (ValidationException ex)
            {
                logger.LogWarning(ex, ex.Message);
                return CreateProblem(HttpStatusCode.BadRequest, ex);
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, ex.Message);
                return CreateProblem(HttpStatusCode.BadRequest, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.LogWarning(ex, ex.Message);
                return CreateProblem(HttpStatusCode.Unauthorized, ex);
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning(ex, ex.Message);
                return CreateProblem(HttpStatusCode.NotFound, ex);
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, ex.Message);
                return CreateProblem(HttpStatusCode.Conflict, ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in TryMethod");
                return CreateProblem(HttpStatusCode.InternalServerError, ex, genericOnProd: true);
            }
        }

        private IActionResult CreateProblem(HttpStatusCode code, Exception ex, bool genericOnProd = false)
        {
            var problem = new ProblemDetails
            {
                Status = (int)code,
                Title = ToDefaultTitle(code),
                Detail = ex.Message
            };

            // useful for correlation in logs
            problem.Extensions["traceId"] = HttpContext?.TraceIdentifier;

            return StatusCode(problem.Status.Value, problem);
        }

        private static string ToDefaultTitle(HttpStatusCode code) => code switch
        {
            HttpStatusCode.BadRequest => "Invalid request",
            HttpStatusCode.Unauthorized => "Unauthorized",
            HttpStatusCode.Forbidden => "Forbidden",
            HttpStatusCode.NotFound => "Resource not found",
            HttpStatusCode.Conflict => "Conflict",
            HttpStatusCode.UnprocessableEntity => "Invalid entity",
            _ => "Error"
        };
    }
}
