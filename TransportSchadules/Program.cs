using TransportSchadules.Data;
using TransportSchadules.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransportSchadules.Middleware;
using TransportSchadules.Models;
using Microsoft.AspNetCore.Authorization;

namespace TransportSchadules
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Получение строки подключения из конфигурации
            string connectionString = builder.Configuration.GetConnectionString("DBConnection");

            // Настройка сервисов
            builder.Services.AddDbContext<TransportDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Настройка Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<TransportDbContext>()
            .AddDefaultTokenProviders();

            // Кэширование и сессии
            builder.Services.AddMemoryCache();
            builder.Services.AddResponseCaching();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(); // Обратите внимание на эту строку

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login"; 
                options.AccessDeniedPath = "/Identity/Account/AccessDenied"; 
            });

            builder.Services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter(policy));
            });

            builder.Services.AddRazorPages(options =>
            {
                options.Conventions.AllowAnonymousToPage("/Identity/Account/Login");
                options.Conventions.AllowAnonymousToPage("/Identity/Account/Register");
                options.Conventions.AllowAnonymousToPage("/Identity/Account/AccessDenied");
            });



            // Настройка контроллеров с кешированием
            builder.Services.AddControllers(options =>
            {
                options.CacheProfiles.Add("Default", new CacheProfile
                {
                    Duration = 2 * 28 + 240
                });
            });

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            // Регистрация пользовательских сервисов
            builder.Services.AddTransient<IViewModelService, HomeModelService>();

            // Создание приложения
            WebApplication app = builder.Build();

            // Настройка pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            // Порядок middleware имеет значение:
            app.UseSession(); // Этот middleware должен быть вызван до UseRouting()

            app.UseRouting();
            app.UseAuthentication(); // Аутентификация
            app.UseAuthorization();  // Авторизация

            app.UseDbInitializer();

            app.UseResponseCaching();

            // Настройка маршрутов
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            // Запуск приложения
            app.Run();
        }
    }
}
