using InventoryManagementSystem.Api.Entities;
using InventoryManagementSystem.Api.Repositories.Models;
using InventoryManagementSystem.Api.Services.Models;

namespace InventoryManagementSystem.Api.Services.Contracts;

public interface IEmployeeService
{
    Task<ServiceResult<Employee>> CreateAsync(Employee employee, CancellationToken cancellationToken);

    Task<ServiceResult<Employee>> UpdateAsync(int id, Employee employee, CancellationToken cancellationToken);

    Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken);

    Task<ServiceResult<Employee?>> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<ServiceResult<PagedResult<Employee>>> GetPagedAsync(
        RepositoryQueryOptions options,
        CancellationToken cancellationToken);
}
