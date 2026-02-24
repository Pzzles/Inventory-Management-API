using InventoryManagementSystem.Api.Services.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace InventoryManagementSystem.Api.Errors;

public sealed class ApiError
{
    public string Message { get; init; } = "An unexpected error occurred.";

    public string CorrelationId { get; init; } = string.Empty;

    public IReadOnlyDictionary<string, string[]>? Errors { get; init; }

    public static ApiError FromModelState(ModelStateDictionary modelState, string correlationId)
    {
        var errors = modelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(
                entry => entry.Key,
                entry => entry.Value!.Errors.Select(error => error.ErrorMessage).ToArray());

        return new ApiError
        {
            Message = "Validation failed.",
            CorrelationId = correlationId,
            Errors = errors
        };
    }

    public static ApiError FromServiceErrors(IEnumerable<ServiceError> errors, string correlationId)
    {
        var groupedErrors = errors
            .GroupBy(error => error.Code)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.Message).ToArray());

        return new ApiError
        {
            Message = "Validation failed.",
            CorrelationId = correlationId,
            Errors = groupedErrors
        };
    }
}
