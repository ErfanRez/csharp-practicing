using ETL.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace ETL.API.Infrastructure
{
    public static class ControllerExtensions
    {
        public static IActionResult ToActionResult(this ControllerBase controller, Error error)
        {
            object payload() => new { error = error.Code, message = error.Description };

            return error.Type switch
            {
                ErrorType.Validation | ErrorType.Failure => controller.BadRequest(payload()),
                ErrorType.NotFound => controller.NotFound(payload()),
                ErrorType.Conflict => controller.Conflict(payload()),
                ErrorType.Problem => controller.StatusCode(500, payload()),
                _ => controller.StatusCode(500, payload())
            };
        }
        public static IActionResult FromResult<T>(this ControllerBase controller, Result<T> result)
        {
            if (!result.IsFailure)
                return controller.Ok(result.Value);

            return controller.ToActionResult(result.Error);
        }

        public static IActionResult FromResult(this ControllerBase controller, Result result)
        {
            if (!result.IsFailure)
                return controller.Ok();

            return controller.ToActionResult(result.Error);
        }
    }
}
