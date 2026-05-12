using Microsoft.EntityFrameworkCore;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Models;
using SmartGrid.Infrastructure.Persistence.SQLDatabase.Common;
using SmartGrid.Infrastructure.Persistence.SQLDatabase.Entities;

namespace SmartGrid.Infrastructure.Persistence.SQLDatabase.Repositories
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly SmartGridDbContext _context;
        private readonly IDatabaseMapper<Property, PropertyEntity> _mapper;

        public PropertyRepository(SmartGridDbContext context, IDatabaseMapper<Property, PropertyEntity> mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<Property>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            var entities = await _context.Properties
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(ct);

            return entities.Select(_mapper.ToDomain).Where(x => x is not null).Select(x => x!).ToList();
        }

        public async Task<Property?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var entity = await _context.Properties
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            return entity is null ? null : _mapper.ToDomain(entity);
        }

        public async Task AddAsync(Property property, CancellationToken ct = default)
        {
            var entity = _mapper.ToEntity(property);
            await _context.Properties.AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Property property, CancellationToken ct = default)
        {
            var entity = await _context.Properties.FirstOrDefaultAsync(x => x.Id == property.Id, ct);
            if (entity is null) return;

            entity.Name = property.Name;
            entity.City = property.City;
            entity.Address = property.Address;
            entity.Description = property.Description;
            entity.PropertyType = property.PropertyType.ToString();

            _context.Properties.Update(entity);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var entity = await _context.Properties.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (entity is null) return;

            _context.Properties.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }
    }
}
