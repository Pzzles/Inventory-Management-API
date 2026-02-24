using InventoryManagementSystem.Api.Data;
using InventoryManagementSystem.Api.Entities;
using InventoryManagementSystem.Api.Repositories.Contracts;
using InventoryManagementSystem.Api.Repositories.Models;
using InventoryManagementSystem.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Api.Repositories.EfCore;

public sealed class AssetRepository : Repository<Asset>, IAssetRepository
{
    public AssetRepository(InventoryDbContext context, IDateTimeProvider dateTimeProvider)
        : base(context, dateTimeProvider)
    {
    }

    public async Task<bool> AssetTagExistsAsync(
        string assetTag,
        int? excludeId,
        CancellationToken cancellationToken)
    {
        var query = Entities.IgnoreQueryFilters().AsQueryable();

        if (excludeId.HasValue)
        {
            query = query.Where(asset => asset.Id != excludeId.Value);
        }

        return await query.AnyAsync(
            asset => asset.AssetTag == assetTag,
            cancellationToken);
    }

    protected override IQueryable<Asset> ApplyFilters(
        IQueryable<Asset> query,
        RepositoryQueryOptions options)
    {
        if (options is AssetQueryOptions assetOptions)
        {
            if (!string.IsNullOrWhiteSpace(assetOptions.AssetTag))
            {
                query = query.Where(asset => asset.AssetTag.Contains(assetOptions.AssetTag));
            }

            if (!string.IsNullOrWhiteSpace(assetOptions.SerialNumber))
            {
                query = query.Where(asset => asset.SerialNumber.Contains(assetOptions.SerialNumber));
            }

            if (!string.IsNullOrWhiteSpace(assetOptions.Type))
            {
                query = query.Where(asset => asset.Type.Contains(assetOptions.Type));
            }

            if (!string.IsNullOrWhiteSpace(assetOptions.Brand))
            {
                query = query.Where(asset => asset.Brand.Contains(assetOptions.Brand));
            }
        }

        if (!string.IsNullOrWhiteSpace(options.Search))
        {
            var search = options.Search;
            query = query.Where(asset =>
                asset.AssetTag.Contains(search) ||
                asset.SerialNumber.Contains(search) ||
                asset.Type.Contains(search) ||
                asset.Brand.Contains(search) ||
                asset.Model.Contains(search));
        }

        query = ApplySorting(query, options.SortBy, options.SortDirection);

        return query;
    }

    private static IQueryable<Asset> ApplySorting(
        IQueryable<Asset> query,
        string? sortBy,
        string? sortDirection)
    {
        var descending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return (sortBy ?? string.Empty).ToLowerInvariant() switch
        {
            "assettag" => descending ? query.OrderByDescending(asset => asset.AssetTag) : query.OrderBy(asset => asset.AssetTag),
            "serialnumber" => descending ? query.OrderByDescending(asset => asset.SerialNumber) : query.OrderBy(asset => asset.SerialNumber),
            "type" => descending ? query.OrderByDescending(asset => asset.Type) : query.OrderBy(asset => asset.Type),
            "brand" => descending ? query.OrderByDescending(asset => asset.Brand) : query.OrderBy(asset => asset.Brand),
            "model" => descending ? query.OrderByDescending(asset => asset.Model) : query.OrderBy(asset => asset.Model),
            "purchasedate" => descending ? query.OrderByDescending(asset => asset.PurchaseDate) : query.OrderBy(asset => asset.PurchaseDate),
            "status" => descending ? query.OrderByDescending(asset => asset.Status) : query.OrderBy(asset => asset.Status),
            _ => descending ? query.OrderByDescending(asset => asset.AssetTag) : query.OrderBy(asset => asset.AssetTag)
        };
    }
}
