using IndaloAventurApi.Domain.Abstractions;

namespace IndaloAventurApi.Domain.TrailSignals;

public sealed class SignalComment : Entity
{
    private SignalComment()
    {
    }

    private SignalComment(Guid id, Guid signalId, Guid userId, DateTime fechaComentario, string texto)
    {
        Id = id;
        SignalId = signalId;
        UserId = userId;
        FechaComentario = fechaComentario;
        Texto = texto;
    }

    public Guid Id { get; private set; }
    public Guid SignalId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime FechaComentario { get; private set; }
    public string Texto { get; private set; } = string.Empty;
    public Signal? Signal { get; private set; }

    internal static SignalComment Crear(Guid signalId, Guid userId, string texto, DateTime? fechaComentario = null)
    {
        if (signalId == Guid.Empty) throw new DomainException("La senal del comentario es obligatoria.");
        if (userId == Guid.Empty) throw new DomainException("El usuario del comentario es obligatorio.");
        if (string.IsNullOrWhiteSpace(texto)) throw new DomainException("El texto del comentario es obligatorio.");

        var now = (fechaComentario ?? DateTime.UtcNow).ToUniversalTime();
        return new SignalComment(Guid.NewGuid(), signalId, userId, now, texto.Trim());
    }
}
