namespace ams_api.Models;

public class AssetAllocation
{
    public Guid Id { get; set; }

    public Guid AssetId { get; set; }
    public Guid UserId { get; set; }

    public DateTime AllocatedAt { get; set; }
    public Guid AllocatedBy { get; set; }

    public DateTime? ReturnedAt { get; set; }
    public string? ReturnCondition { get; set; }
    public string? ReturnImageUrl { get; set; }
}
