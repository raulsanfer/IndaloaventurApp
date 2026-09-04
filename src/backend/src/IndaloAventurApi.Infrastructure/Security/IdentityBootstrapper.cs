using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IndaloAventurApi.Infrastructure.Security;

public static class IdentityBootstrapper
{
    public static async Task InitializeIdentityAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Persistence.ApplicationDbContext>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var useEnsureCreated = configuration.GetValue<bool>("Testing:UseEnsureCreated");

        if (!useEnsureCreated && string.Equals(dbContext.Database.ProviderName, "Microsoft.EntityFrameworkCore.SqlServer", StringComparison.Ordinal))
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
        else
        {
            await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        }

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();
        var adminOptions = scope.ServiceProvider.GetRequiredService<IOptions<AdminSeedOptions>>().Value;

        await EnsureRoleExistsAsync(roleManager, IdentityRoles.Admin);
        await EnsureRoleExistsAsync(roleManager, IdentityRoles.Member);

        if (!adminOptions.Enabled)
        {
            return;
        }

        ValidateAdminSeedOptions(adminOptions);

        var adminUser = await userManager.FindByEmailAsync(adminOptions.Email);
        if (adminUser is null)
        {
            adminUser = new Usuario
            {
                UserName = adminOptions.Email,
                Email = adminOptions.Email,
                EmailConfirmed = true,
                LockoutEnabled = true,
                IsMember = false
            };

            var createResult = await userManager.CreateAsync(adminUser, adminOptions.Password);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException($"Unable to seed admin user: {string.Join("; ", createResult.Errors.Select(x => x.Description))}");
            }
        }
        else if (!adminUser.LockoutEnabled)
        {
            adminUser.LockoutEnabled = true;
            var updateResult = await userManager.UpdateAsync(adminUser);
            if (!updateResult.Succeeded)
            {
                throw new InvalidOperationException($"Unable to enable lockout for admin user: {string.Join("; ", updateResult.Errors.Select(x => x.Description))}");
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, IdentityRoles.Admin))
        {
            await userManager.AddToRoleAsync(adminUser, IdentityRoles.Admin);
        }
    }

    private static void ValidateAdminSeedOptions(AdminSeedOptions adminOptions)
    {
        if (string.IsNullOrWhiteSpace(adminOptions.Email))
        {
            throw new InvalidOperationException("Admin seed is enabled but 'AdminSeed:Email' is not configured.");
        }

        if (string.IsNullOrWhiteSpace(adminOptions.Password))
        {
            throw new InvalidOperationException("Admin seed is enabled but 'AdminSeed:Password' is not configured.");
        }
    }

    private static async Task EnsureRoleExistsAsync(RoleManager<IdentityRole<Guid>> roleManager, string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var result = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unable to seed role '{roleName}': {string.Join("; ", result.Errors.Select(x => x.Description))}");
            }
        }
    }
}

