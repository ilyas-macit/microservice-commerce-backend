using MediatR;

namespace ProductService.Domain.Events;

public class ProductAddedEvent : INotification
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
