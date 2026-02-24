using InventoryManagementSystem.Api.DTOs.Requests;
using InventoryManagementSystem.Api.DTOs.Responses;
using InventoryManagementSystem.Api.Entities;

namespace InventoryManagementSystem.Api.Mapping;

public static class LocationMapper
{
    public static Location ToEntity(CreateLocationRequest request)
    {
        return new Location
        {
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            AddressLine1 = request.AddressLine1,
            City = request.City,
            State = request.State,
            Country = request.Country
        };
    }

    public static void ApplyUpdate(UpdateLocationRequest request, Location entity)
    {
        entity.Name = request.Name;
        entity.Code = request.Code;
        entity.Description = request.Description;
        entity.AddressLine1 = request.AddressLine1;
        entity.City = request.City;
        entity.State = request.State;
        entity.Country = request.Country;
    }

    public static LocationResponse ToResponse(Location entity)
    {
        return new LocationResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Code = entity.Code,
            Description = entity.Description,
            AddressLine1 = entity.AddressLine1,
            City = entity.City,
            State = entity.State,
            Country = entity.Country,
            CreatedAtUtc = entity.CreatedAtUtc,
            UpdatedAtUtc = entity.UpdatedAtUtc
        };
    }
}
