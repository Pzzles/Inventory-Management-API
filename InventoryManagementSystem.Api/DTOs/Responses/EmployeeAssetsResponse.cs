using InventoryManagementSystem.Api.DTOs.Responses;

namespace InventoryManagementSystem.Api.DTOs.Responses;

public sealed class EmployeeAssetsResponse
{
    public int EmployeeId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string StaffNumber { get; set; } = string.Empty;

    public IReadOnlyList<AssetResponse> Assets { get; set; } = Array.Empty<AssetResponse>();
}
