using SmartGrid.Domain.Enums;
using SmartGrid.Domain.Models;
using SmartGrid.Infrastructure.Persistence.SQLDatabase.Common;
using SmartGrid.Infrastructure.Persistence.SQLDatabase.Entities;

namespace SmartGrid.Infrastructure.Persistence.SQLDatabase.Mappers
{
    public class PropertyMapper : IDatabaseMapper<Property, PropertyEntity>
    {
        public Property? ToDomain(PropertyEntity entity)
        {
            return Property.Load(
                entity.Id,
                entity.UserId,
                entity.Name,
                entity.City,
                entity.Address,
                entity.Description,
                Enum.Parse<PropertyType>(entity.PropertyType),
                entity.CreatedAt
            );
        }

        public PropertyEntity ToEntity(Property domain)
        {
            return new PropertyEntity
            {
                Id = domain.Id,
                UserId = domain.UserId,
                Name = domain.Name,
                City = domain.City,
                Address = domain.Address,
                Description = domain.Description,
                PropertyType = domain.PropertyType.ToString(),
                CreatedAt = domain.CreatedAt
            };
        }
    }
}
