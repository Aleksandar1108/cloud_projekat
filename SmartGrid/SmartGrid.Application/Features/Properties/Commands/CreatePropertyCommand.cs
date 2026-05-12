using FluentValidation;
using MediatR;
using SmartGrid.Application.Interfaces;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.Models;

namespace SmartGrid.Application.Features.Properties.Commands
{
    public record CreatePropertyCommand(
        Guid UserId,
        string Name,
        string City,
        string Address,
        string? Description,
        PropertyType PropertyType
    ) : IRequest<Result<PropertyDto>>;

    public class CreatePropertyValidator : AbstractValidator<CreatePropertyCommand>
    {
        public CreatePropertyValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.").MaximumLength(100);
            RuleFor(x => x.City).NotEmpty().WithMessage("City is required.").MaximumLength(100);
            RuleFor(x => x.Address).NotEmpty().WithMessage("Address is required.").MaximumLength(255);
            RuleFor(x => x.Description).MaximumLength(500).When(x => x.Description != null);
            RuleFor(x => x.PropertyType).IsInEnum().WithMessage("Invalid property type.");
        }
    }

    internal class CreatePropertyHandler(
        IPropertyRepository propertyRepository,
        IDateTimeProvider dateTimeProvider) : IRequestHandler<CreatePropertyCommand, Result<PropertyDto>>
    {
        public async Task<Result<PropertyDto>> Handle(CreatePropertyCommand request, CancellationToken ct)
        {
            var property = Property.Create(
                request.UserId,
                request.Name,
                request.City,
                request.Address,
                request.Description,
                request.PropertyType,
                dateTimeProvider.UtcNow
            );

            await propertyRepository.AddAsync(property, ct);

            return Result<PropertyDto>.Success(new PropertyDto(
                property.Id,
                property.UserId,
                property.Name,
                property.City,
                property.Address,
                property.Description,
                property.PropertyType,
                property.CreatedAt
            ));
        }
    }
}
