namespace IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;

public sealed record SocioReservaDto(
    Guid UserId,
    string NombreCompleto,
    string Email);
