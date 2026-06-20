using MediatR;
using SmartGrid.Application.Features.Admin;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.Admin.Tariffs
{
    public record GetTariffModelQuery() : IRequest<Result<TariffModelDto>>;

    internal sealed class GetTariffModelHandler(ITariffModelRepository tariffModelRepository)
        : IRequestHandler<GetTariffModelQuery, Result<TariffModelDto>>
    {
        public async Task<Result<TariffModelDto>> Handle(GetTariffModelQuery request, CancellationToken ct)
        {
            try
            {
                var model = await tariffModelRepository.GetAdminModelAsync(ct);
                return Result<TariffModelDto>.Success(model);
            }
            catch (Exception ex)
            {
                return Result<TariffModelDto>.Failure(
                    $"Neuspesno ucitavanje tarifnog modela: {ex.Message}",
                    ErrorType.Unexpected);
            }
        }
    }

    public record SaveTariffModelCommand(TariffModelDto Model) : IRequest<Result<TariffModelDto>>;

    internal sealed class SaveTariffModelHandler(ITariffModelRepository tariffModelRepository)
        : IRequestHandler<SaveTariffModelCommand, Result<TariffModelDto>>
    {
        public async Task<Result<TariffModelDto>> Handle(SaveTariffModelCommand request, CancellationToken ct)
        {
            if (request.Model.ZoneThresholds.GreenMaxKwh <= 0 ||
                request.Model.ZoneThresholds.BlueMaxKwh <= request.Model.ZoneThresholds.GreenMaxKwh)
            {
                return Result<TariffModelDto>.Failure(
                    "Pragovi zona moraju biti pozitivni, a plava zona mora biti veca od zelene.",
                    ErrorType.Validation);
            }

            var saved = await tariffModelRepository.SaveAdminModelAsync(request.Model, ct);
            return Result<TariffModelDto>.Success(saved);
        }
    }
}
