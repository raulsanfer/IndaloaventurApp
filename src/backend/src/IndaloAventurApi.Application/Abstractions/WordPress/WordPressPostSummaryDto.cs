namespace IndaloAventurApi.Application.Abstractions.WordPress;

public sealed record WordPressPostSummaryDto(
    long Id,
    string Slug,
    string Titulo,
    string Resumen,
    string? ImagenDestacadaUrl,
    DateTime FechaPublicacionUtc);
