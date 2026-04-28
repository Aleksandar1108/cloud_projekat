using MediatR;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.ManualReadings.Queries
{
    public record GetManualReadingsQuery(ManualReadingStatus? Status) : IRequest<Result<IReadOnlyCollection<ManualReadingDto>>>;

    internal class GetManualReadingsHandler(IManualReadingRepository manualReadingRepository)
        : IRequestHandler<GetManualReadingsQuery, Result<IReadOnlyCollection<ManualReadingDto>>>
    {
        public async Task<Result<IReadOnlyCollection<ManualReadingDto>>> Handle(GetManualReadingsQuery request, CancellationToken ct)
        {
            var readings = await manualReadingRepository.GetAllAsync(request.Status, ct);
            return Result<IReadOnlyCollection<ManualReadingDto>>.Success(readings);
        }
    }
}
