using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Villla.Application.Application.Extension;
using Villla.Application.Interfaces.CommonRepos;
using Villla.Application.Interfaces.Services;
using Villla.Application.Services.Implementation;
using Villla.Application.Services.Interface;
using Villla.Application.Settings;
using Villla.Domain.Entities;
using Villla.Infrastructure.CommonImplementation.Services;
using Villla.Infrastructure.Data;
using Villla.Infrastructure.RepositoryImplementation;

namespace Villaa.web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            //builder.Services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                var conn = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION");
                options.UseSqlServer(conn);
            });
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });
            builder.Services.Configure<EmailSettings>(
            builder.Configuration.GetSection("EmailSettings"));

            builder.Services.AddScoped<IUnitOfWork, UniteOfWork>();

            builder.Services.AddApplicationLayer();
            //Stripe.StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
            Stripe.StripeConfiguration.ApiKey =Environment.GetEnvironmentVariable("STRIPE_SECRET");
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
