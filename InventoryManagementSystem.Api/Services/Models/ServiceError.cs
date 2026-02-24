namespace InventoryManagementSystem.Api.Services.Models;

public sealed class ServiceError
{
    public ServiceError(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public string Code { get; }

    public string Message { get; }
}
