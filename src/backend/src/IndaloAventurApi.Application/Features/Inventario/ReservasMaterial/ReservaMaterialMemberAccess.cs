using IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;
using IndaloAventurApi.Application.Common;

namespace IndaloAventurApi.Application.Features.Inventario.ReservasMaterial;

internal static class ReservaMaterialMemberAccess
{
    public static async Task<SocioReservaDto> RequireSocioAsync(
        Guid userId,
        bool isMember,
        ISocioReservaReader socioReader,
        string action,
        CancellationToken cancellationToken)
    {
        if (!isMember)
        {
            throw new ForbiddenAccessException($"Solo los socios del club pueden {action} reservas de material.");
        }

        var socio = await socioReader.GetByUserIdAsync(userId, cancellationToken);
        if (socio is null)
        {
            throw new ForbiddenAccessException($"Para {action} una reserva de material debes disponer de una ficha de socio.");
        }

        return socio;
    }

    public static void RequireOwner(Guid ownerId, Guid authenticatedUserId)
    {
        if (ownerId != authenticatedUserId)
        {
            throw new KeyNotFoundException("La reserva de material no existe.");
        }
    }
}
