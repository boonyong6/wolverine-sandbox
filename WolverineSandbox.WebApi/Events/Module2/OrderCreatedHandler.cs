using WolverineSandbox.Domain.Events;

namespace WolverineSandbox.WebApi.Events.Module2;

public static class OrderCreatedHandler
{
    public static string Handle(OrderCreated orderCreated)
    {
        Console.WriteLine($"[Module1] Handle `OrderCreated`. Order Id:  {orderCreated.OrderId}");
        return "OK 2";
    }
}
