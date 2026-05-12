using MediatR;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.ConsumptionLimits.Commands;

public record UpsertConsumptionLimitCommand(Guid UserId, Guid DeviceId, decimal LimitKwh) : IRequest<Result>;

internal sealed class UpsertConsumptionLimitHandler(IConsumptionLimitRepository repository)
    : IRequestHandler<UpsertConsumptionLimitCommand, Result>
{
    public async Task<Result> Handle(UpsertConsumptionLimitCommand request, CancellationToken ct)
    {
        if (request.LimitKwh <= 0)
            return Result.Failure("LimitKwh must be greater than zero.", ErrorType.Validation);

        await repository.UpsertAsync(
            new ConsumptionLimitSetting
            {
                UserId = request.UserId,
                DeviceId = request.DeviceId,
                LimitKwh = request.LimitKwh,
                LimitRsd = null
            },
            ct);

        return Result.Success();
    }
}
