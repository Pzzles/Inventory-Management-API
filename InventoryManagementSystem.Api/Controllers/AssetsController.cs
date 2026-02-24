using InventoryManagementSystem.Api.DTOs.Requests;
using InventoryManagementSystem.Api.DTOs.Responses;
using InventoryManagementSystem.Api.Mapping;
using InventoryManagementSystem.Api.Repositories.Models;
using InventoryManagementSystem.Api.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AssetsController : ControllerBase
{
    private readonly IAssetService _service;

    public AssetsController(IAssetService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AssetResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] AssetQueryOptions options,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetPagedAsync(options, cancellationToken);

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

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AssetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);

        return result.ToActionResult(this, entity => Ok(AssetMapper.ToResponse(entity!)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(AssetResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateAssetRequest request,
        CancellationToken cancellationToken)
    {
        var entity = AssetMapper.ToEntity(request);
        var result = await _service.CreateAsync(entity, request.OperatorName, cancellationToken);

        return result.ToActionResult(this, created =>
        {
            var response = AssetMapper.ToResponse(created);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        });
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(AssetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateAssetRequest request,
        CancellationToken cancellationToken)
    {
        var entity = new Entities.Asset();
        AssetMapper.ApplyUpdate(request, entity);

        var result = await _service.UpdateAsync(id, entity, request.OperatorName, cancellationToken);

        return result.ToActionResult(this, updated => Ok(AssetMapper.ToResponse(updated)));
    }

    [HttpPost("{id:int}/assign")]
    [ProducesResponseType(typeof(AssetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Assign(
        int id,
        [FromBody] AssignAssetRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.AssignAsync(id, request.EmployeeId, request.OperatorName, cancellationToken);
        return result.ToActionResult(this, updated => Ok(AssetMapper.ToResponse(updated)));
    }

    [HttpPost("{id:int}/return")]
    [ProducesResponseType(typeof(AssetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Return(
        int id,
        [FromBody] ReturnAssetRequest request,
        CancellationToken cancellationToken)
    {
        var returnStatus = request.ReturnStatus ?? Entities.AssetStatus.InStock;
        var result = await _service.ReturnAsync(id, request.OperatorName, returnStatus, cancellationToken);
        return result.ToActionResult(this, updated => Ok(AssetMapper.ToResponse(updated)));
    }

    [HttpPost("swap")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Swap(
        [FromBody] SwapAssetRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.SwapAsync(
            request.OldAssetId,
            request.NewAssetId,
            request.EmployeeId,
            request.OperatorName,
            cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return result.ToActionResult(this);
    }
}
