using System.ComponentModel.DataAnnotations;
using InventoryManagementSystem.Api.Entities;

namespace InventoryManagementSystem.Api.DTOs.Requests;

public sealed class ReturnAssetRequest
{
    [Required]
    public string OperatorName { get; set; } = string.Empty;

    public AssetStatus? ReturnStatus { get; set; }
}
