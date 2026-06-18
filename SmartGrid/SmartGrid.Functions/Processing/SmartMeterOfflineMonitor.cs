using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Features.Alerts.Commands;

namespace SmartGrid.Functions.Processing;

internal class SmartMeterOfflineMonitor(ILogger<SmartMeterOfflineMonitor> logger, IMediator mediator)
{
    [Function("SmartMeterOfflineMonitor")]
    public async Task Run([TimerTrigger("%SmartMeterOfflineMonitorSchedule%")] TimerInfo timerInfo)
    {
        logger.LogInformation("[TIMER] Starting smart meter offline monitoring cycle...");

        var result = await mediator.Send(new CheckSmartMeterOfflineCommand());
        if (result.IsFailure)
        {
            logger.LogError("[ALERT] Smart meter offline monitoring failed: {Error}", result.Error?.Message);
        }
        else
        {
            logger.LogInformation("[ALERT] Smart meter offline monitoring cycle completed.");
        }

        if (timerInfo.ScheduleStatus is not null)
        {
            logger.LogInformation("[TIMER] Next offline monitor schedule at: {NextSchedule}", timerInfo.ScheduleStatus.Next);
        }
    }
}
