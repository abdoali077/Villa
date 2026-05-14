using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villla.Application.Interfaces.CommonRepos;
using Villla.Application.Interfaces.Services;
using Villla.Application.Services.Implementation;
using Villla.Application.Services.Interface;
using Villla.Infrastructure.CommonImplementation.Services;

namespace Villla.Application.Application.Extension
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services)
        {
            //register services 
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IVillaService, VillaService>();
            services.AddScoped<IAmenityService, AmenityService>();
            services.AddScoped<IVillaNumberService, VillaNumberService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IHomeService, HomeService>();
            return services;
        }
     
      

        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            return services
                .AddDataAccess();
               
        }

    }
}
