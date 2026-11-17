namespace IndaloaventurApp.SharedUI.Models.Licenses;

/// <summary>A read-only license type from the authenticated API catalog.</summary>
/// <param name="Id">Contract identifier: 1 Completa, 2 Media, 3 Día.</param>
/// <param name="Nombre">Display name supplied by the API.</param>
public sealed record FederativeLicenseType(int Id, string Nombre);
