using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using ProjetERP.Areas.Identity.Data;
using ProjetERP.Data;
using ProjetERP.Repositories;
using ProjetERP.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ProjetERPDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ProjetERPDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

builder.Services.AddTransient<IEmailSender, SendGridEmailSender>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();

var app = builder.Build();

// Configurer la culture pour gérer les dates correctement (format yyyy-MM-dd pour <input type="date">)
var cultureInfo = new CultureInfo("fr-FR");
cultureInfo.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.MapRazorPages();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ProjetERPDbContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Starting database seeding...");

        // Vérifier s'il y a des migrations en attente avant de les appliquer
        if (context.Database.GetPendingMigrations().Any())
        {
            logger.LogInformation("Applying pending migrations...");
            context.Database.Migrate();
        }
        else
        {
            logger.LogInformation("No pending migrations to apply.");
        }

        // Création des rôles
        string[] roles = ["Admin", "Accountant", "Client", "Supplier"];
        foreach (var role in roles)
        {
            if (string.IsNullOrEmpty(role))
            {
                logger.LogWarning("Role name is null or empty.");
                continue;
            }

            if (!await roleManager.RoleExistsAsync(role))
            {
                logger.LogInformation($"Creating role {role}...");
                var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                if (!roleResult.Succeeded)
                {
                    logger.LogError($"Failed to create role {role}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    throw new Exception($"Failed to create role {role}");
                }
                logger.LogInformation($"Role {role} created.");
            }
            else
            {
                logger.LogInformation($"Role {role} already exists.");
            }
        }

        // Création de l'utilisateur admin
        var adminEmail = "bibooo5378@gmail.com";
        if (string.IsNullOrEmpty(adminEmail))
        {
            logger.LogError("Admin email is null or empty.");
            throw new ArgumentNullException(nameof(adminEmail));
        }

        logger.LogInformation($"Checking for admin user with email {adminEmail}...");
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            logger.LogInformation("Creating admin user...");
            adminUser = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "Admin"
            };
            var result = await userManager.CreateAsync(adminUser, "Admin@1234");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                logger.LogInformation("Admin user created and assigned to Admin role.");
            }
            else
            {
                logger.LogError($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                throw new Exception($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
        else
        {
            logger.LogInformation("Admin user already exists.");
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                logger.LogInformation("Admin role assigned to existing admin user.");
            }
        }

        // Création d'un utilisateur Client pour tester
        var clientEmail = "client@example.com";
        logger.LogInformation($"Checking for Client user with email {clientEmail}...");
        var clientUser = await userManager.FindByEmailAsync(clientEmail);
        if (clientUser == null)
        {
            logger.LogInformation("Creating Client user...");
            clientUser = new User
            {
                UserName = clientEmail,
                Email = clientEmail,
                EmailConfirmed = true,
                FirstName = "Client",
                LastName = "Test"
            };
            var result = await userManager.CreateAsync(clientUser, "Client@1234");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(clientUser, "Client");
                logger.LogInformation("Client user created and assigned to Client role.");
            }
            else
            {
                logger.LogError($"Failed to create Client user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                throw new Exception($"Failed to create Client user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
        else
        {
            logger.LogInformation("Client user already exists.");
            if (!await userManager.IsInRoleAsync(clientUser, "Client"))
            {
                await userManager.AddToRoleAsync(clientUser, "Client");
                logger.LogInformation("Client role assigned to existing Client user.");
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
        throw;
    }
}

app.Run();