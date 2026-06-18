namespace IndaloaventurApp.SharedUI.Models.Phonebook;

public sealed record PhonebookContact(
    Guid Id,
    DateTime FechaAlta,
    string DisplayName,
    string? Telefono1,
    string? Telefono2,
    string? Email,
    string? Direccion,
    string? Observaciones);
