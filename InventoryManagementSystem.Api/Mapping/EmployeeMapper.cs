using InventoryManagementSystem.Api.DTOs.Requests;
using InventoryManagementSystem.Api.DTOs.Responses;
using InventoryManagementSystem.Api.Entities;

namespace InventoryManagementSystem.Api.Mapping;

public static class EmployeeMapper
{
    public static Employee ToEntity(CreateEmployeeRequest request)
    {
        return new Employee
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            StaffNumber = request.StaffNumber,
            Email = request.Email
        };
    }

    public static void ApplyUpdate(UpdateEmployeeRequest request, Employee entity)
    {
        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        entity.StaffNumber = request.StaffNumber;
        entity.Email = request.Email;
    }

    public static EmployeeResponse ToResponse(Employee entity)
    {
        return new EmployeeResponse
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            StaffNumber = entity.StaffNumber,
            Email = entity.Email,
            CreatedAtUtc = entity.CreatedAtUtc,
            UpdatedAtUtc = entity.UpdatedAtUtc
        };
    }
}
