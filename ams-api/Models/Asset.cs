namespace ams_api.Models;

public class Asset
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string Status { get; set; } = "available";
    public string? Condition { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public string? Vendor { get; set; }
    public string? Model { get; set; }
    public string? ImageUrl { get; set; }
    public Guid? AssignedTo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
