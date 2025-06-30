namespace WolverineSandbox.Domain.Events;

public record OrderCreated(Guid OrderId, string regionCode);
