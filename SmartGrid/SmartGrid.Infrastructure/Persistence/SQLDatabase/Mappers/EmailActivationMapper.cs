using SmartGrid.Domain.Models;
using SmartGrid.Domain.ValueObjects.User;
using SmartGrid.Infrastructure.Persistence.SQLDatabase.Common;
using SmartGrid.Infrastructure.Persistence.SQLDatabase.Entities;

namespace SmartGrid.Infrastructure.Persistence.SQLDatabase.Mappers
{
    public class EmailActivationMapper : IDatabaseMapper<EmailActivation, EmailActivationEntity>
    {
        public EmailActivation? ToDomain(EmailActivationEntity entity)
        {
            return new EmailActivation(
             EmailActivationId.FromGuid(entity.IdEmailActivation),
             UserId.FromGuid(entity.UserId),
             ActivationToken.FromString(entity.ActivationToken),
             entity.CreatedAt,
             entity.ExpirationDate
            );
        }

        public EmailActivationEntity ToEntity(EmailActivation domain)
        {
            return new EmailActivationEntity
            {
                IdEmailActivation = domain.Id.Value,
                UserId = domain.UserId.Value,
                ActivationToken = domain.Token.Value,
                CreatedAt = domain.CreatedAt,
                ExpirationDate = domain.ExpiresAt
            };

        }
    }
}
