namespace IndaloAventurApi.Api.Security;

/// <summary>
/// Nombres centralizados de las políticas de limitación de frecuencia de la API.
/// </summary>
public static class ApiRateLimitPolicies
{
    /// <summary>
    /// Limita cambios administrativos de contraseña por actor y usuario objetivo.
    /// </summary>
    public const string AdministrativePasswordChange = "AdministrativePasswordChange";
}
