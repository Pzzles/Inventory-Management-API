using InventoryManagementSystem.Api.Entities;
using InventoryManagementSystem.Api.Repositories.Contracts;
using InventoryManagementSystem.Api.Repositories.Models;
using InventoryManagementSystem.Api.Services.Contracts;
using InventoryManagementSystem.Api.Services.Models;

namespace InventoryManagementSystem.Api.Services.Implementations;

public sealed class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _repository;

    public AuditLogService(IAuditLogRepository repository)
    {
        _repository = repository;
    }

    public async Task<ServiceResult<AuditLog>> CreateAsync(AuditLog auditLog, CancellationToken cancellationToken)
    {
        var validation = Validate(auditLog);
        if (!validation.IsSuccess)
        {
            return ServiceResult<AuditLog>.Fail(validation.Errors.ToArray());
        }

        var created = await _repository.AddAsync(auditLog, cancellationToken);
        return ServiceResult<AuditLog>.Ok(created);
    }

    public async Task<ServiceResult<AuditLog>> UpdateAsync(
        int id,
        AuditLog auditLog,
        CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return ServiceResult<AuditLog>.Fail(new ServiceError("not_found", "Audit log not found."));
        }

        var validation = Validate(auditLog);
        if (!validation.IsSuccess)
        {
            return ServiceResult<AuditLog>.Fail(validation.Errors.ToArray());
        }

        existing.Action = auditLog.Action;
        existing.EntityName = auditLog.EntityName;
        existing.EntityId = auditLog.EntityId;
        existing.Details = auditLog.Details;

        await _repository.UpdateAsync(existing, cancellationToken);
        return ServiceResult<AuditLog>.Ok(existing);
    }

    public async Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return ServiceResult.Fail(new ServiceError("not_found", "Audit log not found."));
        }

        await _repository.SoftDeleteAsync(existing, cancellationToken);
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult<AuditLog?>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var auditLog = await _repository.GetByIdAsync(id, cancellationToken);
        return ServiceResult<AuditLog?>.Ok(auditLog);
    }

    public async Task<ServiceResult<PagedResult<AuditLog>>> GetPagedAsync(
        RepositoryQueryOptions options,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetPagedAsync(options, cancellationToken);
        return ServiceResult<PagedResult<AuditLog>>.Ok(result);
    }

    private static ServiceResult Validate(AuditLog auditLog)
    {
        var errors = new List<ServiceError>();

        if (string.IsNullOrWhiteSpace(auditLog.Action))
        {
            errors.Add(new ServiceError("action_required", "Action is required."));
        }

        if (string.IsNullOrWhiteSpace(auditLog.EntityName))
        {
            errors.Add(new ServiceError("entity_name_required", "Entity name is required."));
        }

        if (string.IsNullOrWhiteSpace(auditLog.EntityId))
        {
            errors.Add(new ServiceError("entity_id_required", "Entity id is required."));
        }

        return errors.Count == 0
            ? ServiceResult.Ok()
            : ServiceResult.Fail(errors.ToArray());
    }
}
