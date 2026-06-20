using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SmartGrid.Application;
using SmartGrid.Infrastructure;
using SmartGrid.WebApi.BackgroundServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartGrid.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add infrastructure & application layers
            services.AddInfrastructure(configuration)
                    .AddApplication();

            var jwtKey = configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("Jwt:Key is not configured.");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                        ValidateIssuer = true,
                        ValidIssuer = "SmartGrid",
                        ValidateAudience = true,
                        ValidAudience = "SmartGrid",
                        ValidateLifetime = true,
                        NameClaimType = "id"
                    };
                });

            // Controllers + JSON enums
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            // Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SmartGrid API",
                    Version = "v1",
                    Description = "API for SmartGrid Web Application"
                });
            });

            return services;
        }

        public static IServiceCollection AddWebApiCors(this IServiceCollection services, string reactOrigin)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("_reactAppPolicy", policy =>
                {
                    policy.WithOrigins(reactOrigin)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });
            return services;
        }

        public static IServiceCollection AddWebApiSignalR(this IServiceCollection services)
        {
            services.AddSignalR()
                .AddJsonProtocol(options =>
                {
                    options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            return services;
        }

        public static IServiceCollection AddWebApiHostedServices(this IServiceCollection services)
        {
            services.AddHostedService<DeviceStatusWorker>();
            return services;
        }
    }
}
