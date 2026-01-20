using ECommerce.DataAccess;
using ECommerce.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.DbSeeder;

public class DbInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DbInitializer> _logger;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManger;
    public DbInitializer(ApplicationDbContext context,ILogger<DbInitializer> logger,RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser> userManger)
    {
        _context = context;
        _logger = logger;
        _roleManager = roleManager;
        _userManger = userManger;
    }
    public void Initialize()
    {
        try
        {
            if(_context.Database.GetPendingMigrations().Any())
                _context.Database.Migrate();
            if (!_roleManager.Roles.Any())
            {
                _roleManager.CreateAsync(new(StaticData.SUPER_ADMIN_ROLE)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new(StaticData.ADMIN_ROLE)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new(StaticData.CUSTOMER_ROLE)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new(StaticData.EMPLOYEE_ROLE)).GetAwaiter().GetResult();

                _userManger.CreateAsync(new()
                {
                    Email = "SuperAdmin@gmail.com",
                    UserName = "SuperAdmin",
                    Name =  "SuperAdmin",
                    EmailConfirmed = true
                },"Admin123$").GetAwaiter().GetResult();
                var user = _userManger.FindByNameAsync("SuperAdmin").GetAwaiter().GetResult();
                _userManger.AddToRoleAsync(user!, StaticData.SUPER_ADMIN_ROLE).GetAwaiter().GetResult();
            }
        }catch(Exception e)
        {
            _logger.LogError($"ERROR: {e.Message}");
        }
    }
}