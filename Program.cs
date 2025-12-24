using Glowry.Data;
using Glowry.Models;
using Glowry.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Glowry
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            //identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {   options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false; 
                options.Password.RequireUppercase =false; 
            })
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();
            //esmail service 
            builder.Services.AddTransient<IEmailSender, Glowry.Services.EmailSender>();
            builder.Services.AddRazorPages();

            var app = builder.Build();

            


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
            app.MapControllerRoute(
                name: "areas",
                pattern: "{controller=Product}/{action=Index}/{id?}")
                .WithStaticAssets();
            app.MapControllerRoute(
                name: "areas",
                pattern: "{controller=ImageMap}/{action=Index}/{id?}")
                .WithStaticAssets();
            app.MapControllerRoute(
                name: "areas",
                pattern: "{controller=ProductImg}/{action=Index}/{id?}")
                .WithStaticAssets();
            app.MapControllerRoute(
                name: "areas",
                pattern: "{controller=ProductOption}/{action=Index}/{id?}")
                .WithStaticAssets();
            app.MapControllerRoute(
            name: "areas",
                pattern: "{controller=Category}/{action=Index}/{id?}")
                .WithStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.MapRazorPages();

            app.Run();
        }
    }
}
