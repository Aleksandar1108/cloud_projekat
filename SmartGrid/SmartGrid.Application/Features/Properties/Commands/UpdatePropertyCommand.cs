using FluentValidation;
using MediatR;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.Properties.Commands
{
    public record UpdatePropertyCommand(
        Guid Id,
        Guid UserId,
        string Name,
        string City,
        string Address,
        string? Description,
        PropertyType PropertyType
    ) : IRequest<Result>;

    public class UpdatePropertyValidator : AbstractValidator<UpdatePropertyCommand>
    {
        public UpdatePropertyValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.City).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Address).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Description).MaximumLength(500).When(x => x.Description != null);
            RuleFor(x => x.PropertyType).IsInEnum();
        }
    }

    internal class UpdatePropertyHandler(
        IPropertyRepository propertyRepository) : IRequestHandler<UpdatePropertyCommand, Result>
    {
        public async Task<Result> Handle(UpdatePropertyCommand request, CancellationToken ct)
        {
            var property = await propertyRepository.GetByIdAsync(request.Id, ct);
            if (property is null)
                return Result.Failure("Property not found.", ErrorType.NotFound);

            if (property.UserId != request.UserId)
                return Result.Failure("Access denied.", ErrorType.Unauthorized);

            property.Update(request.Name, request.City, request.Address, request.Description, request.PropertyType);
            await propertyRepository.UpdateAsync(property, ct);

            return Result.Success();
        }
    }
}
