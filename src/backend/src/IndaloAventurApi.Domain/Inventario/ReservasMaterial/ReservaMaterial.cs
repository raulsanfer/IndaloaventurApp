using IndaloAventurApi.Domain.Abstractions;

namespace IndaloAventurApi.Domain.Inventario.ReservasMaterial;

public sealed class ReservaMaterial : Entity, IAggregateRoot
{
    private ReservaMaterial()
    {
    }

    private ReservaMaterial(
        Guid id,
        Guid userId,
        int inventarioMaterialId,
        int cantidadSolicitada,
        DateTime fechaSolicitudUtc,
        DateOnly fechaRetiradaPrevista,
        DateOnly fechaDevolucionEstimada,
        string? observaciones)
    {
        Id = id;
        UserId = userId;
        InventarioMaterialId = inventarioMaterialId;
        CantidadSolicitada = cantidadSolicitada;
        FechaSolicitudUtc = fechaSolicitudUtc;
        FechaRetiradaPrevista = fechaRetiradaPrevista;
        FechaDevolucionEstimada = fechaDevolucionEstimada;
        Observaciones = observaciones;
        Estado = EstadoReservaMaterial.Solicitada;
        ConcurrencyToken = Guid.NewGuid();
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public int InventarioMaterialId { get; private set; }
    public int CantidadSolicitada { get; private set; }
    public DateTime FechaSolicitudUtc { get; private set; }
    public DateOnly FechaRetiradaPrevista { get; private set; }
    public DateOnly? FechaRetirada { get; private set; }
    public DateOnly FechaDevolucionEstimada { get; private set; }
    public DateOnly? FechaDevolucionEfectiva { get; private set; }
    public string? Observaciones { get; private set; }
    public string? ObservacionAdministracion { get; private set; }
    public EstadoReservaMaterial Estado { get; private set; }
    public bool ConfirmacionDevolucionSocio { get; private set; }
    public bool ConfirmacionDevolucionAdmin { get; private set; }
    public Guid ConcurrencyToken { get; private set; }

    public static ReservaMaterial Crear(
        Guid userId,
        int inventarioMaterialId,
        int cantidadSolicitada,
        DateOnly fechaRetiradaPrevista,
        DateOnly fechaDevolucionEstimada,
        string? observaciones,
        DateTime? fechaSolicitudUtc = null)
    {
        ValidarSolicitud(userId, inventarioMaterialId, cantidadSolicitada, fechaRetiradaPrevista, fechaDevolucionEstimada);

        var fechaUtc = fechaSolicitudUtc ?? DateTime.UtcNow;
        if (fechaUtc.Kind == DateTimeKind.Local)
        {
            fechaUtc = fechaUtc.ToUniversalTime();
        }

        return new ReservaMaterial(
            Guid.NewGuid(),
            userId,
            inventarioMaterialId,
            cantidadSolicitada,
            DateTime.SpecifyKind(fechaUtc, DateTimeKind.Utc),
            fechaRetiradaPrevista,
            fechaDevolucionEstimada,
            InventarioMaterialTexto.Optional(observaciones, "observaciones", InventarioMaterialTexto.ObservacionesMaxLength, true));
    }

    public void ActualizarSolicitud(
        int inventarioMaterialId,
        int cantidadSolicitada,
        DateOnly fechaRetiradaPrevista,
        DateOnly fechaDevolucionEstimada,
        string? observaciones)
    {
        ExigirEstado(EstadoReservaMaterial.Solicitada, "Solo se puede editar una reserva solicitada.");
        ValidarSolicitud(UserId, inventarioMaterialId, cantidadSolicitada, fechaRetiradaPrevista, fechaDevolucionEstimada);

        InventarioMaterialId = inventarioMaterialId;
        CantidadSolicitada = cantidadSolicitada;
        FechaRetiradaPrevista = fechaRetiradaPrevista;
        FechaDevolucionEstimada = fechaDevolucionEstimada;
        Observaciones = InventarioMaterialTexto.Optional(observaciones, "observaciones", InventarioMaterialTexto.ObservacionesMaxLength, true);
        Touch();
    }

    public void RegistrarRetirada(DateOnly fechaRetirada)
    {
        ExigirEstado(EstadoReservaMaterial.Solicitada, "Solo se puede registrar la retirada de una reserva solicitada.");

        FechaRetirada = fechaRetirada;
        Estado = EstadoReservaMaterial.Retirada;
        Touch();
    }

    public void CancelarPorSocio()
    {
        ExigirEstado(EstadoReservaMaterial.Solicitada, "Solo se puede cancelar una reserva solicitada.");
        Estado = EstadoReservaMaterial.Cancelada;
        Touch();
    }

    public void CancelarPorAdministracion(string? observacionAdministracion)
    {
        if (Estado is not (EstadoReservaMaterial.Solicitada or EstadoReservaMaterial.Retirada))
        {
            throw new DomainException("No se puede cancelar una reserva finalizada.");
        }

        ObservacionAdministracion = Estado == EstadoReservaMaterial.Retirada
            ? InventarioMaterialTexto.Required(observacionAdministracion, "observacion administrativa", InventarioMaterialTexto.ObservacionesMaxLength, true)
            : InventarioMaterialTexto.Optional(observacionAdministracion, "observacion administrativa", InventarioMaterialTexto.ObservacionesMaxLength, true);
        Estado = EstadoReservaMaterial.Cancelada;
        Touch();
    }

    public void ConfirmarDevolucionSocio()
    {
        ExigirEstado(EstadoReservaMaterial.Retirada, "Solo se puede confirmar la devolucion de una reserva retirada.");
        ConfirmacionDevolucionSocio = true;
        CompletarDevolucionSiConfirmada();
        Touch();
    }

    public void ConfirmarDevolucionAdministracion(DateOnly fechaDevolucionEfectiva)
    {
        ExigirEstado(EstadoReservaMaterial.Retirada, "Solo se puede confirmar la devolucion de una reserva retirada.");
        if (FechaRetirada is null)
        {
            throw new DomainException("La retirada efectiva es obligatoria antes de confirmar la devolucion.");
        }

        if (fechaDevolucionEfectiva < FechaRetirada.Value)
        {
            throw new DomainException("La fecha efectiva de devolucion no puede ser anterior a la retirada.");
        }

        FechaDevolucionEfectiva = fechaDevolucionEfectiva;
        ConfirmacionDevolucionAdmin = true;
        CompletarDevolucionSiConfirmada();
        Touch();
    }

    private void CompletarDevolucionSiConfirmada()
    {
        if (ConfirmacionDevolucionSocio && ConfirmacionDevolucionAdmin && FechaDevolucionEfectiva is not null)
        {
            Estado = EstadoReservaMaterial.Entregada;
        }
    }

    private static void ValidarSolicitud(
        Guid userId,
        int inventarioMaterialId,
        int cantidadSolicitada,
        DateOnly fechaRetiradaPrevista,
        DateOnly fechaDevolucionEstimada)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainException("El usuario de la reserva es obligatorio.");
        }

        if (inventarioMaterialId <= 0)
        {
            throw new DomainException("El material de inventario es obligatorio.");
        }

        if (cantidadSolicitada <= 0)
        {
            throw new DomainException("La cantidad solicitada debe ser mayor que cero.");
        }

        if (fechaDevolucionEstimada < fechaRetiradaPrevista)
        {
            throw new DomainException("La fecha estimada de devolucion no puede ser anterior a la retirada prevista.");
        }
    }

    private void ExigirEstado(EstadoReservaMaterial estadoEsperado, string message)
    {
        if (Estado != estadoEsperado)
        {
            throw new DomainException(message);
        }
    }

    private void Touch() => ConcurrencyToken = Guid.NewGuid();
}
