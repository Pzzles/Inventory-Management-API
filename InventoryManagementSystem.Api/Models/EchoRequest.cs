using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Api.Models;

public sealed class EchoRequest
{
    [Required]
    public string Message { get; set; } = string.Empty;
}
