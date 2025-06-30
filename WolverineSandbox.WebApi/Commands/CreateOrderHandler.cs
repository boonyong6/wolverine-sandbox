using Ardalis.Result;
using Wolverine;
using WolverineSandbox.Domain.Events;
using WolverineSandbox.WebApi.Data;
using WolverineSandbox.WebApi.Entities;

namespace WolverineSandbox.WebApi.Commands;

public class CreateOrderHandler
{
    public static async Task<ICollection<ValidationError>> ValidateAsync(CreateOrder command, ApplicationDbContext dbContext)
    {
        List<ValidationError> errors = Enumerable.Empty<ValidationError>().ToList();

        if (command.CustomerId is null)
        {
            return errors;
        }

        Customer? customer = await dbContext.Customers.FindAsync(command.CustomerId);
        if (customer is null)
        {
            errors.Add(new() 
            {
                Identifier = nameof(command.CustomerId), 
                ErrorMessage = $"Customer not found. CustomerId: {command.CustomerId}" 
            });
        }

        return errors;
    }

    public async Task<Result<OrderCreated>> HandleAsync(
        CreateOrder command, 
        ApplicationDbContext dbContext, 
        IMessageBus bus,
        ICollection<ValidationError> errors)
    {
        if (errors.Count > 0)
        {
            return Result<OrderCreated>.Invalid(errors);
        }

        Order order = new()
        {
            CustomerId = command.CustomerId,
            OrderDate = DateTimeOffset.UtcNow,
            TotalAmount = command.TotalAmount
        };
        dbContext.Orders.Add(order);

        await dbContext.SaveChangesAsync();

        // Problem: Order Id is not set until SaveChangesAsync is called due to the database generated Id.
        OrderCreated orderCreated = new(order.Id, command.regionCode);
        await bus.PublishAsync(orderCreated, 
            new DeliveryOptions().WithHeader("regionCode", command.regionCode));

        return Result<OrderCreated>.Created(orderCreated);
    }
}
