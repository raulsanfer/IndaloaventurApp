namespace IndaloAventurApi.Api.Security;

/// <summary>
/// Describe una regla de autorizacion aplicada a una ruta documentada de la API.
/// </summary>
/// <param name="HttpMethod">Metodo HTTP protegido por la regla.</param>
/// <param name="Route">Ruta relativa del endpoint.</param>
/// <param name="Classification">Nivel de acceso requerido.</param>
/// <param name="Enforcement">Politica o mecanismo concreto que aplica la seguridad.</param>
/// <param name="Notes">Notas adicionales para auditoria o documentacion.</param>
public sealed record ApiAuthorizationRule(
    string HttpMethod,
    string Route,
    ApiAccessClassification Classification,
    string Enforcement,
    string Notes);
