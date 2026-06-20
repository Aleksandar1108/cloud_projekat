using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Features.Admin;
using SmartGrid.Application.Interfaces;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Application.Features.Payments;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.Models;

namespace SmartGrid.Application.Features.Admin.Network
{
    public record GetAdminMeterNetworkQuery() : IRequest<Result<IReadOnlyCollection<AdminMeterNetworkStatusDto>>>;

    internal sealed class GetAdminMeterNetworkHandler(
        ISmartMeterRepository smartMeterRepository,
        IPropertyRepository propertyRepository,
        IUserRepository userRepository,
        IDeviceStatusQueryRepository deviceStatusQueryRepository,
        IMonthlyBillRepository monthlyBillRepository,
        IPaymentRepository paymentRepository,
        IDateTimeProvider dateTimeProvider,
        ILogger<GetAdminMeterNetworkHandler> logger)
        : IRequestHandler<GetAdminMeterNetworkQuery, Result<IReadOnlyCollection<AdminMeterNetworkStatusDto>>>
    {
        public async Task<Result<IReadOnlyCollection<AdminMeterNetworkStatusDto>>> Handle(
            GetAdminMeterNetworkQuery request,
            CancellationToken ct)
        {
            try
            {
                var now = dateTimeProvider.UtcNow;
                var meters = await smartMeterRepository.GetAllPairedAsync(ct);
                var users = (await userRepository.GetAllAsync(ct)).ToDictionary(x => x.Id.Value);
                var statuses = (await deviceStatusQueryRepository.GetAllAsync(ct))
                    .ToDictionary(x => x.DeviceId.Value, StringComparer.OrdinalIgnoreCase);

                var previousMonth = now.AddMonths(-1);
                var previousBills = await monthlyBillRepository.GetByPeriodAsync(previousMonth.Year, previousMonth.Month, ct);
                var previousBillByDevice = previousBills.ToDictionary(x => x.DeviceId, StringComparer.OrdinalIgnoreCase);
                var currentBills = await monthlyBillRepository.GetByPeriodAsync(now.Year, now.Month, ct);
                var currentBillByDevice = currentBills.ToDictionary(x => x.DeviceId, StringComparer.OrdinalIgnoreCase);

                var results = new List<AdminMeterNetworkStatusDto>();

                foreach (var meter in meters)
                {
                    if (string.IsNullOrWhiteSpace(meter.DeviceUUID))
                    {
                        continue;
                    }

                    var property = await propertyRepository.GetByIdAsync(meter.PropertyId, ct);
                    if (property is null)
                    {
                        continue;
                    }

                    users.TryGetValue(property.UserId, out var owner);
                    statuses.TryGetValue(meter.DeviceUUID, out var status);
                    currentBillByDevice.TryGetValue(meter.DeviceUUID, out var currentBill);

                    var lastBillStatus = "Nema racuna";
                    if (previousBillByDevice.TryGetValue(meter.DeviceUUID, out var previousBill))
                    {
                        var payment = await paymentRepository.GetByBillAsync(
                            meter.DeviceUUID,
                            previousBill.Year,
                            previousBill.Month,
                            ct);

                        lastBillStatus = payment?.Status == PaymentStatus.Paid ? "Placen" : "Neplacen";
                    }

                    results.Add(new AdminMeterNetworkStatusDto(
                        meter.DeviceUUID,
                        meter.SerialNumber ?? "N/A",
                        AdminOwnerResolver.ResolveName(owner),
                        owner?.Email.Value ?? "N/A",
                        $"{property.Address}, {property.City}",
                        status is not null && status.IsOnline(now) ? "Online" : "Offline",
                        status?.LastHeartbeat,
                        currentBill?.TotalKwh ?? 0,
                        lastBillStatus,
                        meter.MaxApprovedPower));
                }

                return Result<IReadOnlyCollection<AdminMeterNetworkStatusDto>>.Success(results);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to load admin meter network status.");
                return Result<IReadOnlyCollection<AdminMeterNetworkStatusDto>>.Failure(
                    "Failed to load meter network status.",
                    ErrorType.Failure);
            }
        }
    }
}
