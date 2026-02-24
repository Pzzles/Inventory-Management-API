using InventoryManagementSystem.Api.DTOs.Requests;
using InventoryManagementSystem.Api.DTOs.Responses;
using InventoryManagementSystem.Api.Mapping;
using InventoryManagementSystem.Api.Repositories.Models;
using InventoryManagementSystem.Api.Services;
using InventoryManagementSystem.Api.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class RepairsController : ControllerBase
{
    private readonly IRepairService _service;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RepairsController(IRepairService service, IDateTimeProvider dateTimeProvider)
    {
        _service = service;
        _dateTimeProvider = dateTimeProvider;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<RepairResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] RepositoryQueryOptions options,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetPagedAsync(options, cancellationToken);

        return result.ToActionResult(this, paged =>
        {
            var mapped = new PagedResult<RepairResponse>(
                paged.Items.Select(RepairMapper.ToResponse).ToList(),
                paged.TotalCount,
                paged.PageNumber,
                paged.PageSize);

            return Ok(mapped);
        });
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RepairResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);

        return result.ToActionResult(this, entity => Ok(RepairMapper.ToResponse(entity!)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(RepairResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateRepairRequest request,
        CancellationToken cancellationToken)
    {
        var entity = RepairMapper.ToEntity(request, _dateTimeProvider.UtcNow);
        var result = await _service.CreateAsync(entity, request.OperatorName, cancellationToken);

        return result.ToActionResult(this, created =>
        {
            var response = RepairMapper.ToResponse(created);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        });
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(RepairResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateRepairRequest request,
        CancellationToken cancellationToken)
    {
        var entity = new Entities.Repair();
        RepairMapper.ApplyUpdate(request, entity);

        var result = await _service.UpdateAsync(id, entity, request.OperatorName, cancellationToken);

        return result.ToActionResult(this, updated => Ok(RepairMapper.ToResponse(updated)));
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
