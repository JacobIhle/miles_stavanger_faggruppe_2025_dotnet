using System.Text.Json.Serialization;

namespace Shop.Domain;


public record Customer(Guid Id, string Name, string Email)
{
    public Customer(string name, string email) : this(Guid.NewGuid(), name, email) { }
}

public record Product
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public string Sku { get; init; } = "";
    public decimal Price { get; init; }

    // Parameterless for System.Text.Json
    public Product() { }

    // Convenience ctor for seeding (auto-generate Id)
    public Product(string name, string sku, decimal price)
    {
        Id = Guid.NewGuid();
        Name = name;
        Sku = sku;
        Price = price;
    }

    // Full ctor for deserialization when Id is present
    [JsonConstructor]
    public Product(Guid id, string name, string sku, decimal price)
    {
        Id = id;
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

// public class OrderItem
// {
//     public Guid ProductId { get; set; }
//     public int Quantity { get; set; }
// }


// public class Order
// {
//     public Guid Id { get; set; }
//     public Guid CustomerId { get; set; }
//     public DateTime CreatedUtc { get; set; }
//     public List<OrderItem> Items { get; set; }
//     public string Status { get; set; }
//
//     public Order()
//     {
//         Id = Guid.NewGuid();
//         CreatedUtc = DateTime.UtcNow;
//         Items = new List<OrderItem>();
//         Status = "Received";
//     }
// }

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
