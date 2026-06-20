namespace SmartGrid.Application.Features.Admin
{
    public record TariffModelDto(
        string Id,
        string Name,
        string EffectiveFrom,
        TariffZoneThresholdsDto ZoneThresholds,
        TariffZonePricesDto ZonePrices,
        TariffFixedCostsDto FixedCosts,
        string UpdatedAtUtc
    );

    public record TariffZoneThresholdsDto(double GreenMaxKwh, double BlueMaxKwh);

    public record TariffZonePriceDto(double VtPriceRsd, double NtPriceRsd);

    public record TariffZonePricesDto(
        TariffZonePriceDto Green,
        TariffZonePriceDto Blue,
        TariffZonePriceDto Red
    );

    public record TariffFixedCostsDto(double BillingPowerFeeRsd, double SupplierCostRsd);

    public record AdminMeterNetworkStatusDto(
        string DeviceId,
        string SerialNumber,
        string OwnerName,
        string OwnerEmail,
        string PropertyAddress,
        string ConnectionStatus,
        DateTime? LastSeenAtUtc,
        double CurrentMonthKwh,
        string LastBillStatus,
        double ApprovedPowerKw
    );

    public record AdminPaymentDto(
        string Id,
        string DeviceId,
        string OwnerName,
        string OwnerEmail,
        string InvoiceId,
        string Period,
        double AmountRsd,
        string PaidAtUtc,
        string PaymentMethod,
        string Status
    );

    public record AdminGeneratedBillDto(
        string InvoiceId,
        string DeviceId,
        string OwnerName,
        string OwnerEmail,
        int Year,
        int Month,
        double VtKwh,
        double NtKwh,
        double TotalKwh,
        double GreenZoneKwh,
        double BlueZoneKwh,
        double RedZoneKwh,
        double EnergyCostRsd,
        double FixedCostsRsd,
        double TotalCostRsd,
        string BillText,
        bool EmailSent
    );

    public record BillingRunDto(
        string Id,
        int Year,
        int Month,
        string StartedAtUtc,
        string CompletedAtUtc,
        string Status,
        int ProcessedMeters,
        int GeneratedBills,
        int EmailsSent,
        IReadOnlyCollection<AdminGeneratedBillDto> Bills
    );

    public record BillingDeliveryStatsDto(
        int TotalGenerated,
        int TotalSent,
        int TotalFailed,
        string? LastRunAtUtc,
        int DeliveryRatePercent
    );
}
