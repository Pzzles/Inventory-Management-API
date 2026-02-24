namespace InventoryManagementSystem.Api.Repositories.Models;

public sealed class RepositoryQueryOptions
{
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 25;

    public bool IncludeDeleted { get; set; }

    public string? Search { get; set; }
}
