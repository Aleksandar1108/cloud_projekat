using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.SmartMeters.Queries
{

    public record GetAllSmartMetersQuery() : IRequest<Result<IEnumerable<SmartMeterDto>>>;

        internal class GetAllSmartMetersHandler(ISmartMeterRepository smartMeterRepository, ILogger<GetAllSmartMetersHandler> logger)
    : IRequestHandler<GetAllSmartMetersQuery, Result<IEnumerable<SmartMeterDto>>>
    {
        public async Task<Result<IEnumerable<SmartMeterDto>>> Handle(
            GetAllSmartMetersQuery request,
            CancellationToken ct)
        {
            try
            {
                var smartMeters = await smartMeterRepository.GetAllPairedAsync(ct);

                var result = smartMeters.Select(x => new SmartMeterDto(
                    x.Id,
                    x.PropertyId,
                    x.Label,
                    x.ConnectionType,
                    x.MaxApprovedPower,
                    x.Note,
                    x.SerialNumber,
                    x.PairingStatus,
                    x.DeviceUUID,
                    x.CreatedAt
                ));

                return Result<IEnumerable<SmartMeterDto>>
                    .Success(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Error while retrieving paired smart meters.");

                return Result<IEnumerable<SmartMeterDto>>
                    .Failure(
                        "Failed to retrieve paired smart meters.",
                        ErrorType.Failure);
            }
        }
    }
    
}