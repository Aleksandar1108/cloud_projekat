using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.Tariffs.Commands
{
    public record UpdateTariffModelCommand(
        int Id,
        string Name,
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
    ) : IRequest<Result>;

    internal class UpdateTariffModelHandler(
        ITariffModelRepository tariffModelRepository,
        ILogger<UpdateTariffModelHandler> logger)
        : IRequestHandler<UpdateTariffModelCommand, Result>
    {
        public async Task<Result> Handle(UpdateTariffModelCommand request, CancellationToken ct)
        {
            var validation = TariffModelValidator.Validate(
                request.Name,
                request.GreenZoneLimitKwh,
                request.BlueZoneLimitKwh);

            if (validation is not null)
            {
                return Result.Failure(validation, ErrorType.Validation);
            }

            try
            {
                var existing = await tariffModelRepository.GetByIdAsync(request.Id, ct);
                if (existing is null)
                {
                    return Result.Failure("Tariff model not found.", ErrorType.NotFound);
                }

                var dto = existing with
                {
                    Name = request.Name.Trim(),
                    GreenZoneVtPrice = request.GreenZoneVtPrice,
                    GreenZoneNtPrice = request.GreenZoneNtPrice,
                    BlueZoneVtPrice = request.BlueZoneVtPrice,
                    BlueZoneNtPrice = request.BlueZoneNtPrice,
                    RedZoneVtPrice = request.RedZoneVtPrice,
                    RedZoneNtPrice = request.RedZoneNtPrice,
                    NetworkCostPerKw = request.NetworkCostPerKw,
                    SupplierCost = request.SupplierCost,
                    ApprovedPowerKw = request.ApprovedPowerKw,
                    GreenZoneLimitKwh = request.GreenZoneLimitKwh,
                    BlueZoneLimitKwh = request.BlueZoneLimitKwh
                };

                var updated = await tariffModelRepository.UpdateAsync(dto, ct);
                return updated
                    ? Result.Success()
                    : Result.Failure("Tariff model not found.", ErrorType.NotFound);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while updating tariff model {Id}.", request.Id);
                return Result.Failure("Failed to update tariff model.", ErrorType.Failure);
            }
        }
    }
}
