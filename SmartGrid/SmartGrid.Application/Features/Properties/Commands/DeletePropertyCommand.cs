using MediatR;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.Properties.Commands
{
    public record DeletePropertyCommand(Guid Id, Guid UserId) : IRequest<Result>;

    internal class DeletePropertyHandler(
        IPropertyRepository propertyRepository) : IRequestHandler<DeletePropertyCommand, Result>
    {
        public async Task<Result> Handle(DeletePropertyCommand request, CancellationToken ct)
        {
            var property = await propertyRepository.GetByIdAsync(request.Id, ct);
            if (property is null)
                return Result.Failure("Property not found.", ErrorType.NotFound);

            if (property.UserId != request.UserId)
                return Result.Failure("Access denied.", ErrorType.Unauthorized);

            await propertyRepository.DeleteAsync(request.Id, ct);
            return Result.Success();
        }
    }
}
