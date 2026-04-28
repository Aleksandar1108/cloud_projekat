using SmartGrid.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGrid.Application.Interfaces.Repositories
{
    public interface IEmailActivationRepository
    {
        Task<EmailActivation?> GetByTokenAsync(string token);
        Task AddAsync(EmailActivation activation);
        Task DeleteAsync(EmailActivation activation);
    }
}
