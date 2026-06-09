using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Repository
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepositoryLayer(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();

            // Add the newly created repositories here:
            services.AddScoped<IEmailWhitelistRepository, EmailWhitelistRepository>();
            services.AddScoped<IMfaCodeRepository, MfaCodeRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUserProductRepository, UserProductRepository>();
            services.AddScoped<IFavouriteProductRepository, FavouriteProductRepository>();
            services.AddScoped<IRegistrationVerificationCodeRepository, RegistrationVerificationCodeRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            
            services.AddScoped<ITenantRepository, TenantRepository>();

            return services;
        }
    }
}
