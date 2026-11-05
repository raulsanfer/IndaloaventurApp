namespace IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.Notifications;

public sealed class ReservaMaterialNotificationOptions
{
    public const string SectionName = "Email";

    public string InventoryReservationNotificationsTo { get; init; } = "club@indaloaventura.com";
}
