namespace SmartGrid.Application.Features.Tariffs.Commands
{
    internal static class TariffModelValidator
    {
        /// <summary>
        /// Returns an error message when invalid, or null when valid.
        /// </summary>
        public static string? Validate(string name, double greenZoneLimitKwh, double blueZoneLimitKwh)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "Tariff model name is required.";
            }

            if (greenZoneLimitKwh <= 0)
            {
                return "Green zone limit must be greater than 0.";
            }

            if (blueZoneLimitKwh <= greenZoneLimitKwh)
            {
                return "Blue zone limit must be greater than the green zone limit.";
            }

            return null;
        }
    }
}
