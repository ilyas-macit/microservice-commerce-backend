namespace ProductService.Domain.Entities;

/// <summary>
/// Ürün varlığı
/// </summary>
public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public static Product Create(string name, string description, decimal price, int stock)
    {
        Validate(price, stock);

        return new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Price = price,
            Stock = stock,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string description, decimal price, int stock)
    {
        Validate(price, stock);

        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void Validate(decimal price, int stock)
    {
        if (price < 0)
        {
            throw new ArgumentException("Price cannot be negative", nameof(price));
        }

        if (stock < 0)
        {
            throw new ArgumentException("Stock cannot be negative", nameof(stock));
        }
    }
}
