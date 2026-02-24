using InventoryManagementSystem.Api.DTOs.Requests;
using InventoryManagementSystem.Api.DTOs.Responses;
using InventoryManagementSystem.Api.Entities;

namespace InventoryManagementSystem.Api.Mapping;

public static class SupplierMapper
{
    public static Supplier ToEntity(CreateSupplierRequest request)
    {
        return new Supplier
        {
            Name = request.Name,
            Code = request.Code,
            Email = request.Email,
            Phone = request.Phone
        };
    }

    public static void ApplyUpdate(UpdateSupplierRequest request, Supplier entity)
    {
        entity.Name = request.Name;
        entity.Code = request.Code;
        entity.Email = request.Email;
        entity.Phone = request.Phone;
    }

    public static SupplierResponse ToResponse(Supplier entity)
    {
        return new SupplierResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Code = entity.Code,
            Email = entity.Email,
            Phone = entity.Phone,
            CreatedAtUtc = entity.CreatedAtUtc,
            UpdatedAtUtc = entity.UpdatedAtUtc
        };
    }
}
