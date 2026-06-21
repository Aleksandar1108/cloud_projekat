using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Common;
using SmartGrid.Application.Features.Billing.Commands;
using SmartGrid.Application.Interfaces;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Application.Interfaces.Storage;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.ValueObjects.User;
using System.Net;
using System.Text;

namespace SmartGrid.Application.Features.ManualReadings.Commands
{
    public record ApproveManualReadingCommand(Guid ReadingId) : IRequest<Result<MonthlyBillDto>>;

    internal sealed class ApproveManualReadingHandler(
        IManualReadingRepository manualReadingRepository,
        ITariffModelRepository tariffModelRepository,
        IMonthlyBillRepository monthlyBillRepository,
        IMonthlyBillTextStorage monthlyBillTextStorage,
        ISmartMeterRepository smartMeterRepository,
        IPropertyRepository propertyRepository,
        IUserRepository userRepository,
        IEmailService emailService,
        ILogger<ApproveManualReadingHandler> logger)
        : IRequestHandler<ApproveManualReadingCommand, Result<MonthlyBillDto>>
    {
        public async Task<Result<MonthlyBillDto>> Handle(ApproveManualReadingCommand request, CancellationToken ct)
        {
            var reading = await manualReadingRepository.GetByIdAsync(request.ReadingId, ct);
            if (reading is null || reading.Status != ManualReadingStatus.Pending)
            {
                return Result<MonthlyBillDto>.Failure("Manual reading not found.", ErrorType.NotFound);
            }

            var tariffModel = await tariffModelRepository.GetActiveAsync(ct);
            if (tariffModel is null)
            {
                return Result<MonthlyBillDto>.Failure(
                    "Nema aktivnog tarifnog modela u bazi.",
                    ErrorType.NotFound);
            }

            var year = reading.ReadingAtUtc.Year;
            var month = reading.ReadingAtUtc.Month;
            var higherKwh = reading.ReadingAtUtc.Hour >= 7 && reading.ReadingAtUtc.Hour < 23
                ? reading.ReadingKwh
                : 0.0;
            var lowerKwh = higherKwh > 0 ? 0.0 : reading.ReadingKwh;

            var bill = MonthlyBillGenerator.BuildBill(
                reading.DeviceId,
                year,
                month,
                higherKwh,
                lowerKwh,
                tariffModel);

            var processed = await manualReadingRepository.MarkProcessedAsync(request.ReadingId, ct);
            if (!processed)
            {
                return Result<MonthlyBillDto>.Failure("Manual reading not found.", ErrorType.NotFound);
            }

            try
            {
                await monthlyBillTextStorage.SaveAsync(new FileData<MonthlyBillTextMetadata>
                {
                    Content = MonthlyBillGenerator.GeneratePdfBytes(bill),
                    Metadata = new MonthlyBillTextMetadata
                    {
                        Year = year,
                        Month = month,
                        DeviceId = reading.DeviceId
                    }
                }, ct);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "PDF racuna nije sacuvan za uredjaj {DeviceId}.", reading.DeviceId);
            }

            await monthlyBillRepository.SaveOrUpdateAsync(bill, ct);
            await SendBillEmailAsync(reading, bill, year, month, ct);

            logger.LogInformation(
                "Manual reading {ReadingId} approved and bill generated for device {DeviceId}.",
                request.ReadingId,
                reading.DeviceId);

            return Result<MonthlyBillDto>.Success(bill);
        }

        private async Task SendBillEmailAsync(
            ManualReadingDto reading,
            MonthlyBillDto bill,
            int year,
            int month,
            CancellationToken ct)
        {
            try
            {
                var meter = await smartMeterRepository.GetByDeviceUUIDAsync(reading.DeviceId, ct);
                if (meter is null)
                {
                    return;
                }

                var property = await propertyRepository.GetByIdAsync(meter.PropertyId, ct);
                if (property is null)
                {
                    return;
                }

                var owner = await userRepository.GetByIdAsync(UserId.FromGuid(property.UserId), ct);
                if (owner is null)
                {
                    return;
                }

                var textBody = new StringBuilder()
                    .AppendLine("Postovani,")
                    .AppendLine()
                    .AppendLine("Vas rucni unos ocitavanja je odobren. U prilogu je mesecni racun:")
                    .AppendLine()
                    .Append(bill.BillText)
                    .ToString();

                var htmlBody = new StringBuilder()
                    .AppendLine("<p>Postovani,</p>")
                    .AppendLine("<p>Vas rucni unos ocitavanja je odobren. U prilogu je mesecni racun:</p>")
                    .Append("<pre style=\"font-family: monospace; white-space: pre-wrap;\">")
                    .Append(WebUtility.HtmlEncode(bill.BillText))
                    .AppendLine("</pre>")
                    .ToString();

                await emailService.SendEmailAsync(
                    owner.Email.Value,
                    $"SmartGrid - Mesecni racun {year:D4}-{month:D2}",
                    textBody,
                    htmlBody,
                    ct);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Bill was generated, but email sending failed for reading {ReadingId}.", reading.Id);
            }
        }
    }
}
