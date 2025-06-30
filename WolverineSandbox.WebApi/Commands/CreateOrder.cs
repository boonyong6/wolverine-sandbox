namespace WolverineSandbox.WebApi.Commands;

public record CreateOrder(decimal TotalAmount, string regionCode, Guid? CustomerId = null);
