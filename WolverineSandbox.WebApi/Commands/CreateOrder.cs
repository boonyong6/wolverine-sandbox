namespace WolverineSandbox.WebApi.Commands;

public record CreateOrder(decimal TotalAmount, Guid? CustomerId = null);
