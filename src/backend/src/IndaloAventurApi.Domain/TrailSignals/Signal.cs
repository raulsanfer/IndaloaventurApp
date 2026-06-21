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
        string foto1Path,
        string foto2Path,
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
        Foto1Path = foto1Path;
        Foto2Path = foto2Path;
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
    public string Foto1Path { get; private set; } = string.Empty;
    public string Foto2Path { get; private set; } = string.Empty;
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
        Guid id,
        float latitud,
        float longitud,
        string titulo,
        string descripcion,
        string foto1Path,
        string? foto2Path,
        bool activo,
        Guid userIdAlta,
        int tipo,
        string tags,
        DateTime? fechaAlta = null)
    {
        var foto2Normalizada = foto2Path ?? string.Empty;
        ValidarCreacion(id, titulo, descripcion, foto1Path, userIdAlta, tipo, tags);

        var now = (fechaAlta ?? DateTime.UtcNow).ToUniversalTime();
        return new Signal(id, latitud, longitud, titulo.Trim(), descripcion.Trim(), foto1Path.Trim(), foto2Normalizada.Trim(), activo, userIdAlta, now, now, userIdAlta, tipo, tags.Trim());
    }

    public void Actualizar(
        float latitud,
        float longitud,
        string titulo,
        string descripcion,
        bool activo,
        Guid userIdModificacion,
        DateTime? fechaModificacion = null)
    {
        ValidarEdicionPropia(titulo, descripcion, userIdModificacion);

        Latitud = latitud;
        Longitud = longitud;
        Titulo = titulo.Trim();
        Descripcion = descripcion.Trim();
        Activo = activo;
        UserIdModificacion = userIdModificacion;
        FechaModificacion = (fechaModificacion ?? DateTime.UtcNow).ToUniversalTime();
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

    public void ActualizarImagenes(string foto1Path, string foto2Path, Guid userIdModificacion, DateTime? fechaModificacion = null)
    {
        ValidarImagenes(foto1Path, userIdModificacion);

        Foto1Path = foto1Path.Trim();
        Foto2Path = foto2Path.Trim();
        UserIdModificacion = userIdModificacion;
        FechaModificacion = (fechaModificacion ?? DateTime.UtcNow).ToUniversalTime();
    }

    private static void ValidarCreacion(Guid id, string titulo, string descripcion, string foto1Path, Guid userId, int tipo, string tags)
    {
        if (id == Guid.Empty) throw new DomainException("El identificador de la senal es obligatorio.");
        ValidarComun(titulo, descripcion, foto1Path, userId, tipo, tags);
    }

    private static void ValidarEdicionPropia(string titulo, string descripcion, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(titulo)) throw new DomainException("El titulo de la senal es obligatorio.");
        if (string.IsNullOrWhiteSpace(descripcion)) throw new DomainException("La descripcion de la senal es obligatoria.");
        if (userId == Guid.Empty) throw new DomainException("El usuario de operacion de la senal es obligatorio.");
    }

    private static void ValidarComun(string titulo, string descripcion, string foto1Path, Guid userId, int tipo, string tags)
    {
        if (string.IsNullOrWhiteSpace(titulo)) throw new DomainException("El titulo de la senal es obligatorio.");
        if (string.IsNullOrWhiteSpace(descripcion)) throw new DomainException("La descripcion de la senal es obligatoria.");
        if (string.IsNullOrWhiteSpace(foto1Path)) throw new DomainException("La foto 1 de la senal es obligatoria.");
        if (userId == Guid.Empty) throw new DomainException("El usuario de operacion de la senal es obligatorio.");
        if (tipo <= 0) throw new DomainException("El tipo de senal es obligatorio.");
        if (string.IsNullOrWhiteSpace(tags)) throw new DomainException("Las etiquetas de la senal son obligatorias.");
    }

    private static void ValidarImagenes(string foto1Path, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(foto1Path)) throw new DomainException("La foto 1 de la senal es obligatoria.");
        if (userId == Guid.Empty) throw new DomainException("El usuario de operacion de la senal es obligatorio.");
    }
}
