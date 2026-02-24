using InventoryManagementSystem.Api.Entities;
using InventoryManagementSystem.Api.Repositories.Models;
using InventoryManagementSystem.Api.Services.Models;

namespace InventoryManagementSystem.Api.Services.Contracts;

public interface IAssetService
{
    Task<ServiceResult<Asset>> CreateAsync(
        Asset asset,
        string operatorName,
        CancellationToken cancellationToken);

    Task<ServiceResult<Asset>> UpdateAsync(
        int id,
        Asset asset,
        string operatorName,
        CancellationToken cancellationToken);

    Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken);

    Task<ServiceResult<Asset?>> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<ServiceResult<PagedResult<Asset>>> GetPagedAsync(
        RepositoryQueryOptions options,
        CancellationToken cancellationToken);

    Task<ServiceResult<Asset>> AssignAsync(
        int assetId,
        int employeeId,
        string operatorName,
        CancellationToken cancellationToken);

    Task<ServiceResult<Asset>> ReturnAsync(
        int assetId,
        string operatorName,
        AssetStatus returnStatus,
        CancellationToken cancellationToken);

    Task<ServiceResult> SwapAsync(
        int oldAssetId,
        int newAssetId,
        int employeeId,
        string operatorName,
        CancellationToken cancellationToken);

    Task<ServiceResult<PagedResult<Asset>>> GetByStatusReportAsync(
        AssetStatus? status,
        RepositoryQueryOptions options,
        CancellationToken cancellationToken);

    Task<ServiceResult<PagedResult<Asset>>> GetByLocationReportAsync(
        int? locationId,
        RepositoryQueryOptions options,
        CancellationToken cancellationToken);

    Task<ServiceResult<PagedResult<Asset>>> GetWarrantyExpiryReportAsync(
        DateTime fromUtc,
        DateTime toUtc,
        RepositoryQueryOptions options,
        CancellationToken cancellationToken);

    Task<ServiceResult<PagedResult<EmployeeAssetsReportItem>>> GetAssetsByEmployeeReportAsync(
        EmployeeAssetsReportOptions options,
        CancellationToken cancellationToken);
}
