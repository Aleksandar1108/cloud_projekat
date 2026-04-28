using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Models;
using SmartGrid.Infrastructure.Common.Options;
using SmartGrid.Infrastructure.Persistence.SQLDatabase.Common;
using SmartGrid.Infrastructure.Persistence.SQLDatabase.Entities;
using SmartGrid.Infrastructure.Persistence.SQLDatabase.Mappers;
using SmartGrid.Infrastructure.Persistence.SQLDatabase.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGrid.Infrastructure.Extensions
{
    public static class SQLDatabaseExtensions
    {
        public static IServiceCollection AddSqlDatabase(this IServiceCollection services)
        {
            services.AddDbContext<SmartGridDbContext>((sp, options) =>
            {
                var sqlOptions = sp
                    .GetRequiredService<IOptions<SQLServerOptions>>().Value;

                var connectionString =
                    $"Server={sqlOptions.Server};" +
                    $"Database={sqlOptions.Database};" +
                    $"User Id={sqlOptions.Login};" +
                    $"Password={sqlOptions.Password};" +
                    $"TrustServerCertificate=True;";

                options.UseSqlServer(connectionString);
            });
            services.AddScoped<IDatabaseMapper<User, UserEntity>, UserMapper>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITariffModelRepository, TariffModelRepository>();

            return services;
        }
    }
}
