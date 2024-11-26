using TransportSchadules.Data;
using TransportSchadules.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransportSchadules.Middleware;
using TransportSchadules.Models;

namespace TransportSchadules
{
    public class Program
    {
            public static void Main(string[] args)
            {
                WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
                IServiceCollection services = builder.Services;

                // Внедрение зависимости для доступа к БД с использованием EF
                string connectionString = builder.Configuration.GetConnectionString("DBConnection");
                services.AddDbContext<TransportDbContext>(options => options.UseSqlServer(connectionString));
                builder.Services.AddDatabaseDeveloperPageExceptionFilter();



            services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<TransportDbContext>()
                .AddDefaultTokenProviders();

            // Добавление кэширования
            services.AddMemoryCache();
                services.AddResponseCaching();
                services.AddControllers(options =>
                {
                    options.CacheProfiles.Add("Default",
                        new CacheProfile()
                        {
                            Duration = 2 * 28 + 240
                        });
                });
                // Добавление поддержки сессии
                services.AddDistributedMemoryCache();
                services.AddSession();
                services.AddRazorPages();
                // Регистрация сервисов
                services.AddTransient<IViewModelService, HomeModelService>();
                
                // Использование MVC
                services.AddControllersWithViews();

                WebApplication app = builder.Build();

                if (app.Environment.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                }

                // Добавляем поддержку статических файлов
                app.UseStaticFiles();

                // Добавляем поддержку сессий
                app.UseSession();

                // Инициализация базы данных
                app.UseDbInitializer();

                // Кэширование
                app.UseOperatinCache("Inspections 10");

                // Настройка маршрутизации
                app.UseRouting();
                app.UseAuthentication();
                app.UseAuthorization();

                app.UseResponseCaching(); // Включение ResponseCaching Middleware
                // Устанавливаем сопоставление маршрутов с контроллерами
                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                app.MapRazorPages();

                app.Run();
            }
        }
}
