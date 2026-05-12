using SmartGrid.Domain.Enums;

namespace SmartGrid.Domain.Models
{
    public class Property
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string City { get; private set; } = string.Empty;
        public string Address { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public PropertyType PropertyType { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Property() { }

        public static Property Create(Guid userId, string name, string city, string address, string? description, PropertyType propertyType, DateTime createdAt)
        {
            return new Property
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = name,
                City = city,
                Address = address,
                Description = description,
                PropertyType = propertyType,
                CreatedAt = createdAt
            };
        }

        public static Property Load(Guid id, Guid userId, string name, string city, string address, string? description, PropertyType propertyType, DateTime createdAt)
        {
            return new Property
            {
                Id = id,
                UserId = userId,
                Name = name,
                City = city,
                Address = address,
                Description = description,
                PropertyType = propertyType,
                CreatedAt = createdAt
            };
        }

        public void Update(string name, string city, string address, string? description, PropertyType propertyType)
        {
            Name = name;
            City = city;
            Address = address;
            Description = description;
            PropertyType = propertyType;
        }
    }
}
