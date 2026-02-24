using InventoryManagementSystem.Api.DTOs.Requests;
using InventoryManagementSystem.Api.DTOs.Responses;
using InventoryManagementSystem.Api.Mapping;
using InventoryManagementSystem.Api.Repositories.Models;
using InventoryManagementSystem.Api.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SuppliersController : ControllerBase
{
    private readonly ISupplierService _service;

    public SuppliersController(ISupplierService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<SupplierResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] RepositoryQueryOptions options,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetPagedAsync(options, cancellationToken);

        return result.ToActionResult(this, paged =>
        {
            var mapped = new PagedResult<SupplierResponse>(
                paged.Items.Select(SupplierMapper.ToResponse).ToList(),
                paged.TotalCount,
                paged.PageNumber,
                paged.PageSize);

            return Ok(mapped);
        });
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(SupplierResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);

        return result.ToActionResult(this, entity => Ok(SupplierMapper.ToResponse(entity!)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(SupplierResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateSupplierRequest request,
        CancellationToken cancellationToken)
    {
        var entity = SupplierMapper.ToEntity(request);
        var result = await _service.CreateAsync(entity, cancellationToken);

        return result.ToActionResult(this, created =>
        {
            var response = SupplierMapper.ToResponse(created);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        });
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(SupplierResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateSupplierRequest request,
        CancellationToken cancellationToken)
    {
        var entity = new Entities.Supplier
        {
            Name = request.Name,
            Code = request.Code,
            Email = request.Email,
            Phone = request.Phone
        };

        var result = await _service.UpdateAsync(id, entity, cancellationToken);

        return result.ToActionResult(this, updated => Ok(SupplierMapper.ToResponse(updated)));
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
