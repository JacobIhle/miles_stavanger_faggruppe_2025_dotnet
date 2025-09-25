using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Shop.Domain;

public interface ICustomerRepository
{
    Customer? Get(Guid id);
    IEnumerable<Customer> GetAll();
    void Add(Customer c);
}

public interface IProductRepository
{
    Product? Get(Guid id);
    IEnumerable<Product> GetAll();
    void Add(Product p);
}

public interface IOrderRepository
{
    Order? Get(Guid id);
    IEnumerable<Order> GetAll();
    void Add(Order o);
    void Update(Order o);
}

public class InMemoryCustomerRepository : ICustomerRepository
{
    private readonly ConcurrentDictionary<Guid, Customer> _store = new();

    public Customer? Get(Guid id) => _store.TryGetValue(id, out var c) ? c : null;
    public IEnumerable<Customer> GetAll() => _store.Values;
    public void Add(Customer c) => _store[c.Id] = c;
}

public class InMemoryProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<Guid, Product> _store = new();
    public Product? Get(Guid id) => _store.TryGetValue(id, out var p) ? p : null;
    public IEnumerable<Product> GetAll() => _store.Values;
    public void Add(Product p) => _store[p.Id] = p;
}

public class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, Order> _store = new();
    public Order? Get(Guid id) => _store.TryGetValue(id, out var o) ? o : null;
    public IEnumerable<Order> GetAll() => _store.Values.OrderByDescending(o => o.CreatedUtc);
    public void Add(Order o) => _store[o.Id] = o;
    public void Update(Order o) => _store[o.Id] = o;
}

public class PricingService(Catalog catalog)
{
    private readonly Catalog _catalog = catalog;

    public decimal CalculateTotal(Order order)
    {
        decimal total = 0m;
        foreach (var item in order.Items)
        {
            var product = _catalog.Products.FirstOrDefault(p => p.Id == item.ProductId);
            if (product != null)
            {
                total += product.Price * item.Quantity;
            }
        }
        return total;
    }
}

public class OrderService(IOrderRepository orders, ICustomerRepository customers, PricingService pricing)
{
    private readonly PricingService _pricing = pricing; // kept although not used directly here yet

    public Order? Get(Guid id) => orders.Get(id);
    public IEnumerable<Order> GetAll() => orders.GetAll();

    public Order Create(CreateOrderRequest req)
    {
        var customer = customers.Get(req.CustomerId);
        if (customer == null) throw new InvalidOperationException("Customer not found");
        
        var order = new Order
        (
            customerId: customer.Id, items: req.Items.ToList(), status: "Received"
        );

        orders.Add(order);
        return order;
    }

    public async IAsyncEnumerable<string> ProcessOrderAsync(Guid id, [EnumeratorCancellation] CancellationToken ct = default)
    {
        var order = orders.Get(id) ?? throw new InvalidOperationException("Order not found");

        order = order with { Status = "Brewing" };
        orders.Update(order);

        yield return "Brewing";
        await Task.Delay(200, ct);

        order = order with { Status = "Packing" };
        orders.Update(order);
        yield return "Packing";
        await Task.Delay(200, ct);

        order = order with { Status = "Ready" };
        orders.Update(order);
        yield return "Ready";
        await Task.Delay(200, ct);
    }
}
