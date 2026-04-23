using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartGrid.Application.Features.Billing.Commands;
using SmartGrid.Application.Features.Billing.Queries;
using SmartGrid.WebApi.Extensions;

namespace SmartGrid.WebApi.Controllers
{
    [Route("api/monthly-billing")]
    [ApiController]
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

        [HttpPost("run")]
        public async Task<IActionResult> RunMonthlyBilling([FromQuery] int? year, [FromQuery] int? month)
        {
            var now = DateTime.UtcNow;
            var targetYear = year ?? now.Year;
            var targetMonth = month ?? now.Month;

            var result = await mediator.Send(new RunMonthlyBillingCommand(targetYear, targetMonth));
            return result.ToActionResult();
        }
    }
}
