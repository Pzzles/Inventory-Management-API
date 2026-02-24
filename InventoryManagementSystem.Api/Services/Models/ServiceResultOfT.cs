namespace InventoryManagementSystem.Api.Services.Models;

public sealed class ServiceResult<T> : ServiceResult
{
    private ServiceResult(bool isSuccess, T? value, IReadOnlyList<ServiceError> errors)
        : base(isSuccess, errors)
    {
        Value = value;
    }

    public T? Value { get; }

    public static ServiceResult<T> Ok(T value)
    {
        return new ServiceResult<T>(true, value, Array.Empty<ServiceError>());
    }

    public new static ServiceResult<T> Fail(params ServiceError[] errors)
    {
        return new ServiceResult<T>(false, default, errors);
    }
}
