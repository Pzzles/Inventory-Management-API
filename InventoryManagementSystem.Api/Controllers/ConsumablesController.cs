using InventoryManagementSystem.Api.DTOs.Requests;
using InventoryManagementSystem.Api.DTOs.Responses;
using InventoryManagementSystem.Api.Mapping;
using InventoryManagementSystem.Api.Repositories.Models;
using InventoryManagementSystem.Api.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ConsumablesController : ControllerBase
{
    private readonly IConsumableService _service;

    public ConsumablesController(IConsumableService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ConsumableResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] RepositoryQueryOptions options,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetPagedAsync(options, cancellationToken);

        return result.ToActionResult(this, paged =>
        {
            var mapped = new PagedResult<ConsumableResponse>(
                paged.Items.Select(ConsumableMapper.ToResponse).ToList(),
                paged.TotalCount,
                paged.PageNumber,
                paged.PageSize);

            return Ok(mapped);
        });
    }

    [HttpGet("low-stock")]
    [ProducesResponseType(typeof(IReadOnlyList<ConsumableResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLowStock(CancellationToken cancellationToken)
    {
        var result = await _service.GetLowStockAsync(cancellationToken);

        return result.ToActionResult(this, items =>
            Ok(items.Select(ConsumableMapper.ToResponse).ToList()));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ConsumableResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);

        return result.ToActionResult(this, entity => Ok(ConsumableMapper.ToResponse(entity!)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ConsumableResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateConsumableRequest request,
        CancellationToken cancellationToken)
    {
        var entity = ConsumableMapper.ToEntity(request);
        var result = await _service.CreateAsync(entity, cancellationToken);

        return result.ToActionResult(this, created =>
        {
            var response = ConsumableMapper.ToResponse(created);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        });
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ConsumableResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateConsumableRequest request,
        CancellationToken cancellationToken)
    {
        var entity = new Entities.Consumable();
        ConsumableMapper.ApplyUpdate(request, entity);

        var result = await _service.UpdateAsync(id, entity, cancellationToken);

        return result.ToActionResult(this, updated => Ok(ConsumableMapper.ToResponse(updated)));
    }

    [HttpPost("{id:int}/stock-in")]
    [ProducesResponseType(typeof(ConsumableResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> StockIn(
        int id,
        [FromBody] StockAdjustmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.StockInAsync(
            id,
            request.Quantity,
            request.Reason,
            request.OperatorName,
            cancellationToken);

        return result.ToActionResult(this, updated => Ok(ConsumableMapper.ToResponse(updated)));
    }

    [HttpPost("{id:int}/stock-out")]
    [ProducesResponseType(typeof(ConsumableResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> StockOut(
        int id,
        [FromBody] StockAdjustmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.StockOutAsync(
            id,
            request.Quantity,
            request.Reason,
            request.OperatorName,
            cancellationToken);

        return result.ToActionResult(this, updated => Ok(ConsumableMapper.ToResponse(updated)));
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
