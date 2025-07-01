using Wolverine;
using WolverineSandbox.Domain.Events;

namespace WolverineSandbox.WebApi.Events.Module1;

public static class OrderCreatedHandler
{
    public static string Handle(OrderCreated orderCreated, Envelope envelope, IMessageContext messageContext)
    {
        //throw new InvalidOperationException("Simulate exception.");

        Console.WriteLine($"[Module1] Handle `OrderCreated`. Order Id: {orderCreated.OrderId}");
        return "OK 1";
    }
}
