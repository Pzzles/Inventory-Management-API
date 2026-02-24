using InventoryManagementSystem.Api.DTOs.Requests;
using InventoryManagementSystem.Api.DTOs.Responses;
using InventoryManagementSystem.Api.Mapping;
using InventoryManagementSystem.Api.Repositories.Models;
using InventoryManagementSystem.Api.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class LocationsController : ControllerBase
{
    private readonly ILocationService _service;

    public LocationsController(ILocationService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<LocationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] RepositoryQueryOptions options,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetPagedAsync(options, cancellationToken);

        return result.ToActionResult(this, paged =>
        {
            var mapped = new PagedResult<LocationResponse>(
                paged.Items.Select(LocationMapper.ToResponse).ToList(),
                paged.TotalCount,
                paged.PageNumber,
                paged.PageSize);

            return Ok(mapped);
        });
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(LocationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);

        return result.ToActionResult(this, entity => Ok(LocationMapper.ToResponse(entity!)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(LocationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateLocationRequest request,
        CancellationToken cancellationToken)
    {
        var entity = LocationMapper.ToEntity(request);
        var result = await _service.CreateAsync(entity, cancellationToken);

        return result.ToActionResult(this, created =>
        {
            var response = LocationMapper.ToResponse(created);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        });
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(LocationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateLocationRequest request,
        CancellationToken cancellationToken)
    {
        var entity = new Entities.Location
        {
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            AddressLine1 = request.AddressLine1,
            City = request.City,
            State = request.State,
            Country = request.Country
        };

        var result = await _service.UpdateAsync(id, entity, cancellationToken);

        return result.ToActionResult(this, updated => Ok(LocationMapper.ToResponse(updated)));
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
