using InventoryManagementSystem.Api.DTOs.Requests;
using InventoryManagementSystem.Api.DTOs.Responses;
using InventoryManagementSystem.Api.Entities;

namespace InventoryManagementSystem.Api.Mapping;

public static class ConsumableMapper
{
    public static Consumable ToEntity(CreateConsumableRequest request)
    {
        return new Consumable
        {
            Name = request.Name,
            Category = request.Category,
            Unit = request.Unit,
            QuantityOnHand = request.QuantityOnHand,
            ReorderLevel = request.ReorderLevel,
            LocationId = request.LocationId
        };
    }

    public static void ApplyUpdate(UpdateConsumableRequest request, Consumable entity)
    {
        entity.Name = request.Name;
        entity.Category = request.Category;
        entity.Unit = request.Unit;
        entity.QuantityOnHand = request.QuantityOnHand;
        entity.ReorderLevel = request.ReorderLevel;
        entity.LocationId = request.LocationId;
    }

    public static ConsumableResponse ToResponse(Consumable entity)
    {
        return new ConsumableResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Category = entity.Category,
            Unit = entity.Unit,
            QuantityOnHand = entity.QuantityOnHand,
            ReorderLevel = entity.ReorderLevel,
            IsLowStock = entity.QuantityOnHand <= entity.ReorderLevel,
            LocationId = entity.LocationId,
            CreatedAtUtc = entity.CreatedAtUtc,
            UpdatedAtUtc = entity.UpdatedAtUtc
        };
    }
}
