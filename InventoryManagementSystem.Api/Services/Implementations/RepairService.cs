using InventoryManagementSystem.Api.Entities;
using InventoryManagementSystem.Api.Repositories.Contracts;
using InventoryManagementSystem.Api.Repositories.Models;
using InventoryManagementSystem.Api.Services;
using InventoryManagementSystem.Api.Services.Contracts;
using InventoryManagementSystem.Api.Services.Models;

namespace InventoryManagementSystem.Api.Services.Implementations;

public sealed class RepairService : IRepairService
{
    private readonly IRepairRepository _repository;
    private readonly IAssetRepository _assetRepository;
    private readonly IAssetStatusHistoryRepository _statusHistoryRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IAuditLogService _auditLogService;

    public RepairService(
        IRepairRepository repository,
        IAssetRepository assetRepository,
        IAssetStatusHistoryRepository statusHistoryRepository,
        IDateTimeProvider dateTimeProvider,
        IAuditLogService auditLogService)
    {
        _repository = repository;
        _assetRepository = assetRepository;
        _statusHistoryRepository = statusHistoryRepository;
        _dateTimeProvider = dateTimeProvider;
        _auditLogService = auditLogService;
    }

    public async Task<ServiceResult<Repair>> CreateAsync(
        Repair repair,
        string operatorName,
        CancellationToken cancellationToken)
    {
        var validation = Validate(repair, operatorName);
        if (!validation.IsSuccess)
        {
            return ServiceResult<Repair>.Fail(validation.Errors.ToArray());
        }

        var asset = await _assetRepository.GetByIdAsync(repair.AssetId, cancellationToken);
        if (asset is null)
        {
            return ServiceResult<Repair>.Fail(new ServiceError("asset_not_found", "Asset not found."));
        }

        repair.LoggedAtUtc = _dateTimeProvider.UtcNow;
        repair.CompletedAtUtc = repair.Status == RepairStatus.Completed ? _dateTimeProvider.UtcNow : null;

        var created = await _repository.AddAsync(repair, cancellationToken);

        await EnsureAssetInRepair(asset, operatorName, cancellationToken);

        await WriteAudit("repair_create", "Repair", created.Id.ToString(), operatorName, "Repair logged.", cancellationToken);

        return ServiceResult<Repair>.Ok(created);
    }

    public async Task<ServiceResult<Repair>> UpdateAsync(
        int id,
        Repair repair,
        string operatorName,
        CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return ServiceResult<Repair>.Fail(new ServiceError("not_found", "Repair not found."));
        }

        var validation = Validate(repair, operatorName);
        if (!validation.IsSuccess)
        {
            return ServiceResult<Repair>.Fail(validation.Errors.ToArray());
        }

        if (!IsValidStatusTransition(existing.Status, repair.Status))
        {
            return ServiceResult<Repair>.Fail(new ServiceError(
                "invalid_status_transition",
                $"Cannot transition from {existing.Status} to {repair.Status}."));
        }

        var asset = await _assetRepository.GetByIdAsync(repair.AssetId, cancellationToken);
        if (asset is null)
        {
            return ServiceResult<Repair>.Fail(new ServiceError("asset_not_found", "Asset not found."));
        }

        existing.AssetId = repair.AssetId;
        existing.Vendor = repair.Vendor;
        existing.Cost = repair.Cost;
        existing.Notes = repair.Notes;
        existing.Status = repair.Status;
        existing.CompletedAtUtc = repair.Status == RepairStatus.Completed
            ? _dateTimeProvider.UtcNow
            : null;

        await _repository.UpdateAsync(existing, cancellationToken);

        if (repair.Status is RepairStatus.Logged or RepairStatus.InProgress)
        {
            await EnsureAssetInRepair(asset, operatorName, cancellationToken);
        }

        await WriteAudit("repair_update", "Repair", existing.Id.ToString(), operatorName, "Repair updated.", cancellationToken);

        return ServiceResult<Repair>.Ok(existing);
    }

    public async Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return ServiceResult.Fail(new ServiceError("not_found", "Repair not found."));
        }

        await _repository.SoftDeleteAsync(existing, cancellationToken);

        await WriteAudit("repair_delete", "Repair", id.ToString(), "system", "Repair deleted.", cancellationToken);

        return ServiceResult.Ok();
    }

    public async Task<ServiceResult<Repair?>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var repair = await _repository.GetByIdAsync(id, cancellationToken);
        return ServiceResult<Repair?>.Ok(repair);
    }

    public async Task<ServiceResult<PagedResult<Repair>>> GetPagedAsync(
        RepositoryQueryOptions options,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetPagedAsync(options, cancellationToken);
        return ServiceResult<PagedResult<Repair>>.Ok(result);
    }

    private static ServiceResult Validate(Repair repair)
        => Validate(repair, "system");

    private static ServiceResult Validate(Repair repair, string operatorName)
    {
        var errors = new List<ServiceError>();

        if (repair.AssetId <= 0)
        {
            errors.Add(new ServiceError("asset_required", "Asset is required."));
        }

        if (string.IsNullOrWhiteSpace(repair.Vendor))
        {
            errors.Add(new ServiceError("vendor_required", "Vendor is required."));
        }

        if (string.IsNullOrWhiteSpace(operatorName))
        {
            errors.Add(new ServiceError("operator_required", "Operator name is required."));
        }

        return errors.Count == 0
            ? ServiceResult.Ok()
            : ServiceResult.Fail(errors.ToArray());
    }

    private static bool IsValidStatusTransition(RepairStatus fromStatus, RepairStatus toStatus)
    {
        if (fromStatus == toStatus)
        {
            return true;
        }

        return fromStatus switch
        {
            RepairStatus.Logged => toStatus is RepairStatus.InProgress or RepairStatus.Completed,
            RepairStatus.InProgress => toStatus is RepairStatus.Completed,
            RepairStatus.Completed => false,
            _ => false
        };
    }

    private async Task EnsureAssetInRepair(
        Asset asset,
        string operatorName,
        CancellationToken cancellationToken)
    {
        if (asset.Status == AssetStatus.InRepair)
        {
            return;
        }

        if (asset.Status is AssetStatus.Retired or AssetStatus.LostStolen)
        {
            return;
        }

        var fromStatus = asset.Status;
        asset.Status = AssetStatus.InRepair;
        await _assetRepository.UpdateAsync(asset, cancellationToken);

        var statusHistory = new AssetStatusHistory
        {
            AssetId = asset.Id,
            FromStatus = fromStatus,
            ToStatus = AssetStatus.InRepair,
            ChangedAtUtc = _dateTimeProvider.UtcNow,
            OperatorName = operatorName
        };

        await _statusHistoryRepository.AddAsync(statusHistory, cancellationToken);

        await WriteAudit(
            "asset_status_change",
            "Asset",
            asset.Id.ToString(),
            operatorName,
            $"Status changed to {AssetStatus.InRepair}.",
            cancellationToken);
    }

    private async Task WriteAudit(
        string action,
        string entityName,
        string entityId,
        string operatorName,
        string summary,
        CancellationToken cancellationToken)
    {
        var audit = new AuditLog
        {
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            OperatorName = operatorName,
            Summary = summary,
            OccurredAtUtc = _dateTimeProvider.UtcNow
        };

        await _auditLogService.CreateAsync(audit, cancellationToken);
    }
}
