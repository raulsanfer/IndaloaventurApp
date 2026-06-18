using IndaloAventurApi.Domain.Abstractions;

namespace IndaloAventurApi.Domain.TrailSignals;

public sealed class Signal : Entity, IAggregateRoot
{
    private readonly List<SignalComment> _comentarios = [];

    private Signal()
    {
    }

    private Signal(
        Guid id,
        float latitud,
        float longitud,
        string titulo,
        string descripcion,
        byte[] foto1,
        byte[] foto2,
        bool activo,
        Guid userIdAlta,
        DateTime fechaAlta,
        DateTime fechaModificacion,
        Guid userIdModificacion,
        int tipo,
        string tags)
    {
        Id = id;
        Latitud = latitud;
        Longitud = longitud;
        Titulo = titulo;
        Descripcion = descripcion;
        Foto1 = foto1;
        Foto2 = foto2;
        Activo = activo;
        UserIdAlta = userIdAlta;
        FechaAlta = fechaAlta;
        FechaModificacion = fechaModificacion;
        UserIdModificacion = userIdModificacion;
        Tipo = tipo;
        Tags = tags;
    }

    public Guid Id { get; private set; }
    public float Latitud { get; private set; }
    public float Longitud { get; private set; }
    public string Titulo { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;
    public byte[] Foto1 { get; private set; } = [];
    public byte[] Foto2 { get; private set; } = [];
    public bool Activo { get; private set; }
    public Guid UserIdAlta { get; private set; }
    public DateTime FechaAlta { get; private set; }
    public DateTime FechaModificacion { get; private set; }
    public Guid UserIdModificacion { get; private set; }
    public int Tipo { get; private set; }
    public string Tags { get; private set; } = string.Empty;
    public SignalType? SignalType { get; private set; }
    public IReadOnlyCollection<SignalComment> Comentarios => _comentarios;

    public static Signal Crear(
        float latitud,
        float longitud,
        string titulo,
        string descripcion,
        byte[] foto1,
        byte[]? foto2,
        bool activo,
        Guid userIdAlta,
        int tipo,
        string tags,
        DateTime? fechaAlta = null)
    {
        var foto2Normalizada = foto2 ?? [];
        ValidarCreacion(titulo, descripcion, foto1, userIdAlta, tipo, tags);

        var now = (fechaAlta ?? DateTime.UtcNow).ToUniversalTime();
        return new Signal(Guid.NewGuid(), latitud, longitud, titulo.Trim(), descripcion.Trim(), foto1.ToArray(), foto2Normalizada.ToArray(), activo, userIdAlta, now, now, userIdAlta, tipo, tags.Trim());
    }

    public void Actualizar(
        float latitud,
        float longitud,
        string titulo,
        string descripcion,
        byte[] foto1,
        byte[] foto2,
        bool activo,
        Guid userIdModificacion,
        int tipo,
        string tags,
        DateTime? fechaModificacion = null)
    {
        ValidarActualizacion(titulo, descripcion, foto1, foto2, userIdModificacion, tipo, tags);

        Latitud = latitud;
        Longitud = longitud;
        Titulo = titulo.Trim();
        Descripcion = descripcion.Trim();
        Foto1 = foto1.ToArray();
        Foto2 = foto2.ToArray();
        Activo = activo;
        UserIdModificacion = userIdModificacion;
        FechaModificacion = (fechaModificacion ?? DateTime.UtcNow).ToUniversalTime();
        Tipo = tipo;
        Tags = tags.Trim();
    }

    public void ActualizarContenidoPropio(
        string titulo,
        string descripcion,
        bool activo,
        Guid userIdModificacion,
        DateTime? fechaModificacion = null)
    {
        ValidarEdicionPropia(titulo, descripcion, userIdModificacion);

        Titulo = titulo.Trim();
        Descripcion = descripcion.Trim();
        Activo = activo;
        UserIdModificacion = userIdModificacion;
        FechaModificacion = (fechaModificacion ?? DateTime.UtcNow).ToUniversalTime();
    }

    public SignalComment AnadirComentario(string texto, Guid userId, DateTime? fechaComentario = null)
    {
        var comentario = SignalComment.Crear(Id, userId, texto, fechaComentario);
        _comentarios.Add(comentario);
        return comentario;
    }

    private static void ValidarCreacion(string titulo, string descripcion, byte[] foto1, Guid userId, int tipo, string tags)
    {
        ValidarComun(titulo, descripcion, foto1, userId, tipo, tags);
    }

    private static void ValidarActualizacion(string titulo, string descripcion, byte[] foto1, byte[] foto2, Guid userId, int tipo, string tags)
    {
        ValidarComun(titulo, descripcion, foto1, userId, tipo, tags);
        if (foto2.Length == 0) throw new DomainException("La foto 2 de la senal es obligatoria.");
    }

    private static void ValidarEdicionPropia(string titulo, string descripcion, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(titulo)) throw new DomainException("El titulo de la senal es obligatorio.");
        if (string.IsNullOrWhiteSpace(descripcion)) throw new DomainException("La descripcion de la senal es obligatoria.");
        if (userId == Guid.Empty) throw new DomainException("El usuario de operacion de la senal es obligatorio.");
    }

    private static void ValidarComun(string titulo, string descripcion, byte[] foto1, Guid userId, int tipo, string tags)
    {
        if (string.IsNullOrWhiteSpace(titulo)) throw new DomainException("El titulo de la senal es obligatorio.");
        if (string.IsNullOrWhiteSpace(descripcion)) throw new DomainException("La descripcion de la senal es obligatoria.");
        if (foto1.Length == 0) throw new DomainException("La foto 1 de la senal es obligatoria.");
        if (userId == Guid.Empty) throw new DomainException("El usuario de operacion de la senal es obligatorio.");
        if (tipo <= 0) throw new DomainException("El tipo de senal es obligatorio.");
        if (string.IsNullOrWhiteSpace(tags)) throw new DomainException("Las etiquetas de la senal son obligatorias.");
    }
}
