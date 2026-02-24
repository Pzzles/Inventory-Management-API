using InventoryManagementSystem.Api.Data;
using InventoryManagementSystem.Api.Entities;
using InventoryManagementSystem.Api.Repositories.Contracts;
using InventoryManagementSystem.Api.Services;

namespace InventoryManagementSystem.Api.Repositories.EfCore;

public sealed class LocationRepository : Repository<Location>, ILocationRepository
{
    public LocationRepository(InventoryDbContext context, IDateTimeProvider dateTimeProvider)
        : base(context, dateTimeProvider)
    {
    }
}
