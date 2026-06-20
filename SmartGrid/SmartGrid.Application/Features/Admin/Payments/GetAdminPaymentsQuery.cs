using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Features.Admin;
using SmartGrid.Application.Features.Payments;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.Admin.Payments
{
    public record GetAdminPaymentsQuery() : IRequest<Result<IReadOnlyCollection<AdminPaymentDto>>>;

    internal sealed class GetAdminPaymentsHandler(
        IPaymentRepository paymentRepository,
        ISmartMeterRepository smartMeterRepository,
        IPropertyRepository propertyRepository,
        IUserRepository userRepository,
        ILogger<GetAdminPaymentsHandler> logger)
        : IRequestHandler<GetAdminPaymentsQuery, Result<IReadOnlyCollection<AdminPaymentDto>>>
    {
        public async Task<Result<IReadOnlyCollection<AdminPaymentDto>>> Handle(
            GetAdminPaymentsQuery request,
            CancellationToken ct)
        {
            try
            {
                var payments = await paymentRepository.GetAllPaidAsync(ct);
                var meters = await smartMeterRepository.GetAllPairedAsync(ct);
                var meterByDevice = meters
                    .Where(x => !string.IsNullOrWhiteSpace(x.DeviceUUID))
                    .ToDictionary(x => x.DeviceUUID!, StringComparer.OrdinalIgnoreCase);

                var users = (await userRepository.GetAllAsync(ct)).ToDictionary(x => x.Id.Value);
                var results = new List<AdminPaymentDto>();

                foreach (var payment in payments)
                {
                    meterByDevice.TryGetValue(payment.DeviceId, out var meter);
                    var ownerEmail = "N/A";
                    var ownerName = "Nepoznat korisnik";

                    if (meter is not null)
                    {
                        var property = await propertyRepository.GetByIdAsync(meter.PropertyId, ct);
                        if (property is not null && users.TryGetValue(property.UserId, out var owner))
                        {
                            ownerEmail = owner.Email.Value;
                            ownerName = AdminOwnerResolver.ResolveName(owner);
                        }
                    }

                    results.Add(new AdminPaymentDto(
                        $"{payment.DeviceId}-{payment.Year:D4}-{payment.Month:D2}",
                        payment.DeviceId,
                        ownerName,
                        ownerEmail,
                        $"INV-{payment.Year:D4}-{payment.Month:D2}-{payment.DeviceId[..Math.Min(8, payment.DeviceId.Length)]}",
                        $"{payment.Year:D4}-{payment.Month:D2}",
                        payment.AmountMinor / 100.0,
                        payment.PaidAtUtc!.Value.ToString("o"),
                        "Stripe",
                        "Realizovana"));
                }

                return Result<IReadOnlyCollection<AdminPaymentDto>>.Success(results);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to load admin payments.");
                return Result<IReadOnlyCollection<AdminPaymentDto>>.Failure(
                    "Failed to load payments.",
                    ErrorType.Failure);
            }
        }
    }
}
