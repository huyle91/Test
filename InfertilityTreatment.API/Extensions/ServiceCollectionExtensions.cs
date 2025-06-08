using FluentValidation;
using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Business.Services;
using InfertilityTreatment.Business.Helpers;
using InfertilityTreatment.Business.Validators;
using InfertilityTreatment.Business.Mappings;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Data.Repositories.Implementations;
using InfertilityTreatment.Entity.DTOs.Auth;

namespace InfertilityTreatment.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            // Repository Pattern
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ITreatmentCycleRepository, TreatmentCycleRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            // Business Services
            services.AddScoped<IAuthService, AuthService>();

            // Helpers
            services.AddScoped<JwtHelper>();

            // AutoMapper
            services.AddAutoMapper(typeof(AutoMapperProfile));

            // FluentValidation
            services.AddScoped<IValidator<LoginRequestDto>, LoginRequestValidator>();
            services.AddScoped<IValidator<RegisterRequestDto>, RegisterRequestValidator>();

            return services;
        }
    }
}
