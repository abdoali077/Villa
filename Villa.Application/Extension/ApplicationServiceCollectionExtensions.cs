using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villla.Application.Caching;
using Villla.Application.Decorators;
using Villla.Application.Interfaces.CommonRepos;
using Villla.Application.Services.Implementation;
using Villla.Application.Services.Interface;
using Villla.Application.Services.Interface.Cashing;
using Villla.Infrastructure.CommonImplementation.Services;

namespace Villla.Application.Application.Extension
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services)
        {
        
            //---------------------------------
            services.AddMemoryCache();
            services.AddScoped<ICacheService, MemoryCacheService>();

            // ================= BASE SERVICES =================
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IAccountService, AccountService>();

            // ================= VILLA (DECORATOR) =================
            services.AddScoped<IVillaService, VillaService>();
            services.Decorate<IVillaService, CachedVillaService>();

            // ================= AMENITY (DECORATOR) =================
            services.AddScoped<IAmenityService, AmenityService>();
            services.Decorate<IAmenityService, CachedAmenityService>();

            // ================= VILLA NUMBER (DECORATOR) =================
            services.AddScoped<IVillaNumberService, VillaNumberService>();
            services.Decorate<IVillaNumberService, CachedVillaNumberService>();

            // ================= HOME (DECORATOR) =================
            services.AddScoped<IHomeService, HomeService>();
            services.Decorate<IHomeService, CachedHomeService>();

            return services;
        }
     
      

        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            return services
                .AddDataAccess();
               
        }

    }
}
