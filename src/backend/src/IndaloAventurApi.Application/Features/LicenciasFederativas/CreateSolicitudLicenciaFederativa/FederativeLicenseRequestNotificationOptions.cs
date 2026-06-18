namespace IndaloAventurApi.Application.Features.LicenciasFederativas.CreateSolicitudLicenciaFederativa;

public sealed class FederativeLicenseRequestNotificationOptions
{
    public const string SectionName = "Email";

    public string FederativeLicenseRequestsTo { get; init; } = "club@indaloaventura.com";
}
