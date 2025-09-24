namespace Shop.Domain;


public record Customer(Guid Id, string Name, string Email)
{
    public Customer(string name, string email) : this(Guid.NewGuid(), name, email) { }
}

// public class Product
// {
//     public Guid Id { get; set; }
//     public string Sku { get; set; }
//     public string Name { get; set; }
//     public decimal Price { get; set; }
//
//     public Product()
//     {
//         Id = Guid.NewGuid();
//         Sku = string.Empty;
//         Name = string.Empty;
//     }
// }

public class OrderItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

public record Product(Guid Id, string Name, string Sku, decimal Price)
{
    public Product(string name, string sku, decimal price) : this(Guid.NewGuid(), name, sku, price) { }
}

// public record OrderItem(Guid ProductId, int Quantity);
//
// public record Order(Guid Id, Guid CustomerId, DateTime CreatedUtc, List<OrderItem> OrderItems, string Status)
// {
//     public Order(Guid customerId, List<OrderItem> orderItems, string status) : this(Guid.NewGuid(),customerId DateTime.UtcNow, orderItems, status) { }
// }


public class Order
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime CreatedUtc { get; set; }
    public List<OrderItem> Items { get; set; }
    public string Status { get; set; }

    public Order()
    {
        Id = Guid.NewGuid();
        CreatedUtc = DateTime.UtcNow;
        Items = new List<OrderItem>();
        Status = "Received";
    }
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
