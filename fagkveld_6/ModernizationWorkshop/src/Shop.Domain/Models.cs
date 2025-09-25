using System.Text.Json.Serialization;

namespace Shop.Domain;


public record Customer
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public string Email { get; init; } = "";

    public Customer(string name, string email)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
    }
}

public record Product
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public string Sku { get; init; } = "";
    public decimal Price { get; init; }

    public Product(string name, string sku, decimal price)
    {
        Id = Guid.NewGuid();
        Name = name;
        Sku = sku;
        Price = price;
    }
}

public record OrderItem(Guid ProductId, int Quantity);

public record Order(Guid Id, Guid CustomerId, DateTime CreatedUtc, List<OrderItem> Items, string Status)
{
    public Order(Guid customerId, List<OrderItem> items, string status) : this(Guid.NewGuid(), customerId, DateTime.UtcNow, items, status) { }
}

public class CreateOrderRequest
{
    public Guid CustomerId { get; set; }
    public List<OrderItem> Items { get; set; } = new();
}

public class Catalog
{
    public List<Product> Products { get; set; } = new();

    public Product? FindBySku(string sku)
    {
        return Products.FirstOrDefault(p => p.Sku.Equals(sku, StringComparison.OrdinalIgnoreCase));
    }
}
