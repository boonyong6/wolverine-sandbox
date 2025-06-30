using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Wolverine;
using WolverineSandbox.Domain.Events;
using WolverineSandbox.WebApi.Commands;

namespace WolverineSandbox.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IMessageBus _bus;

    public OrdersController(IMessageBus bus)
    {
        _bus = bus;
    }

    [TranslateResultToActionResult]
    [ExpectedFailures(ResultStatus.Invalid, ResultStatus.Created)]
    [HttpPost]
    public async Task<Result<OrderCreated>> Create([FromBody] CreateOrderRequest request)
    {
        CreateOrder command = new(request.TotalAmount, request.regionCode, request.CustomerId);

        // NOTE: If the response (e.g. OrderCreated) is not captured, Wolverine
        // will automatically cascade the response to its handlers (if any).
        Result<OrderCreated> result = await _bus.InvokeAsync<Result<OrderCreated>>(command);

        return result;
    }

    [HttpPost("{orderId}/Ship")]
    public async Task<IActionResult> Ship(Guid orderId, [FromBody] ShipOrderRequest request)
    {
        ShipOrder command = new(orderId, request.CustomerId);
        string response = await _bus.InvokeAsync<string>(command);
        return Ok(response);
    }
}

public record CreateOrderRequest(decimal TotalAmount, string regionCode, Guid? CustomerId = null);
public record ShipOrderRequest(Guid CustomerId);
