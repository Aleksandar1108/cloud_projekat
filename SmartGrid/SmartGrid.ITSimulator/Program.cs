using Microsoft.Extensions.Configuration;
using SmartGrid.ITSimulator.Enums;
using SmartGrid.ITSimulator.Services;
using System.Diagnostics.Metrics;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var baseApiUrl = configuration["SimulatorSettings:BaseApiUrl"]
    ?? throw new InvalidOperationException("Device URL is not configured");

var delayMs = int.Parse(
    configuration["SimulatorSettings:DelayMilliseconds"] ?? "10000");

var maxVariation = double.Parse(
    configuration["SimulatorSettings:MaxPowerVariation"] ?? "50");



using var httpClient = new HttpClient
{
    BaseAddress = new Uri(baseApiUrl)
};

var simulator = new SimulatorService(maxVariation);
var publisher = new TelemetryPublisher(httpClient);

var smartMeterClient = new SmartMeterClient(httpClient);


var smartMeters = await smartMeterClient.GetAllAsync();

if (!smartMeters.Any())
{
    Console.WriteLine("No paired smart meters found.");
    return;
}

string currentVersion = "v1.0.0";

Console.WriteLine( $"Loaded {smartMeters.Count} paired smart meters.");

try
{
    while (true)
    {
        foreach(var meter in smartMeters)
        {
            if (string.IsNullOrWhiteSpace(meter.DeviceUUID))
            {
                continue;
            }
            var deviceType =meter.ConnectionType == "Trofazni"? DeviceType.Trofazni: DeviceType.Monofazni;

            var telemetry = simulator.GenerateTelemetry(
                meter.DeviceUUID,
                meter.Label,
                meter.MaxApprovedPower,
                currentVersion,
                deviceType);

            var (success, errorMessage) =
                await publisher.PublishSafeAsync(telemetry);
            if (success)
            {
                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine("Success" + telemetry.CurrentPower + " " + telemetry.NominalPower);

                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine(errorMessage ?? "Unknown error");

                Console.ResetColor();
            }

            await Task.Delay(delayMs);
        }
    }
}
catch (OperationCanceledException)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("\n[SYSTEM] Simulation stopped by user.");
    Console.ResetColor();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Fatal error: {ex.Message}");
    Console.ResetColor();
    Environment.Exit(1);
}