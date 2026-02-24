namespace InventoryManagementSystem.Api.Repositories.Models;

public sealed class AssetQueryOptions : RepositoryQueryOptions
{
    public string? AssetTag { get; set; }

    public string? SerialNumber { get; set; }

    public string? Type { get; set; }

    public string? Brand { get; set; }
}
