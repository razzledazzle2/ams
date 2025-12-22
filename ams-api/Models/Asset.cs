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
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}
