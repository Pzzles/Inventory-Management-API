namespace InventoryManagementSystem.Api.Repositories.Models;

public class RepositoryQueryOptions
{
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 25;

    public bool IncludeDeleted { get; set; }

    public string? Search { get; set; }

    public string? SortBy { get; set; }

    public string? SortDirection { get; set; } = "asc";
}
