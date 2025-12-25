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
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            //identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
            
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            builder.Services.AddControllersWithViews();
            //esmail service 
            builder.Services.AddTransient<IEmailSender, Glowry.Services.EmailSender>();


            builder.Services.AddRazorPages();
            


            var app = builder.Build();

            //Role
         

            
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                // Create roles
                if (!await roleManager.RoleExistsAsync("Admin"))
                    await roleManager.CreateAsync(new IdentityRole("Admin"));

                if (!await roleManager.RoleExistsAsync("User"))
                    await roleManager.CreateAsync(new IdentityRole("User"));

                // Create admin user (optional)
                var adminEmail = "mennamennamenna333@yahoo.com";
                var adminPassword = "Admin12345";

                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    await userManager.CreateAsync(adminUser, adminPassword);
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
            /* ===== End ===== */




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

            await app.RunAsync();

        }
    }
} 