using CreditWorks.VehicleManagement.Application.Contracts;
using CreditWorks.VehicleManagement.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CreditWorks.VehicleManagement.Rest
{
    /// <summary>
    ///     Provides shared response and exception handling for API controllers.
    /// </summary>
    /// <typeparam name="TClass">The controller type used to categorize log entries.</typeparam>
    /// <param name="logger">The logger used for request failures.</param>
    public class BaseController<TClass>(ILoggingService<TClass> logger)
        : ControllerBase where TClass : class
    {
        /// <summary>
        ///     Runs an asynchronous operation and converts its result or exception to an HTTP response.
        /// </summary>
        /// <typeparam name="T">The operation result type.</typeparam>
        /// <param name="func">The operation to run.</param>
        /// <returns>
        ///     An HTTP response containing the result or an error.
        /// </returns>
        protected async Task<IActionResult> ToResponse<T>(Func<Task<T>> func)
        {
            try
            {
                var result = await func.Invoke();
                // Preserve responses already selected by an action, such as Accepted.
                return result is IActionResult actionResult ? actionResult : Ok(result);
            }
            catch (ResourceNotFoundException ex)
            {
                logger.LogInfo($"{ex.Message}", func);
                return Problem(statusCode: 404, title: "Not found", detail: ex.Message);
            }
            catch (BusinessValidationException ex)
            {
                logger.LogInfo($"{ex.Message}", func);
                return Problem(statusCode: 400, title: "Invalid request", detail: ex.Message);
            }
            catch (BusinessConflictException ex)
            {
                logger.LogInfo($"{ex.Message}", func);
                return Problem(statusCode: 409, title: "Conflict", detail: ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{ex.Message}", func);
                return Problem(statusCode: 500, title: "Server error");
            }
        }

    }
}
