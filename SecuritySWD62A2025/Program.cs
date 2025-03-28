using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecuritySWD62A2025.Data;
using SecuritySWD62A2025.Repositories;
using SecuritySWD62A2025.Utilities;
using System.Security.Cryptography;

namespace SecuritySWD62A2025
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

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            //we will need to add code which allows for External Login
            builder.Services.AddAuthentication()
            .AddMicrosoftAccount(options =>
            {
                options.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"]!;
                options.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"]!;
            });

            builder.Services.AddControllersWithViews();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Lockout duration
                options.Lockout.MaxFailedAccessAttempts = 3; // Number of failed attempts before lockout
                options.Lockout.AllowedForNewUsers = true; // Enable lockout for new users

                options.Password.RequireDigit = true;  // Require at least one number (0-9)
                options.Password.RequiredLength = 8;   // Minimum password length
                options.Password.RequireNonAlphanumeric = true; // Require special characters (e.g., !, @, #)
                options.Password.RequireUppercase = true; // Require at least one uppercase letter
                options.Password.RequireLowercase = true; // Require at least one lowercase letter

            });


            builder.Services.AddScoped<ArtifactsRepository>();
            builder.Services.AddScoped<ArticlesRepository>();
            builder.Services.AddScoped<EncryptionUtility>();
           
            var app = builder.Build();
           

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();

                //things that you don't want to let the users see unless you're working form visual studio....you put it here
                //e.g if you want to disclose error technical information...you do it here
            }
            else
            {

                //you do not disclose error information
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(); //from the url we can download and access files e.g. jpgs, pdfs, etc...

            app.UseRouting(); //throught routing we can access the actions/methods...etc not the pages/views

            app.UseAuthentication();
            app.UseAuthorization(); //enables the usage of [Authorize]

            app.MapControllerRoute( //defining a defaulte
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"); //Home/Index    //Articles/Details/1
            app.MapRazorPages(); //.cshtml allows us to write C# within html

            app.Run();
        }
    }
}
