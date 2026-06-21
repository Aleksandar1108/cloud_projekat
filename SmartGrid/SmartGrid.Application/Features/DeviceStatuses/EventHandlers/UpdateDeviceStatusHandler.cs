using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Features.Telemetries.Events;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Models;

namespace SmartGrid.Application.Features.DeviceStatuses.EventHandlers;

internal class UpdateDeviceStatusHandler(
        IDeviceRepository deviceRepository,
        ILogger<UpdateDeviceStatusHandler> logger) : INotificationHandler<TelemetryProcessedEvent>
{
    public async Task Handle(TelemetryProcessedEvent notification, CancellationToken ct)
    {
        var telemetry = notification.Telemetry;

        try
        {
            var device = await deviceRepository.GetWithStatusByIdAsync(
                telemetry.DeviceType,
                telemetry.DeviceId,
                ct);

            DeviceStatus status;
            if (device is null)
            {
                status = DeviceStatus.CreateDefault(telemetry.DeviceId, telemetry.DeviceType, telemetry.Timestamp);
            }
            else
            {
                status = device.Status;
            }

            status.UpdateTelemetry(telemetry);

            await deviceRepository.SaveStatusAsync(status, ct);

            logger.LogInformation("Successfully updated status for device {DeviceId}.", telemetry.DeviceId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error asynchronously updating status for device {DeviceId}", telemetry.DeviceId);
        }
    }
}