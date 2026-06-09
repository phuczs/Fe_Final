using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServiceLayer(this IServiceCollection services)
        {
            services.AddScoped<IEmailSender, GmailEmailSender>();
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IMfaService, MfaService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuditQueryService, AuditQueryService>();
            services.AddScoped<IWhitelistService, WhitelistService>();
            services.AddScoped<IRegistrationVerificationService, RegistrationVerificationService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<INotificationService, NotificationService>();

            return services;
        }
    }
}
