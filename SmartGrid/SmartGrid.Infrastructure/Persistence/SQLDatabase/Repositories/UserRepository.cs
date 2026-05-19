using Microsoft.EntityFrameworkCore;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Models;
using SmartGrid.Domain.ValueObjects.User;
using SmartGrid.Infrastructure.Persistence.SQLDatabase.Common;
using SmartGrid.Infrastructure.Persistence.SQLDatabase.Entities;

namespace SmartGrid.Infrastructure.Persistence.SQLDatabase.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SmartGridDbContext _context;

        private readonly IDatabaseMapper<User, UserEntity> _userMapper;

        public UserRepository(SmartGridDbContext context, IDatabaseMapper<User, UserEntity> userMapper)
        {
            _context = context;
            _userMapper = userMapper;
        }

        public async Task<User?> GetByIdAsync(UserId id, CancellationToken ct)
        {
            var entity = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IdUsers == id.Value, ct);
            return entity is null ? null : _userMapper.ToDomain(entity);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
        {
            var entity = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == email, ct);
            return entity is null ? null : _userMapper.ToDomain(entity);
        }

        public async Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken ct)
        {
            var entities = await _context.Users
                .AsNoTracking()
                .ToListAsync(ct);   
            return entities.Select(_userMapper.ToDomain).ToList();
        }

        public async Task AddAsync(User user, CancellationToken ct)
        {
            var entity = _userMapper.ToEntity(user);

            await _context.Users.AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(User user, CancellationToken ct)
        {
            var entity = await _context.Users
                .FirstOrDefaultAsync(x => x.IdUsers == user.Id,ct);

            if (entity is null)
                return;

            entity.Email = user.Email.Value;
            entity.Password = user.Password.Value;
            entity.Role = user.Role.ToString();
            entity.AccountCreated = user.AccountCreated;
            entity.IsActive = user.ActivationStatus.Value;

            _context.Users.Update(entity);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(UserId id, CancellationToken ct)
        {
            var entity = await _context.Users
                .FirstOrDefaultAsync(x => x.IdUsers == id.Value, ct);

            if (entity is null)
                return;

            _context.Users.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }
    }
}