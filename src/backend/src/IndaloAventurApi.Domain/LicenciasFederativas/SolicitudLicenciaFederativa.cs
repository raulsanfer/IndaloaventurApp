using IndaloAventurApi.Domain.Abstractions;

namespace IndaloAventurApi.Domain.LicenciasFederativas;

public sealed class SolicitudLicenciaFederativa : Entity, IAggregateRoot
{
    private SolicitudLicenciaFederativa()
    {
    }

    private SolicitudLicenciaFederativa(
        Guid id,
        Guid userId,
        int temporada,
        int tarifaLicenciaFederativaId,
        EstadoSolicitudLicenciaFederativa estado,
        DateTime fechaCreacionUtc)
    {
        Id = id;
        UserId = userId;
        Temporada = temporada;
        TarifaLicenciaFederativaId = tarifaLicenciaFederativaId;
        Estado = estado;
        FechaCreacionUtc = fechaCreacionUtc;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public int Temporada { get; private set; }
    public int TarifaLicenciaFederativaId { get; private set; }
    public EstadoSolicitudLicenciaFederativa Estado { get; private set; }
    public DateTime FechaCreacionUtc { get; private set; }
    public TarifaLicenciaFederativa? TarifaLicenciaFederativa { get; private set; }

    public static SolicitudLicenciaFederativa Crear(Guid userId, int temporada, TarifaLicenciaFederativa tarifa)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainException("El usuario de la solicitud es obligatorio.");
        }

        if (temporada <= 0)
        {
            throw new DomainException("La temporada de la solicitud es obligatoria.");
        }

        if (tarifa is null)
        {
            throw new DomainException("La tarifa de la solicitud es obligatoria.");
        }

        if (tarifa.Temporada != temporada)
        {
            throw new DomainException("La tarifa seleccionada no pertenece a la misma temporada de la solicitud.");
        }

        return new SolicitudLicenciaFederativa(
            Guid.NewGuid(),
            userId,
            temporada,
            tarifa.Id,
            EstadoSolicitudLicenciaFederativa.Pendiente,
            DateTime.UtcNow);
    }

    public void Confirmar()
    {
        Estado = EstadoSolicitudLicenciaFederativa.Confirmada;
    }

    public void MarcarPendiente()
    {
        Estado = EstadoSolicitudLicenciaFederativa.Pendiente;
    }

    public void Cancelar()
    {
        Estado = EstadoSolicitudLicenciaFederativa.Cancelada;
    }
}
