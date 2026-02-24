using InventoryManagementSystem.Api.DTOs.Requests;
using InventoryManagementSystem.Api.DTOs.Responses;
using InventoryManagementSystem.Api.Entities;

namespace InventoryManagementSystem.Api.Mapping;

public static class RepairMapper
{
    public static Repair ToEntity(CreateRepairRequest request, DateTime loggedAtUtc)
    {
        return new Repair
        {
            AssetId = request.AssetId,
            Vendor = request.Vendor,
            Cost = request.Cost,
            Notes = request.Notes,
            Status = request.Status,
            LoggedAtUtc = loggedAtUtc
        };
    }

    public static void ApplyUpdate(UpdateRepairRequest request, Repair entity)
    {
        entity.AssetId = request.AssetId;
        entity.Vendor = request.Vendor;
        entity.Cost = request.Cost;
        entity.Notes = request.Notes;
        entity.Status = request.Status;
    }

    public static RepairResponse ToResponse(Repair entity)
    {
        return new RepairResponse
        {
            Id = entity.Id,
            AssetId = entity.AssetId,
            Vendor = entity.Vendor,
            Cost = entity.Cost,
            Notes = entity.Notes,
            Status = entity.Status,
            LoggedAtUtc = entity.LoggedAtUtc,
            CompletedAtUtc = entity.CompletedAtUtc,
            CreatedAtUtc = entity.CreatedAtUtc,
            UpdatedAtUtc = entity.UpdatedAtUtc
        };
    }
}
