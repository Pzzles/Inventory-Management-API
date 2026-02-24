using InventoryManagementSystem.Api.Entities;
using InventoryManagementSystem.Api.Repositories.Models;
using InventoryManagementSystem.Api.Services.Models;

namespace InventoryManagementSystem.Api.Services.Contracts;

public interface IAuditLogService
{
    Task<ServiceResult<AuditLog>> CreateAsync(AuditLog auditLog, CancellationToken cancellationToken);

    Task<ServiceResult<AuditLog>> UpdateAsync(int id, AuditLog auditLog, CancellationToken cancellationToken);

    Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken);

    Task<ServiceResult<AuditLog?>> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<ServiceResult<PagedResult<AuditLog>>> GetPagedAsync(
        RepositoryQueryOptions options,
        CancellationToken cancellationToken);
}
