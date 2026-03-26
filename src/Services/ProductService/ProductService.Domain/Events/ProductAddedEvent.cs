namespace ProductService.Domain.Events;

public class ProductAddedEvent
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
