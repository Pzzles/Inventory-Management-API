using InventoryManagementSystem.Api.Entities;
using InventoryManagementSystem.Api.Repositories.Contracts;
using InventoryManagementSystem.Api.Repositories.Models;
using InventoryManagementSystem.Api.Services.Contracts;
using InventoryManagementSystem.Api.Services.Models;

namespace InventoryManagementSystem.Api.Services.Implementations;

public sealed class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;

    public EmployeeService(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    public async Task<ServiceResult<Employee>> CreateAsync(Employee employee, CancellationToken cancellationToken)
    {
        var validation = Validate(employee);
        if (!validation.IsSuccess)
        {
            return ServiceResult<Employee>.Fail(validation.Errors.ToArray());
        }

        var created = await _repository.AddAsync(employee, cancellationToken);
        return ServiceResult<Employee>.Ok(created);
    }

    public async Task<ServiceResult<Employee>> UpdateAsync(
        int id,
        Employee employee,
        CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return ServiceResult<Employee>.Fail(new ServiceError("not_found", "Employee not found."));
        }

        var validation = Validate(employee);
        if (!validation.IsSuccess)
        {
            return ServiceResult<Employee>.Fail(validation.Errors.ToArray());
        }

        existing.FirstName = employee.FirstName;
        existing.LastName = employee.LastName;
        existing.StaffNumber = employee.StaffNumber;
        existing.Email = employee.Email;

        await _repository.UpdateAsync(existing, cancellationToken);
        return ServiceResult<Employee>.Ok(existing);
    }

    public async Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return ServiceResult.Fail(new ServiceError("not_found", "Employee not found."));
        }

        await _repository.SoftDeleteAsync(existing, cancellationToken);
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult<Employee?>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var employee = await _repository.GetByIdAsync(id, cancellationToken);
        return ServiceResult<Employee?>.Ok(employee);
    }

    public async Task<ServiceResult<PagedResult<Employee>>> GetPagedAsync(
        RepositoryQueryOptions options,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetPagedAsync(options, cancellationToken);
        return ServiceResult<PagedResult<Employee>>.Ok(result);
    }

    private static ServiceResult Validate(Employee employee)
    {
        var errors = new List<ServiceError>();

        if (string.IsNullOrWhiteSpace(employee.FirstName))
        {
            errors.Add(new ServiceError("first_name_required", "First name is required."));
        }

        if (string.IsNullOrWhiteSpace(employee.LastName))
        {
            errors.Add(new ServiceError("last_name_required", "Last name is required."));
        }

        if (string.IsNullOrWhiteSpace(employee.StaffNumber))
        {
            errors.Add(new ServiceError("staff_number_required", "Staff number is required."));
        }

        return errors.Count == 0
            ? ServiceResult.Ok()
            : ServiceResult.Fail(errors.ToArray());
    }
}
