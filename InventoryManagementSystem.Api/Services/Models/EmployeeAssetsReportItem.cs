using InventoryManagementSystem.Api.Entities;

namespace InventoryManagementSystem.Api.Services.Models;

public sealed class EmployeeAssetsReportItem
{
    public EmployeeAssetsReportItem(Employee employee, IReadOnlyList<Asset> assets)
    {
        Employee = employee;
        Assets = assets;
    }

    public Employee Employee { get; }

    public IReadOnlyList<Asset> Assets { get; }
}
