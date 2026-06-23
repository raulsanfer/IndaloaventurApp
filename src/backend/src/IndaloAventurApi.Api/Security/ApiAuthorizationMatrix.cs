namespace IndaloAventurApi.Api.Security;

/// <summary>
/// Reune la matriz de autorizacion documentada para las rutas de la API.
/// </summary>
public static class ApiAuthorizationMatrix
{
    /// <summary>
    /// Coleccion de reglas de acceso conocidas para cada endpoint documentado.
    /// </summary>
    public static IReadOnlyCollection<ApiAuthorizationRule> Rules { get; } =
    [
        new("POST", "/api/auth/register", ApiAccessClassification.Anonymous, "Publico", "Registro anonimo."),
        new("POST", "/api/auth/login", ApiAccessClassification.Anonymous, "Publico", "Login anonimo."),
        new("POST", "/api/auth/social-login", ApiAccessClassification.Anonymous, "Publico", "Login social anonimo."),
        new("POST", "/api/auth/passrecovery", ApiAccessClassification.Anonymous, "Publico", "Solicitud publica de recuperacion."),
        new("POST", "/api/auth/reset-password", ApiAccessClassification.Anonymous, "Publico", "Reseteo publico con token."),

        new("GET", "/api/agenda-telefonica", ApiAccessClassification.Authenticated, "Policy: Authenticated", "Lectura para cualquier usuario autenticado."),
        new("GET", "/api/agenda-telefonica/{id:guid}", ApiAccessClassification.Authenticated, "Policy: Authenticated", "Detalle para cualquier usuario autenticado."),
        new("POST", "/api/agenda-telefonica", ApiAccessClassification.Admin, "Policy: Admin", "Alta solo administracion."),
        new("PUT", "/api/agenda-telefonica/{id:guid}", ApiAccessClassification.Admin, "Policy: Admin", "Edicion solo administracion."),
        new("DELETE", "/api/agenda-telefonica/{id:guid}", ApiAccessClassification.Admin, "Policy: Admin", "Borrado solo administracion."),

        new("GET", "/api/cargos", ApiAccessClassification.Admin, "Policy: Admin", "Catalogo interno de cargos."),
        new("POST", "/api/cargos", ApiAccessClassification.Admin, "Policy: Admin", "Alta de cargos."),
        new("PUT", "/api/cargos/{id:int}", ApiAccessClassification.Admin, "Policy: Admin", "Edicion de cargos."),
        new("DELETE", "/api/cargos/{id:int}", ApiAccessClassification.Admin, "Policy: Admin", "Borrado de cargos."),

        new("POST", "/api/fichas-socio/{userId:guid}", ApiAccessClassification.Admin, "Policy: Admin", "Alta de ficha para un tercero."),
        new("GET", "/api/fichas-socio/me", ApiAccessClassification.Authenticated, "Policy: Authenticated", "Autoservicio del titular."),
        new("GET", "/api/fichas-socio/{userId:guid}", ApiAccessClassification.OwnerOrAdmin, "Handler: OwnerOrAdmin", "Consulta de ficha propia o por administracion."),
        new("PUT", "/api/fichas-socio/me", ApiAccessClassification.Authenticated, "Policy: Authenticated", "Edicion autoservicio del titular."),
        new("PUT", "/api/fichas-socio/{userId:guid}", ApiAccessClassification.OwnerOrAdmin, "Handler: OwnerOrAdmin", "Edicion de ficha propia o por administracion."),

        new("GET", "/api/licencias-federativas/tarifas", ApiAccessClassification.Authenticated, "Policy: Authenticated", "Consulta de tarifas para cualquier autenticado."),
        new("POST", "/api/licencias-federativas/me/solicitudes", ApiAccessClassification.ClubMember, "Policy: ClubMember", "Solo socios reales con claim IsMember."),
        new("GET", "/api/licencias-federativas/me/solicitudes", ApiAccessClassification.Authenticated, "Policy: Authenticated", "Listado autoservicio del usuario autenticado."),
        new("GET", "/api/licencias-federativas/me/solicitudes/{solicitudId:guid}", ApiAccessClassification.Authenticated, "Policy: Authenticated", "Detalle autoservicio resuelto por identidad del token."),
        new("GET", "/api/licencias-federativas/admin/solicitudes", ApiAccessClassification.Admin, "Policy: Admin", "Consulta administrativa global."),
        new("PUT", "/api/licencias-federativas/admin/users/{userId:guid}/solicitudes/{solicitudId:guid}/estado", ApiAccessClassification.Admin, "Policy: Admin", "Cambio administrativo de estado."),

        new("GET", "/api/signals", ApiAccessClassification.Authenticated, "Policy: Authenticated", "Busqueda para cualquier autenticado."),
        new("GET", "/api/signals/{id:guid}", ApiAccessClassification.Authenticated, "Policy: Authenticated", "Detalle para cualquier autenticado."),
        new("GET", "/api/signals/{id:guid}/images", ApiAccessClassification.Authenticated, "Policy: Authenticated", "Imagenes para cualquier autenticado."),
        new("GET", "/api/signals/{id:guid}/comments", ApiAccessClassification.Authenticated, "Policy: Authenticated", "Comentarios para cualquier autenticado."),
        new("POST", "/api/signals", ApiAccessClassification.Authenticated, "Policy: MemberOrAdmin", "Creacion permitida a Admin o rol tecnico Member."),
        new("POST", "/api/signals/{id:guid}/comments", ApiAccessClassification.Authenticated, "Policy: Authenticated", "Comentario disponible para cualquier autenticado."),
        new("PUT", "/api/signals/{id:guid}", ApiAccessClassification.OwnerOrAdmin, "Policy: MemberOrAdmin + handler propietario/admin", "Edicion restringida por rol tecnico y titularidad."),
        new("DELETE", "/api/signals/{id:guid}", ApiAccessClassification.Authenticated, "Policy: MemberOrAdmin", "Operacion bloqueada funcionalmente, pero protegida por rol tecnico."),

        new("GET", "/api/signal-types", ApiAccessClassification.Authenticated, "Policy: Authenticated", "Lectura para cualquier autenticado."),
        new("POST", "/api/signal-types", ApiAccessClassification.Admin, "Policy: Admin", "Alta solo administracion."),
        new("PUT", "/api/signal-types/{id:int}", ApiAccessClassification.Admin, "Policy: Admin", "Edicion solo administracion."),
        new("DELETE", "/api/signal-types/{id:int}", ApiAccessClassification.Admin, "Policy: Admin", "Borrado solo administracion."),

        new("GET", "/api/users", ApiAccessClassification.Admin, "Policy: Admin", "Listado de usuarios gestionados."),
        new("POST", "/api/users", ApiAccessClassification.Admin, "Policy: Admin", "Alta de usuario administrada."),
        new("PUT", "/api/users/{userId:guid}", ApiAccessClassification.Admin, "Policy: Admin", "Edicion de usuario administrada."),
        new("POST", "/api/users/{userId:guid}/deactivate", ApiAccessClassification.Admin, "Policy: Admin", "Desactivacion logica."),
        new("POST", "/api/users/{userId:guid}/reactivate", ApiAccessClassification.Admin, "Policy: Admin", "Reactivacion logica."),

        new("GET", "/api/wordpress/posts", ApiAccessClassification.Authenticated, "Policy: Authenticated", "Listado de posts para cualquier autenticado."),
        new("GET", "/api/wordpress/posts/{slug}", ApiAccessClassification.Authenticated, "Policy: Authenticated", "Detalle de post para cualquier autenticado.")
    ];
}
