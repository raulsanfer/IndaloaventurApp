namespace IndaloAventurApi.Application.Abstractions.Phonebook;

public sealed record FichaContactoDto(
    Guid Id,
    DateTime FechaAlta,
    string Nombre,
    string Telefono1,
    string? Telefono2,
    string? Email,
    string? Direccion,
    string? Observaciones);
