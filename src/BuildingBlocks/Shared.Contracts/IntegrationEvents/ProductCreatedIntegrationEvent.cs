namespace Shared.Contracts.IntegrationEvents;

public class ProductCreatedIntegrationEvent
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
