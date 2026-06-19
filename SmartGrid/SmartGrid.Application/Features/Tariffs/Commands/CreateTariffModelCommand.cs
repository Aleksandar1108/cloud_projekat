using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.Tariffs.Commands
{
    public record CreateTariffModelCommand(
        string Name,
        bool IsActive,
        double GreenZoneVtPrice,
        double GreenZoneNtPrice,
        double BlueZoneVtPrice,
        double BlueZoneNtPrice,
        double RedZoneVtPrice,
        double RedZoneNtPrice,
        double NetworkCostPerKw,
        double SupplierCost,
        double ApprovedPowerKw,
        double GreenZoneLimitKwh,
        double BlueZoneLimitKwh
    ) : IRequest<Result<int>>;

    internal class CreateTariffModelHandler(
        ITariffModelRepository tariffModelRepository,
        ILogger<CreateTariffModelHandler> logger)
        : IRequestHandler<CreateTariffModelCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(CreateTariffModelCommand request, CancellationToken ct)
        {
            var validation = TariffModelValidator.Validate(
                request.Name,
                request.GreenZoneLimitKwh,
                request.BlueZoneLimitKwh);

            if (validation is not null)
            {
                return Result<int>.Failure(validation, ErrorType.Validation);
            }

            try
            {
                var dto = new TariffModelDto(
                    0,
                    request.Name.Trim(),
                    request.IsActive,
                    DateTime.UtcNow,
                    request.GreenZoneVtPrice,
                    request.GreenZoneNtPrice,
                    request.BlueZoneVtPrice,
                    request.BlueZoneNtPrice,
                    request.RedZoneVtPrice,
                    request.RedZoneNtPrice,
                    request.NetworkCostPerKw,
                    request.SupplierCost,
                    request.ApprovedPowerKw,
                    request.GreenZoneLimitKwh,
                    request.BlueZoneLimitKwh);

                var id = await tariffModelRepository.CreateAsync(dto, ct);
                return Result<int>.Success(id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while creating tariff model.");
                return Result<int>.Failure("Failed to create tariff model.", ErrorType.Failure);
            }
        }
    }
}
