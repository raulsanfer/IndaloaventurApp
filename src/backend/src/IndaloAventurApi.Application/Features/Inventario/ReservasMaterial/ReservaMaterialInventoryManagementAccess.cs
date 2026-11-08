using IndaloAventurApi.Application.Common;

namespace IndaloAventurApi.Application.Features.Inventario.ReservasMaterial;

internal static class ReservaMaterialInventoryManagementAccess
{
    public static void RequireInventoryManagement(bool canManageInventory)
    {
        if (!canManageInventory)
        {
            throw new ForbiddenAccessException("Solo administracion o los gestores de inventario pueden gestionar las reservas de material.");
        }
    }
}
