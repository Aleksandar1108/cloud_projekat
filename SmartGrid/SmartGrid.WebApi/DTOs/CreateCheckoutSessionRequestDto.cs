namespace SmartGrid.WebApi.DTOs
{
    public record CreateCheckoutSessionRequestDto(
        string DeviceId,
        int Year,
        int Month
    );
}

