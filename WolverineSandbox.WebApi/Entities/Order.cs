namespace WolverineSandbox.WebApi.Entities;

public class Order : Entity
{
    public Guid? CustomerId { get; set; }
    public DateTimeOffset OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
}
