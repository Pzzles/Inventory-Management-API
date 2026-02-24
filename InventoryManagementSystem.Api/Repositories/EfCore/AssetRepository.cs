using InventoryManagementSystem.Api.Data;
using InventoryManagementSystem.Api.Entities;
using InventoryManagementSystem.Api.Repositories.Contracts;
using InventoryManagementSystem.Api.Services;

namespace InventoryManagementSystem.Api.Repositories.EfCore;

public sealed class AssetRepository : Repository<Asset>, IAssetRepository
{
    public AssetRepository(InventoryDbContext context, IDateTimeProvider dateTimeProvider)
        : base(context, dateTimeProvider)
    {
    }
}
