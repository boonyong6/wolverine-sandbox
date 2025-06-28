using WolverineSandbox.WebApi.Data;
using WolverineSandbox.WebApi.Entities;

namespace WolverineSandbox.WebApi.Commands;

public static class ShipOrderHandler
{
    // This would be called first.
    public static async Task<(Order, Customer?)> LoadAsync(ShipOrder command, ApplicationDbContext dbContext)
    {
        Order order = await dbContext.Orders.FindAsync(command.OrderId)
            ?? throw new InvalidOperationException($"Order with ID {command.OrderId} not found.");
        Customer? customer = await dbContext.Customers.FindAsync(command.CustomerId);

        return (order, customer);
    }

    // By making this method completely synchronous and having it just receive the
    // data it needs to make determinations of what to do next, Wolverine makes this
    // business logic easy to unit test.
    public static string Handle(ShipOrder command, Order order, Customer? customer)
    {
        // Use the command data, plus the related Order & Customer data to 
        // "decide" what action to take next.

        return "OrderShipped";
    }
}
