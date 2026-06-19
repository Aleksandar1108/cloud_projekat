using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.Tariffs.Commands
{
    public record ActivateTariffModelCommand(int Id) : IRequest<Result>;

    internal class ActivateTariffModelHandler(
        ITariffModelRepository tariffModelRepository,
        ILogger<ActivateTariffModelHandler> logger)
        : IRequestHandler<ActivateTariffModelCommand, Result>
    {
        public async Task<Result> Handle(ActivateTariffModelCommand request, CancellationToken ct)
        {
            try
            {
                var activated = await tariffModelRepository.SetActiveAsync(request.Id, ct);
                return activated
                    ? Result.Success()
                    : Result.Failure("Tariff model not found.", ErrorType.NotFound);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while activating tariff model {Id}.", request.Id);
                return Result.Failure("Failed to activate tariff model.", ErrorType.Failure);
            }
        }
    }
}
