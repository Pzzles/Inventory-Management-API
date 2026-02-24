using InventoryManagementSystem.Api.DTOs.Responses;
using InventoryManagementSystem.Api.Mapping;
using InventoryManagementSystem.Api.Repositories.Models;
using InventoryManagementSystem.Api.Services.Contracts;
using InventoryManagementSystem.Api.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Api.Controllers;

[ApiController]
[Route("api/reports")]
public sealed class ReportsController : ControllerBase
{
    private readonly IAssetService _assetService;

    public ReportsController(IAssetService assetService)
    {
        _assetService = assetService;
    }

    [HttpGet("assets/by-status")]
    [ProducesResponseType(typeof(PagedResult<AssetResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> AssetsByStatus(
        [FromQuery] Entities.AssetStatus? status,
        [FromQuery] RepositoryQueryOptions options,
        CancellationToken cancellationToken)
    {
        var result = await _assetService.GetByStatusReportAsync(status, options, cancellationToken);

        return result.ToActionResult(this, paged =>
        {
            var mapped = new PagedResult<AssetResponse>(
                paged.Items.Select(AssetMapper.ToResponse).ToList(),
                paged.TotalCount,
                paged.PageNumber,
                paged.PageSize);

            return Ok(mapped);
        });
    }

    [HttpGet("assets/by-location")]
    [ProducesResponseType(typeof(PagedResult<AssetResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> AssetsByLocation(
        [FromQuery] int? locationId,
        [FromQuery] RepositoryQueryOptions options,
        CancellationToken cancellationToken)
    {
        var result = await _assetService.GetByLocationReportAsync(locationId, options, cancellationToken);

        return result.ToActionResult(this, paged =>
        {
            var mapped = new PagedResult<AssetResponse>(
                paged.Items.Select(AssetMapper.ToResponse).ToList(),
                paged.TotalCount,
                paged.PageNumber,
                paged.PageSize);

            return Ok(mapped);
        });
    }

    [HttpGet("assets/assigned-by-employee")]
    [ProducesResponseType(typeof(PagedResult<EmployeeAssetsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> AssetsAssignedByEmployee(
        [FromQuery] EmployeeAssetsReportOptions options,
        CancellationToken cancellationToken)
    {
        var result = await _assetService.GetAssetsByEmployeeReportAsync(options, cancellationToken);

        return result.ToActionResult(this, paged =>
        {
            var items = paged.Items.Select(item => new EmployeeAssetsResponse
            {
                EmployeeId = item.Employee.Id,
                FirstName = item.Employee.FirstName,
                LastName = item.Employee.LastName,
                Email = item.Employee.Email,
                StaffNumber = item.Employee.StaffNumber,
                Assets = item.Assets.Select(AssetMapper.ToResponse).ToList()
            }).ToList();

            var mapped = new PagedResult<EmployeeAssetsResponse>(
                items,
                paged.TotalCount,
                paged.PageNumber,
                paged.PageSize);

            return Ok(mapped);
        });
    }

    [HttpGet("assets/warranty-expiry")]
    [ProducesResponseType(typeof(PagedResult<AssetResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> WarrantyExpiry(
        [FromQuery] RepositoryQueryOptions options,
        [FromQuery] int days = 30,
        CancellationToken cancellationToken = default)
    {
        var safeDays = days <= 0 ? 30 : days;
        var fromUtc = DateTime.UtcNow;
        var toUtc = fromUtc.AddDays(safeDays);

        var result = await _assetService.GetWarrantyExpiryReportAsync(fromUtc, toUtc, options, cancellationToken);

        return result.ToActionResult(this, paged =>
        {
            var mapped = new PagedResult<AssetResponse>(
                paged.Items.Select(AssetMapper.ToResponse).ToList(),
                paged.TotalCount,
                paged.PageNumber,
                paged.PageSize);

            return Ok(mapped);
        });
    }
}
