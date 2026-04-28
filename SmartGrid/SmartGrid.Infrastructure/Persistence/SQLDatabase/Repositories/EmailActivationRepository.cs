using Microsoft.EntityFrameworkCore;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Models;
using SmartGrid.Infrastructure.Persistence.SQLDatabase.Common;
using SmartGrid.Infrastructure.Persistence.SQLDatabase.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGrid.Infrastructure.Persistence.SQLDatabase.Repositories
{
    public class EmailActivationRepository : IEmailActivationRepository
    {
        private readonly SmartGridDbContext _context;
        private readonly IDatabaseMapper<EmailActivation, EmailActivationEntity> _mapper;

        public EmailActivationRepository(
            SmartGridDbContext context,
            IDatabaseMapper<EmailActivation, EmailActivationEntity> mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddAsync(EmailActivation activation)
        {
            var entity = _mapper.ToEntity(activation);

            await _context.EmailActivations.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(EmailActivation activation)
        {
            var entity = await _context.EmailActivations
                .FirstOrDefaultAsync(x => x.IdEmailActivation == activation.Id.Value);

            if (entity is null)
                return;

            _context.EmailActivations.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<EmailActivation?> GetByTokenAsync(string token)
        {
            var entity = await _context.EmailActivations
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ActivationToken == token);

            return entity is null ? null : _mapper.ToDomain(entity);
        }
    }
}
