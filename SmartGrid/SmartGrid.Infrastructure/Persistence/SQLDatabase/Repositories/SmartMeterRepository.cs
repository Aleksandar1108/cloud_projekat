using Microsoft.EntityFrameworkCore;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.Models;
using SmartGrid.Infrastructure.Persistence.SQLDatabase.Common;
using SmartGrid.Infrastructure.Persistence.SQLDatabase.Entities;

namespace SmartGrid.Infrastructure.Persistence.SQLDatabase.Repositories
{
    public class SmartMeterRepository : ISmartMeterRepository
    {
        private readonly SmartGridDbContext _context;
        private readonly IDatabaseMapper<SmartMeter, SmartMeterEntity> _mapper;

        public SmartMeterRepository(SmartGridDbContext context, IDatabaseMapper<SmartMeter, SmartMeterEntity> mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<SmartMeter>> GetByPropertyIdAsync(Guid propertyId, CancellationToken ct = default)
        {
            var entities = await _context.SmartMeters
                .AsNoTracking()
                .Where(x => x.PropertyId == propertyId)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync(ct);

            return entities.Select(_mapper.ToDomain).Where(x => x is not null).Select(x => x!).ToList();
        }

        public async Task<SmartMeter?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var entity = await _context.SmartMeters
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            return entity is null ? null : _mapper.ToDomain(entity);
        }

        public async Task<SmartMeter?> GetBySerialNumberAsync(string serialNumber, CancellationToken ct = default)
        {
            var entity = await _context.SmartMeters
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.SerialNumber == serialNumber, ct);

            return entity is null ? null : _mapper.ToDomain(entity);
        }

        public async Task<SmartMeter?> GetByDeviceUUIDAsync(string deviceUUID, CancellationToken ct = default)
        {
            var entity = await _context.SmartMeters
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.DeviceUUID == deviceUUID, ct);

            return entity is null ? null : _mapper.ToDomain(entity);
        }

        public async Task<IReadOnlyCollection<SmartMeter>> GetAllPairedAsync(CancellationToken ct = default)
        {
            var pairedStatus = PairingStatus.Paired.ToString();
            var entities = await _context.SmartMeters
                .AsNoTracking()
                .Where(x => x.PairingStatus == pairedStatus && x.DeviceUUID != null)
                .ToListAsync(ct);

            return entities.Select(_mapper.ToDomain).Where(x => x is not null).Select(x => x!).ToList();
        }

        public async Task<IReadOnlyCollection<SmartMeter>> GetAllAsync(CancellationToken ct = default)
        {
            var entities = await _context.SmartMeters
                .AsNoTracking()
                .OrderBy(x => x.CreatedAt)
                .ToListAsync(ct);

            return entities.Select(_mapper.ToDomain).Where(x => x is not null).Select(x => x!).ToList();
        }

        public async Task AddAsync(SmartMeter smartMeter, CancellationToken ct = default)
        {
            var entity = _mapper.ToEntity(smartMeter);
            await _context.SmartMeters.AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(SmartMeter smartMeter, CancellationToken ct = default)
        {
            var entity = await _context.SmartMeters.FirstOrDefaultAsync(x => x.Id == smartMeter.Id, ct);
            if (entity is null) return;

            entity.Label = smartMeter.Label;
            entity.ConnectionType = smartMeter.ConnectionType.ToString();
            entity.MaxApprovedPower = smartMeter.MaxApprovedPower;
            entity.Note = smartMeter.Note;
            entity.SerialNumber = smartMeter.SerialNumber;
            entity.PairingStatus = smartMeter.PairingStatus.ToString();
            entity.DeviceUUID = smartMeter.DeviceUUID;
            entity.AccessToken = smartMeter.AccessToken;

            _context.SmartMeters.Update(entity);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var entity = await _context.SmartMeters.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (entity is null) return;

            _context.SmartMeters.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }

        public Task<SmartMeter?> GetByDeviceUuidAsync(string deviceUuid, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
