using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Features.Billing.Commands;

namespace SmartGrid.Functions.Processing
{
    internal class MonthlyBillingProcessor(ILogger<MonthlyBillingProcessor> logger, IMediator mediator)
    {
        [Function("MonthlyBillingProcessor")]
        public async Task Run([TimerTrigger("%MonthlyBillingSchedule%")] TimerInfo timerInfo)
        {
            var period = DateTime.UtcNow.AddMonths(-1);
            logger.LogInformation("[TIMER] Starting monthly billing for {Year}-{Month}.", period.Year, period.Month);

            var result = await mediator.Send(new RunMonthlyBillingCommand(period.Year, period.Month));

            if (result.IsFailure)
            {
                logger.LogError("[BILLING] Monthly billing failed: {Error}.", result.Error?.Message);
                return;
            }

            logger.LogInformation("[BILLING] Monthly billing finished for {Count} devices.", result.Value.Count);

            if (timerInfo.ScheduleStatus is not null)
            {
                logger.LogInformation("[TIMER] Next monthly billing schedule at: {Next}.", timerInfo.ScheduleStatus.Next);
            }
        }
    }
}
