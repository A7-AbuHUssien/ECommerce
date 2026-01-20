using ECommerce.DataAccess;
using ECommerce.DbSeeder;
using ECommerce.Repositories;
using ECommerce.Repositories.Interfaces;
using ECommerce.Services;
using ECommerce.Services.Interfaces;
using ECommerce.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;
using AccountService = ECommerce.Services.AccountService;
using OrderService = Stripe.Climate.OrderService;
using ProductService = ECommerce.Services.ProductService;

namespace ECommerce;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddDbContext<ApplicationDbContext>();

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        // Register ApplicationDbContext with Dependency Injection
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ??
                                 throw new InvalidOperationException(
                                     "Connection string 'DefaultConnection' not found.")));

        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped(typeof(IProductSubImageRepository), typeof(ProductSubImageRepository));
        builder.Services.AddScoped(typeof(IProductColor), typeof(ProductColorRepository));
        builder.Services.AddScoped(typeof(IOrderRepository), typeof(ECommerce.Repositories.OrderRepository));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Identity/Account/Login";
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";
        });
        builder.Services.AddTransient(typeof(IEmailSender), typeof(EmailSender));
        builder.Services.AddScoped(typeof(IAccountService), typeof(AccountService));
        builder.Services.AddScoped(typeof(IBrandService), typeof(BrandService));
        builder.Services.AddScoped(typeof(ICategoryService), typeof(CategoryService));
        builder.Services.AddScoped(typeof(IProductService), typeof(ProductService));
        builder.Services.AddScoped(typeof(IUserService), typeof(UserService));
        builder.Services.AddScoped(typeof(ICartService), typeof(CartService));
        builder.Services.AddScoped(typeof(IPromotionService), typeof(PromotionService));
        builder.Services.AddScoped(typeof(IOrderService), typeof(ECommerce.Services.OrderService));
        builder.Services.AddScoped<DbInitializer>();

        // Stripe
        StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe")["SecretKey"];

        var app = builder.Build();
        using (var scope = app.Services.CreateScope())
        {
            var initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
            initializer.Initialize();
        }


        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            // ✅ Fixed route (should not be /HomeController/Error)
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();


        // ✅ Route for Areas first
        app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

        // ✅ Default route for non-area controllers
        app.MapControllerRoute(
            name: "default",
            pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}