using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartGrid.Application.Features.Admin;
using SmartGrid.Application.Features.Admin.Billing;
using SmartGrid.Application.Features.Admin.Network;
using SmartGrid.Application.Features.Admin.Payments;
using SmartGrid.Application.Features.Admin.Tariffs;
using SmartGrid.Application.Features.Billing.Commands;
using SmartGrid.WebApi.Extensions;

namespace SmartGrid.WebApi.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "Admin,SysAdmin")]
    public class AdminController(IMediator mediator) : ControllerBase
    {
        [HttpGet("tariffs")]
        public async Task<IActionResult> GetTariffModel()
        {
            var result = await mediator.Send(new GetTariffModelQuery());
            return result.ToActionResult();
        }

        [HttpPut("tariffs")]
        public async Task<IActionResult> SaveTariffModel([FromBody] TariffModelDto model)
        {
            var result = await mediator.Send(new SaveTariffModelCommand(model));
            return result.ToActionResult();
        }

        [HttpGet("network/meters")]
        public async Task<IActionResult> GetMeterNetworkStatus()
        {
            var result = await mediator.Send(new GetAdminMeterNetworkQuery());
            return result.ToActionResult();
        }

        [HttpGet("payments")]
        public async Task<IActionResult> GetPayments()
        {
            var result = await mediator.Send(new GetAdminPaymentsQuery());
            return result.ToActionResult();
        }

        [HttpGet("billing/runs")]
        public async Task<IActionResult> GetBillingRuns()
        {
            var result = await mediator.Send(new GetBillingRunsQuery());
            return result.ToActionResult();
        }

        [HttpPost("billing/run")]
        public async Task<IActionResult> RunMonthlyBilling([FromQuery] int? year, [FromQuery] int? month)
        {
            var now = DateTime.UtcNow;
            var targetYear = year ?? now.Year;
            var targetMonth = month ?? now.Month;

            var result = await mediator.Send(new RunMonthlyBillingCommand(targetYear, targetMonth));
            return result.ToActionResult();
        }

        [HttpGet("delivery/stats")]
        public async Task<IActionResult> GetDeliveryStats()
        {
            var result = await mediator.Send(new GetBillingDeliveryStatsQuery());
            return result.ToActionResult();
        }
    }
}
