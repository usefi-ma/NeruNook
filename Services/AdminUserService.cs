using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NeruNook.Data;

namespace NeruNook.Services;

public sealed record AdminUserSummary(
    string Id,
    string Email,
    bool IsAdmin,
    bool IsLocked,
    DateTimeOffset? LockoutEnd);

public sealed record UserOperationResult(bool Succeeded, string Message)
{
    public static UserOperationResult Success(string message) => new(true, message);
    public static UserOperationResult Failure(IEnumerable<IdentityError> errors) =>
        new(false, string.Join(" ", errors.Select(x => x.Description)));
    public static UserOperationResult Failure(string message) => new(false, message);
}

public sealed class AdminUserService(IServiceScopeFactory scopeFactory)
{
    public async Task<IReadOnlyList<AdminUserSummary>> GetUsersAsync()
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var users = await userManager.Users.OrderBy(x => x.Email).ToListAsync();
        var result = new List<AdminUserSummary>(users.Count);

        foreach (var user in users)
        {
            result.Add(new(
                user.Id,
                user.Email ?? user.UserName ?? "",
                await userManager.IsInRoleAsync(user, IdentitySeeder.AdminRole),
                user.LockoutEnd is not null && user.LockoutEnd > DateTimeOffset.UtcNow,
                user.LockoutEnd));
        }

        return result;
    }

    public async Task<UserOperationResult> CreateAsync(string email, string password, bool makeAdmin)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var normalizedEmail = email.Trim();

        if (await userManager.FindByEmailAsync(normalizedEmail) is not null)
        {
            return UserOperationResult.Failure("A user with this email already exists.");
        }

        var user = new ApplicationUser
        {
            UserName = normalizedEmail,
            Email = normalizedEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return UserOperationResult.Failure(result.Errors);
        }

        if (makeAdmin)
        {
            result = await userManager.AddToRoleAsync(user, IdentitySeeder.AdminRole);
            if (!result.Succeeded)
            {
                await userManager.DeleteAsync(user);
                return UserOperationResult.Failure(result.Errors);
            }
        }

        return UserOperationResult.Success("User created.");
    }

    public async Task<UserOperationResult> SetLockedAsync(string userId, bool locked)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return UserOperationResult.Failure("User not found.");
        }

        if (locked && await IsLastActiveAdminAsync(userManager, user))
        {
            return UserOperationResult.Failure("The last active administrator cannot be disabled.");
        }

        var result = await userManager.SetLockoutEndDateAsync(
            user,
            locked ? DateTimeOffset.MaxValue : null);

        if (!locked)
        {
            await userManager.ResetAccessFailedCountAsync(user);
        }

        return result.Succeeded
            ? UserOperationResult.Success(locked ? "User disabled." : "User enabled.")
            : UserOperationResult.Failure(result.Errors);
    }

    public async Task<UserOperationResult> SetAdminAsync(string userId, bool isAdmin)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return UserOperationResult.Failure("User not found.");
        }

        if (!isAdmin && await IsLastActiveAdminAsync(userManager, user))
        {
            return UserOperationResult.Failure("The last active administrator must keep the Admin role.");
        }

        var result = isAdmin
            ? await userManager.AddToRoleAsync(user, IdentitySeeder.AdminRole)
            : await userManager.RemoveFromRoleAsync(user, IdentitySeeder.AdminRole);

        return result.Succeeded
            ? UserOperationResult.Success("Role updated.")
            : UserOperationResult.Failure(result.Errors);
    }

    public async Task<UserOperationResult> ResetPasswordAsync(string userId, string newPassword)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return UserOperationResult.Failure("User not found.");
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, newPassword);
        return result.Succeeded
            ? UserOperationResult.Success("Password reset.")
            : UserOperationResult.Failure(result.Errors);
    }

    private static async Task<bool> IsLastActiveAdminAsync(
        UserManager<ApplicationUser> userManager,
        ApplicationUser candidate)
    {
        if (!await userManager.IsInRoleAsync(candidate, IdentitySeeder.AdminRole))
        {
            return false;
        }

        var admins = await userManager.GetUsersInRoleAsync(IdentitySeeder.AdminRole);
        return admins.Count(x => x.LockoutEnd is null || x.LockoutEnd <= DateTimeOffset.UtcNow) <= 1;
    }
}
