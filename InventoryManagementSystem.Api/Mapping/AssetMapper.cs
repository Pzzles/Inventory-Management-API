using InventoryManagementSystem.Api.DTOs.Requests;
using InventoryManagementSystem.Api.DTOs.Responses;
using InventoryManagementSystem.Api.Entities;

namespace InventoryManagementSystem.Api.Mapping;

public static class AssetMapper
{
    public static Asset ToEntity(CreateAssetRequest request)
    {
        return new Asset
        {
            AssetTag = request.AssetTag,
            SerialNumber = request.SerialNumber,
            Type = request.Type,
            Brand = request.Brand,
            Model = request.Model,
            Description = request.Description,
            PurchaseDate = request.PurchaseDate,
            WarrantyExpiry = request.WarrantyExpiry,
            Status = request.Status,
            LocationId = request.LocationId,
            SupplierId = request.SupplierId,
            AssignedEmployeeId = request.AssignedEmployeeId,
            Notes = request.Notes
        };
    }

    public static void ApplyUpdate(UpdateAssetRequest request, Asset entity)
    {
        entity.AssetTag = request.AssetTag;
        entity.SerialNumber = request.SerialNumber;
        entity.Type = request.Type;
        entity.Brand = request.Brand;
        entity.Model = request.Model;
        entity.Description = request.Description;
        entity.PurchaseDate = request.PurchaseDate;
        entity.WarrantyExpiry = request.WarrantyExpiry;
        entity.Status = request.Status;
        entity.LocationId = request.LocationId;
        entity.SupplierId = request.SupplierId;
        entity.AssignedEmployeeId = request.AssignedEmployeeId;
        entity.Notes = request.Notes;
    }

    public static AssetResponse ToResponse(Asset entity)
    {
        return new AssetResponse
        {
            Id = entity.Id,
            AssetTag = entity.AssetTag,
            SerialNumber = entity.SerialNumber,
            Type = entity.Type,
            Brand = entity.Brand,
            Model = entity.Model,
            Description = entity.Description,
            PurchaseDate = entity.PurchaseDate,
            WarrantyExpiry = entity.WarrantyExpiry,
            Status = entity.Status,
            LocationId = entity.LocationId,
            SupplierId = entity.SupplierId,
            AssignedEmployeeId = entity.AssignedEmployeeId,
            Notes = entity.Notes,
            CreatedAtUtc = entity.CreatedAtUtc,
            UpdatedAtUtc = entity.UpdatedAtUtc
        };
    }
}
