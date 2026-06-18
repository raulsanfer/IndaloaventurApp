namespace IndaloAventurApi.Application.Abstractions.TrailSignals;

public sealed record SignalDto(
    Guid Id,
    float Latitud,
    float Longitud,
    string Titulo,
    string Descripcion,
    bool Activo,
    Guid UserIdAlta,
    DateTime FechaAlta,
    DateTime FechaModificacion,
    Guid UserIdModificacion,
    int Tipo,
    string Tags);
