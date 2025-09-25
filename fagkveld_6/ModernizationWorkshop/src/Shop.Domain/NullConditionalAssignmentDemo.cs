namespace Shop.Domain;

// Demonstrates C# 14 null-conditional assignment target: customer?.LastOrder = GetOrder();
public class CustomerWithLastOrder
{
    public CustomerWithLastOrder(string name)
    {
        Name = name;
    }

    public string Name { get; }
    public Order? LastOrder { get; set; }
}

public static class NullConditionalAssignmentDemo
{
    public static void AssignOrder(CustomerWithLastOrder? customer)
    {
        customer?.LastOrder = CreateSampleOrder();
    }

    public static Order CreateSampleOrder()
        => new(status : "Demo", customerId : Guid.NewGuid(), items : new List<OrderItem> { new (Guid.NewGuid(), 1) });
}