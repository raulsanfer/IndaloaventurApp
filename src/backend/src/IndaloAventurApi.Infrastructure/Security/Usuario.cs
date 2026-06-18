using Microsoft.AspNetCore.Identity;

namespace IndaloAventurApi.Infrastructure.Security;

public sealed class Usuario : IdentityUser<Guid>
{
    // Este flag representa si el usuario ya es socio del club tras validacion administrativa.
    // Es independiente del rol tecnico Member, que se asigna por defecto al registrarse.
    public bool IsMember { get; set; }
}
