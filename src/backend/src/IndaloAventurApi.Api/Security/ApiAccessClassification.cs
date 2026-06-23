namespace IndaloAventurApi.Api.Security;

/// <summary>
/// Clasifica el nivel de acceso requerido por una ruta de la API.
/// </summary>
public enum ApiAccessClassification
{
    /// <summary>
    /// La ruta es publica y no requiere autenticacion.
    /// </summary>
    Anonymous,
    /// <summary>
    /// La ruta requiere un usuario autenticado.
    /// </summary>
    Authenticated,
    /// <summary>
    /// La ruta requiere privilegios de administrador.
    /// </summary>
    Admin,
    /// <summary>
    /// La ruta permite acceso al propietario del recurso o a un administrador.
    /// </summary>
    OwnerOrAdmin,
    /// <summary>
    /// La ruta requiere que el usuario sea socio del club.
    /// </summary>
    ClubMember
}
