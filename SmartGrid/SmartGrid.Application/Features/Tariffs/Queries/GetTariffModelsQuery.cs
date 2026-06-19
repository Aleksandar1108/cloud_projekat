using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.Tariffs.Queries
{
    public record GetTariffModelsQuery() : IRequest<Result<IReadOnlyList<TariffModelDto>>>;

    internal class GetTariffModelsHandler(
        ITariffModelRepository tariffModelRepository,
        ILogger<GetTariffModelsHandler> logger)
        : IRequestHandler<GetTariffModelsQuery, Result<IReadOnlyList<TariffModelDto>>>
    {
        public async Task<Result<IReadOnlyList<TariffModelDto>>> Handle(GetTariffModelsQuery request, CancellationToken ct)
        {
            try
            {
                var models = await tariffModelRepository.GetAllAsync(ct);
                return Result<IReadOnlyList<TariffModelDto>>.Success(models);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while loading tariff models.");
                return Result<IReadOnlyList<TariffModelDto>>.Failure("Failed to load tariff models.", ErrorType.Failure);
            }
        }
    }
}
