namespace IndaloaventurApp.SharedUI.Models.Licenses;

public sealed record UpdateAdminFederativeLicenseStatusRequest(
    Guid UserId,
    Guid SolicitudId,
    string Estado);
