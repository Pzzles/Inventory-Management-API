namespace InventoryManagementSystem.Api.Services.Models;

public class ServiceResult
{
    protected ServiceResult(bool isSuccess, IReadOnlyList<ServiceError> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }

    public bool IsSuccess { get; }

    public IReadOnlyList<ServiceError> Errors { get; }

    public static ServiceResult Ok()
    {
        return new ServiceResult(true, Array.Empty<ServiceError>());
    }

    public static ServiceResult Fail(params ServiceError[] errors)
    {
        return new ServiceResult(false, errors);
    }
}
