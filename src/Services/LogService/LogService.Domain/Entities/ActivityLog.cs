namespace LogService.Domain.Entities;

/// <summary>
/// Log varlığı
/// </summary>
public class ActivityLog
{
    public int Id { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
