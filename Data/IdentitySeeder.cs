using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NeruNook.Configuration;

namespace NeruNook.Data;

public static class IdentitySeeder
{
    public const string AdminRole = "Admin";

    public static async Task SeedIdentityAsync(this IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<AdminSeedOptions>>().Value;
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("IdentitySeeder");

        if (!await roleManager.RoleExistsAsync(AdminRole))
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole(AdminRole));
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Unable to seed the Admin role: {string.Join("; ", roleResult.Errors.Select(x => x.Description))}");
            }
        }

        if (string.IsNullOrWhiteSpace(options.Email) || string.IsNullOrWhiteSpace(options.Password))
        {
            logger.LogWarning(
                "No initial administrator was created. Configure AdminSeed:Email and AdminSeed:Password using user-secrets or environment variables.");
            return;
        }

        var user = await userManager.FindByEmailAsync(options.Email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = options.Email,
                Email = options.Email,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(user, options.Password);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Unable to seed the initial administrator: {string.Join("; ", createResult.Errors.Select(x => x.Description))}");
            }
        }

        if (!await userManager.IsInRoleAsync(user, AdminRole))
        {
            var roleResult = await userManager.AddToRoleAsync(user, AdminRole);
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Unable to assign the initial administrator role: {string.Join("; ", roleResult.Errors.Select(x => x.Description))}");
            }
        }

        logger.LogInformation("Initial NeruNook administrator provisioned from configuration.");
    }
}
