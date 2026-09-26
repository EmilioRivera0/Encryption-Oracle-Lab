using EncryptionOracleVulnerability.Interfaces;
using EncryptionOracleVulnerability.Interfaces.Interfaces;
using EncryptionOracleVulnerability.Middlewares;
using EncryptionOracleVulnerability.Services;
using EncryptionOracleVulnerability.Services.Interfaces;
using EncryptionOracleVulnerability.Utilities;
using EncryptionOracleVulnerability.Utilities.Interfaces;

namespace EncryptionOracleVulnerability
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddAuthentication(AuthTokenMiddleware.AuthenticationType)
                .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, AuthTokenAuthenticationHandler>(
                    AuthTokenMiddleware.AuthenticationType, null);
            builder.Services.AddAuthorization();
            builder.Services.AddSingleton<IUserRepository, UserRepository>();
            builder.Services.AddSingleton<IBlobRepository, BlobRepository>();
            builder.Services.AddScoped<ILoginService, LoginService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IFileOperationsService, FileOperationsService>();
            builder.Services.AddSingleton<IEncryptionUtility, EncryptionUtility>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseMiddleware<AuthTokenMiddleware>();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
