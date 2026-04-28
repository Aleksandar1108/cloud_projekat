using MediatR;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Application.Interfaces.Repositories;

namespace SmartGrid.Application.Features.ManualReadings.Commands
{
    public record ApproveManualReadingCommand(Guid ReadingId) : IRequest<Result>;

    internal class ApproveManualReadingHandler(IManualReadingRepository manualReadingRepository)
        : IRequestHandler<ApproveManualReadingCommand, Result>
    {
        public async Task<Result> Handle(ApproveManualReadingCommand request, CancellationToken ct)
        {
            var ok = await manualReadingRepository.MarkProcessedAsync(request.ReadingId, ct);
            if (!ok)
            {
                return Result.Failure("Manual reading not found.", ErrorType.NotFound);
            }

            return Result.Success();
        }
    }
}
