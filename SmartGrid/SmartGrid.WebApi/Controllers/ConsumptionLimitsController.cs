using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartGrid.Application.Features.ConsumptionLimits.Commands;
using SmartGrid.Application.Features.ConsumptionLimits.Queries;
using SmartGrid.WebApi.DTOs;
using SmartGrid.WebApi.Extensions;

namespace SmartGrid.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ConsumptionLimitsController(IMediator mediator) : ControllerBase
{
    /// <summary>Pregled limita, mesečne potrošnje (kWh iz telemetrije) i statusa obaveštenja za korisnika.</summary>
    [HttpGet("summary/{userId:guid}")]
    public async Task<IActionResult> GetSummary(Guid userId)
    {
        var result = await mediator.Send(new GetConsumptionLimitSummariesQuery(userId));
        return result.ToActionResult();
    }

    /// <summary>Podešavanje mesečnog limita potrošnje (kWh) po brojilu — zahtev specifikacije (hitna upozorenja).</summary>
    [HttpPut]
    public async Task<IActionResult> Put([FromBody] SetConsumptionLimitRequest request)
    {
        var result = await mediator.Send(new UpsertConsumptionLimitCommand(request.UserId, request.DeviceId, request.LimitKwh));
        return result.ToActionResult();
    }
}
