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

                // ��������� ����������� ��� ������� � �� � �������������� EF
                string connectionString = builder.Configuration.GetConnectionString("DBConnection");
                services.AddDbContext<TransportDbContext>(options => options.UseSqlServer(connectionString));
                builder.Services.AddDatabaseDeveloperPageExceptionFilter();



            services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<TransportDbContext>()
                .AddDefaultTokenProviders();

            // ���������� �����������
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
                // ���������� ��������� ������
                services.AddDistributedMemoryCache();
                services.AddSession();
                services.AddRazorPages();
                // ����������� ��������
                services.AddTransient<IViewModelService, HomeModelService>();
                
                // ������������� MVC
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

                // ��������� ��������� ����������� ������
                app.UseStaticFiles();

                // ��������� ��������� ������
                app.UseSession();

                // ������������� ���� ������
                app.UseDbInitializer();

                // �����������
                app.UseOperatinCache("Inspections 10");

                // ��������� �������������
                app.UseRouting();
                app.UseAuthentication();
                app.UseAuthorization();

                app.UseResponseCaching(); // ��������� ResponseCaching Middleware
                // ������������� ������������� ��������� � �������������
                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                app.MapRazorPages();

                app.Run();
            }
        }
}
