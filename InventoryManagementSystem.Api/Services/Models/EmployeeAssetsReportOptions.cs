namespace InventoryManagementSystem.Api.Services.Models;

public sealed class EmployeeAssetsReportOptions
{
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 25;

    public int? EmployeeId { get; set; }

    public string? Email { get; set; }

    public string? StaffNumber { get; set; }
}
