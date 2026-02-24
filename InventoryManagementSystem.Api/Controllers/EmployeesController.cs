using InventoryManagementSystem.Api.DTOs.Requests;
using InventoryManagementSystem.Api.DTOs.Responses;
using InventoryManagementSystem.Api.Mapping;
using InventoryManagementSystem.Api.Repositories.Models;
using InventoryManagementSystem.Api.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;

    public EmployeesController(IEmployeeService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<EmployeeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] RepositoryQueryOptions options,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetPagedAsync(options, cancellationToken);

        return result.ToActionResult(this, paged =>
        {
            var mapped = new PagedResult<EmployeeResponse>(
                paged.Items.Select(EmployeeMapper.ToResponse).ToList(),
                paged.TotalCount,
                paged.PageNumber,
                paged.PageSize);

            return Ok(mapped);
        });
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);

        return result.ToActionResult(this, entity => Ok(EmployeeMapper.ToResponse(entity!)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var entity = EmployeeMapper.ToEntity(request);
        var result = await _service.CreateAsync(entity, cancellationToken);

        return result.ToActionResult(this, created =>
        {
            var response = EmployeeMapper.ToResponse(created);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        });
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var entity = new Entities.Employee
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            StaffNumber = request.StaffNumber,
            Email = request.Email
        };

        var result = await _service.UpdateAsync(id, entity, cancellationToken);

        return result.ToActionResult(this, updated => Ok(EmployeeMapper.ToResponse(updated)));
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
