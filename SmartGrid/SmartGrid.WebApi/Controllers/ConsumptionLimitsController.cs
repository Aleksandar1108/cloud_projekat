using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartGrid.Application.Features.ConsumptionLimits.Commands;
using SmartGrid.Application.Features.ConsumptionLimits.Queries;
using SmartGrid.WebApi.DTOs;
using SmartGrid.WebApi.Extensions;

namespace SmartGrid.WebApi.Controllers;

[Route("api/properties/{propertyId:guid}/smart-meters/{meterId:guid}/consumption-limit")]
[ApiController]
[Authorize]
public class ConsumptionLimitsController(IMediator mediator) : ControllerBase
{
    private Guid? GetCurrentUserId()
    {
        var claim = User.FindFirst("id")?.Value;
        return Guid.TryParse(claim, out var id) ? id : null;
    }

    [HttpGet]
    public async Task<IActionResult> Get(Guid propertyId, Guid meterId)
    {
        var userId = GetCurrentUserId();
        if (userId is null) return Unauthorized();

        var result = await mediator.Send(new GetConsumptionLimitQuery(propertyId, meterId, userId.Value));
        return result.ToActionResult();
    }

    [HttpPut]
    public async Task<IActionResult> Set(Guid propertyId, Guid meterId, [FromBody] SetConsumptionLimitRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId is null) return Unauthorized();

        var result = await mediator.Send(new SetConsumptionLimitCommand(
            propertyId,
            meterId,
            userId.Value,
            request.Unit,
            request.LimitValue));

        return result.ToActionResult();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(Guid propertyId, Guid meterId)
    {
        var userId = GetCurrentUserId();
        if (userId is null) return Unauthorized();

        var result = await mediator.Send(new DeleteConsumptionLimitCommand(propertyId, meterId, userId.Value));
        return result.ToActionResult();
    }
}
