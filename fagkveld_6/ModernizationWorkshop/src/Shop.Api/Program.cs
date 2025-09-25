using Shop.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>();
builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
builder.Services.AddSingleton<Catalog>(_ => SeedCatalog());
builder.Services.AddSingleton<PricingService>();
builder.Services.AddSingleton<OrderService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

// Seed a couple of customers and products at startup
SeedData(app.Services);
SeedInitialOrder(app.Services);

app.Run();

static Catalog SeedCatalog()
{
    var catalog = new Catalog
    {
        Products = new List<Product>
        {
            new (name: "test", "sku", price: 1.0m),
            new ("Espresso", "ESP-01", 29.0m),
            new ("Latte", "LAT-01", 39.0m),
            new ("Cappuccino", "CAP-01", 35.0m),
        }
    };
    return catalog;
}

static void SeedData(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var customers = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();
    var products = scope.ServiceProvider.GetRequiredService<IProductRepository>();
    var catalog = scope.ServiceProvider.GetRequiredService<Catalog>();

    foreach (var p in catalog.Products)
    {
        products.Add(p);
    }

    var ada = new Customer("Ada Lovelace", "Ada.Lovelace@example.com");
    customers.Add(ada);

    var demoCustomer = new CustomerWithLastOrder(ada.Name);
    NullConditionalAssignmentDemo.AssignOrder(demoCustomer);
}

static void SeedInitialOrder(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var orders = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
    var customers = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();
    var catalog = scope.ServiceProvider.GetRequiredService<Catalog>();

    var customer = customers.GetAll().FirstOrDefault();
    if (customer is null) return; 

    // Avoid duplicating if an order already exists for this customer
    if (orders.GetAll().Any(o => o.CustomerId == customer.Id)) return;

    var product = catalog.Products.FirstOrDefault();
    if (product is null) return;

    var order = new Order(
        customerId: customer.Id,
        items: new List<OrderItem> { new(product.Id, 2) },
        status: "Received"
    );

    orders.Add(order);
    Console.WriteLine($"\u001b[32mSeeded initial order {order.Id} for customer {customer.Name} with product {product.Name}\u001b[0m");
}
