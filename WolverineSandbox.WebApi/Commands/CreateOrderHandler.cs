using Ardalis.Result;
using WolverineSandbox.Domain.Events;
using WolverineSandbox.WebApi.Data;
using WolverineSandbox.WebApi.Entities;

namespace WolverineSandbox.WebApi.Commands;

public class CreateOrderHandler
{
    public static async Task<ICollection<ValidationError>> ValidateAsync(CreateOrder command, ApplicationDbContext dbContext)
    {
        List<ValidationError> errors = new();

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

    public async Task<Result<OrderCreated>> HandleAsync(CreateOrder command, ApplicationDbContext dbContext, ICollection<ValidationError> errors)
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

        return Result<OrderCreated>.Created(new OrderCreated(order.Id));
    }
}
