using IndaloAventurApi.Application.Common;

namespace IndaloAventurApi.Application.Features.Inventario.ReservasMaterial;

internal static class ReservaMaterialAdminAccess
{
    public static void RequireAdmin(bool isAdmin)
    {
        if (!isAdmin)
        {
            throw new ForbiddenAccessException("Solo administracion puede gestionar las reservas de material.");
        }
    }
}
