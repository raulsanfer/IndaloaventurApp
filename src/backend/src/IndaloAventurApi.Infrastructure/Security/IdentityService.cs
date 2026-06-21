using IndaloAventurApi.Application.Abstractions.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IndaloAventurApi.Infrastructure.Security;

public sealed class IdentityService(
    UserManager<Usuario> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    ISocialTokenValidator socialTokenValidator)
    : IIdentityService
{
    public async Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterAsync(string email, string password, CancellationToken cancellationToken)
    {
        // Todo usuario nuevo entra con rol Member para poder usar la app,
        // pero IsMember permanece a false hasta que administracion lo marque como socio del club.
        var user = new Usuario { UserName = email, Email = email, LockoutEnabled = true, IsMember = false };
        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await EnsureRoleExistsAsync(IdentityRoles.Member);
            await userManager.AddToRoleAsync(user, IdentityRoles.Member);
        }
        return (result.Succeeded, result.Errors.Select(x => x.Description));
    }

    public async Task<(bool Succeeded, Guid? UserId, IEnumerable<string> Roles, bool IsMember)> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return (false, null, [], false);
        }

        await EnsureLockoutEnabledAsync(user);

        if (await userManager.IsLockedOutAsync(user))
        {
            return (false, null, [], false);
        }

        var valid = await userManager.CheckPasswordAsync(user, password);
        if (!valid)
        {
            await userManager.AccessFailedAsync(user);
            return (false, null, [], false);
        }

        await userManager.ResetAccessFailedCountAsync(user);

        var roles = await userManager.GetRolesAsync(user);
        return (true, user.Id, roles, user.IsMember);
    }

    public async Task<(bool Succeeded, Guid? UserId, string? Email, IEnumerable<string> Roles, bool IsMember, IEnumerable<string> Errors)> ValidateSocialLoginAsync(string provider, string token, CancellationToken cancellationToken)
    {
        var validation = await socialTokenValidator.ValidateAsync(provider, token, cancellationToken);
        if (!validation.Succeeded)
        {
            return (false, null, null, [], false, validation.Errors);
        }

        var loginInfo = new UserLoginInfo(validation.Provider, validation.ProviderUserId, validation.Provider);
        var user = await userManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);
        if (user is null)
        {
            user = await userManager.FindByEmailAsync(validation.Email);
            if (user is null)
            {
                // En primer acceso social se crea un usuario operativo con rol Member,
                // manteniendo IsMember a false hasta la validacion administrativa de socio.
                user = new Usuario
                {
                    UserName = validation.Email,
                    Email = validation.Email,
                    EmailConfirmed = true,
                    LockoutEnabled = true,
                    IsMember = false
                };

                var createResult = await userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    return (false, null, null, [], false, createResult.Errors.Select(x => x.Description));
                }

                await EnsureRoleExistsAsync(IdentityRoles.Member);
                await userManager.AddToRoleAsync(user, IdentityRoles.Member);
            }
            else
            {
                await EnsureLockoutEnabledAsync(user);
            }

            var addLoginResult = await userManager.AddLoginAsync(user, loginInfo);
            if (!addLoginResult.Succeeded)
            {
                return (false, null, null, [], false, addLoginResult.Errors.Select(x => x.Description));
            }
        }

        await EnsureLockoutEnabledAsync(user);

        if (await userManager.IsLockedOutAsync(user))
        {
            return (false, null, null, [], false, ["La cuenta de usuario esta desactivada."]);
        }

        var roles = await userManager.GetRolesAsync(user);
        return (true, user.Id, user.Email, roles, user.IsMember, []);
    }

    public async Task<bool> IsUserActiveAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return false;
        }

        return !await userManager.IsLockedOutAsync(user);
    }

    public async Task<(bool Succeeded, Guid UserId, IEnumerable<string> Errors)> CreateUserAsync(string email, string password, IEnumerable<string> roles, CancellationToken cancellationToken)
    {
        var user = new Usuario { UserName = email, Email = email, LockoutEnabled = true, IsMember = false };
        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return (false, Guid.Empty, result.Errors.Select(x => x.Description));
        }

        var rolesToAssign = roles.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        if (rolesToAssign.Length > 0)
        {
            foreach (var role in rolesToAssign)
            {
                await EnsureRoleExistsAsync(role);
            }
            var addRolesResult = await userManager.AddToRolesAsync(user, rolesToAssign);
            if (!addRolesResult.Succeeded)
            {
                return (false, Guid.Empty, addRolesResult.Errors.Select(x => x.Description));
            }
        }

        return (true, user.Id, []);
    }

    public async Task<IReadOnlyCollection<ManagedUserDto>> ListUsersAsync(string? email, CancellationToken cancellationToken)
    {
        var query = userManager.Users.AsQueryable();
        if (!string.IsNullOrWhiteSpace(email))
        {
            var normalizedEmail = email.Trim().ToUpperInvariant();
            query = query.Where(x => x.NormalizedEmail != null && x.NormalizedEmail.Contains(normalizedEmail));
        }

        var users = await query.OrderBy(x => x.Email).ToListAsync(cancellationToken);
        var result = new List<ManagedUserDto>(users.Count);

        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            result.Add(new ManagedUserDto(user.Id, user.Email ?? string.Empty, user.IsMember, roles.ToArray()));
        }

        return result;
    }

    public async Task<(bool Succeeded, IEnumerable<string> Errors)> SetIsMemberAsync(Guid userId, bool isMember, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.SingleOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null)
        {
            return (false, ["Usuario no encontrado."]);
        }

        user.IsMember = isMember;
        return (true, []);
    }

    public async Task<(bool Succeeded, IEnumerable<string> Errors)> UpdateUserAsync(Guid userId, string email, bool isMember, IEnumerable<string> roles, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return (false, ["Usuario no encontrado."]);
        }

        user.Email = email;
        user.UserName = email;
        // IsMember expresa condicion de socio del club; no equivale a pertenecer al rol tecnico Member.
        user.IsMember = isMember;
        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return (false, updateResult.Errors.Select(x => x.Description));
        }

        var existingRoles = await userManager.GetRolesAsync(user);
        var rolesToRemove = existingRoles.Except(roles, StringComparer.OrdinalIgnoreCase).ToArray();
        if (rolesToRemove.Length > 0)
        {
            var removeResult = await userManager.RemoveFromRolesAsync(user, rolesToRemove);
            if (!removeResult.Succeeded)
            {
                return (false, removeResult.Errors.Select(x => x.Description));
            }
        }

        var rolesToAdd = roles.Except(existingRoles, StringComparer.OrdinalIgnoreCase).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        if (rolesToAdd.Length > 0)
        {
            var addResult = await userManager.AddToRolesAsync(user, rolesToAdd);
            if (!addResult.Succeeded)
            {
                return (false, addResult.Errors.Select(x => x.Description));
            }
        }

        return (true, []);
    }

    public async Task<(bool Succeeded, IEnumerable<string> Errors)> DeactivateUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return (false, ["Usuario no encontrado."]);
        }

        user.LockoutEnabled = true;
        user.LockoutEnd = DateTimeOffset.MaxValue;
        var result = await userManager.UpdateAsync(user);
        return result.Succeeded
            ? (true, [])
            : (false, result.Errors.Select(x => x.Description));
    }

    public async Task<(bool Succeeded, IEnumerable<string> Errors)> ReactivateUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return (false, ["Usuario no encontrado."]);
        }
        
        user.LockoutEnd = null;
        await userManager.ResetAccessFailedCountAsync(user);
        var result = await userManager.UpdateAsync(user);
        return result.Succeeded
            ? (true, [])
            : (false, result.Errors.Select(x => x.Description));
    }

    public async Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return null;
        }

        if (await userManager.IsLockedOutAsync(user))
        {
            return null;
        }

        return await userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<(bool Succeeded, IEnumerable<string> Errors)> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return (false, ["No se ha podido completar el reseteo de contrasena."]);
        }

        var result = await userManager.ResetPasswordAsync(user, token, newPassword);
        return result.Succeeded
            ? (true, [])
            : (false, result.Errors.Select(MapIdentityError));
    }

    private static string MapIdentityError(IdentityError error) =>
        error.Code switch
        {
            "InvalidToken" => "El token de recuperacion no es valido o ha expirado.",
            "PasswordTooShort" => "La contrasena debe tener al menos 8 caracteres.",
            "PasswordRequiresDigit" => "La contrasena debe contener al menos un numero.",
            "PasswordRequiresUpper" => "La contrasena debe contener al menos una letra mayuscula.",
            "PasswordRequiresLower" => "La contrasena debe contener al menos una letra minuscula.",
            _ => error.Description
        };

    private async Task EnsureRoleExistsAsync(string role)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }
    }

    private async Task EnsureLockoutEnabledAsync(Usuario user)
    {
        if (user.LockoutEnabled)
        {
            return;
        }

        user.LockoutEnabled = true;
        await userManager.UpdateAsync(user);
    }
}
