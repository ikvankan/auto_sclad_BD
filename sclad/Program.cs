using sclad.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using sclad.Utility;
using sclad.Utility.BrainTree;
using Microsoft.Extensions.Configuration;
using Braintree;


namespace sclad
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession(Options =>
            {
                Options.IdleTimeout = TimeSpan.FromMinutes(10);
                Options.Cookie.HttpOnly = true;
                Options.Cookie.IsEssential = true;
            });
            builder.Services.Configure<BrainTreeSettings>(builder.Configuration.GetSection("Braintree"));
            builder.Services.AddSingleton<IBrainTreeGate, BrainTreeGate>();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("Defaultconnection")));
            builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddDefaultTokenProviders().AddDefaultUI()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddTransient<IEmailSender, EmailSender>();

            builder.Services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = "965777046602-pm3oljbc4ka7c09t7ua9bal83hdkvsq5.apps.googleusercontent.com";
                options.ClientSecret = "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";
            });

            var supportedCultures = new[] { new CultureInfo("en-US") };
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            
            var app = builder.Build();
            app.UseRequestLocalization();




            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            }
            );
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}