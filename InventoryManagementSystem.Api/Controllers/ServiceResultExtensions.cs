using InventoryManagementSystem.Api.Errors;
using InventoryManagementSystem.Api.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Api.Controllers;

public static class ServiceResultExtensions
{
    public static IActionResult ToActionResult(this ServiceResult result, ControllerBase controller)
    {
        if (result.IsSuccess)
        {
            return controller.NoContent();
        }

        return ToErrorResult(result.Errors, controller);
    }

    public static IActionResult ToActionResult<T>(
        this ServiceResult<T> result,
        ControllerBase controller,
        Func<T, IActionResult> onSuccess)
    {
        if (result.IsSuccess && result.Value is not null)
        {
            return onSuccess(result.Value);
        }

        if (result.IsSuccess)
        {
            return controller.NotFound(ApiError.FromServiceErrors(
                [new ServiceError("not_found", "Resource not found.")],
                controller.HttpContext.TraceIdentifier));
        }

        return ToErrorResult(result.Errors, controller);
    }

    private static IActionResult ToErrorResult(
        IReadOnlyList<ServiceError> errors,
        ControllerBase controller)
    {
        var correlationId = controller.HttpContext.TraceIdentifier;

        if (errors.Any(error => error.Code == "not_found"))
        {
            var message = errors.First(error => error.Code == "not_found").Message;
            return controller.NotFound(new ApiError
            {
                Message = message,
                CorrelationId = correlationId
            });
        }

        return controller.BadRequest(ApiError.FromServiceErrors(errors, correlationId));
    }
}
