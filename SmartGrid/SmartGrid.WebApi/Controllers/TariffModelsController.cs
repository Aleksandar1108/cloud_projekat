using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartGrid.Application.Features.Tariffs.Commands;
using SmartGrid.Application.Features.Tariffs.Queries;
using SmartGrid.WebApi.Authorization;
using SmartGrid.WebApi.DTOs;
using SmartGrid.WebApi.Extensions;

namespace SmartGrid.WebApi.Controllers
{
    [Route("api/tariff-models")]
    [ApiController]
    [Authorize(Roles = Roles.AnyAdmin)]
    public class TariffModelsController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await mediator.Send(new GetTariffModelsQuery(), ct);
            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TariffModelRequestDTO request, CancellationToken ct)
        {
            if (request is null)
            {
                return BadRequest(new { message = "Invalid or empty JSON payload." });
            }

            var command = new CreateTariffModelCommand(
                request.Name,
                request.IsActive,
                request.GreenZoneVtPrice,
                request.GreenZoneNtPrice,
                request.BlueZoneVtPrice,
                request.BlueZoneNtPrice,
                request.RedZoneVtPrice,
                request.RedZoneNtPrice,
                request.NetworkCostPerKw,
                request.SupplierCost,
                request.ApprovedPowerKw,
                request.GreenZoneLimitKwh,
                request.BlueZoneLimitKwh);

            var result = await mediator.Send(command, ct);
            return result.ToActionResult();
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] TariffModelRequestDTO request, CancellationToken ct)
        {
            if (request is null)
            {
                return BadRequest(new { message = "Invalid or empty JSON payload." });
            }

            var command = new UpdateTariffModelCommand(
                id,
                request.Name,
                request.GreenZoneVtPrice,
                request.GreenZoneNtPrice,
                request.BlueZoneVtPrice,
                request.BlueZoneNtPrice,
                request.RedZoneVtPrice,
                request.RedZoneNtPrice,
                request.NetworkCostPerKw,
                request.SupplierCost,
                request.ApprovedPowerKw,
                request.GreenZoneLimitKwh,
                request.BlueZoneLimitKwh);

            var result = await mediator.Send(command, ct);
            return result.ToActionResult();
        }

        [HttpPatch("{id:int}/activate")]
        public async Task<IActionResult> Activate(int id, CancellationToken ct)
        {
            var result = await mediator.Send(new ActivateTariffModelCommand(id), ct);
            return result.ToActionResult();
        }
    }
}
