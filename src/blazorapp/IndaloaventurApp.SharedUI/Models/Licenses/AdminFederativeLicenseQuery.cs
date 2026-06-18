namespace IndaloaventurApp.SharedUI.Models.Licenses;

public sealed record AdminFederativeLicenseQuery(
    Guid? UserId = null,
    int? Temporada = null,
    string? Estado = null);
