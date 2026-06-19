using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartGrid.Application.Features.Billing.Commands;
using SmartGrid.Application.Features.Billing.Queries;
using SmartGrid.WebApi.Authorization;
using SmartGrid.WebApi.Extensions;

namespace SmartGrid.WebApi.Controllers
{
    [Route("api/monthly-billing")]
    [ApiController]
    [Authorize]
    public class MonthlyBillingController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetMonthlyBills([FromQuery] int? year, [FromQuery] int? month)
        {
            var now = DateTime.UtcNow;
            var targetYear = year ?? now.Year;
            var targetMonth = month ?? now.Month;

            var result = await mediator.Send(new GetMonthlyBillsQuery(targetYear, targetMonth));
            return result.ToActionResult();
        }

        [Authorize(Roles = Roles.AnyAdmin)]
        [HttpPost("run")]
        public async Task<IActionResult> RunMonthlyBilling([FromQuery] int? year, [FromQuery] int? month)
        {
            var now = DateTime.UtcNow;
            var targetYear = year ?? now.Year;
            var targetMonth = month ?? now.Month;

            var result = await mediator.Send(new RunMonthlyBillingCommand(targetYear, targetMonth));
            return result.ToActionResult();
        }

        [Authorize(Roles = Roles.AnyAdmin)]
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats([FromQuery] int? year, [FromQuery] int? month)
        {
            var now = DateTime.UtcNow;
            var targetYear = year ?? now.Year;
            var targetMonth = month ?? now.Month;

            var result = await mediator.Send(new GetBillingStatsQuery(targetYear, targetMonth));
            return result.ToActionResult();
        }
    }
}
