namespace IndaloAventurApi.Application.Abstractions.WordPress;

public sealed record WordPressPostDto(
    long Id,
    string Slug,
    string Titulo,
    string Resumen,
    string Contenido,
    string? ImagenDestacadaUrl,
    string Enlace,
    DateTime FechaPublicacionUtc);
