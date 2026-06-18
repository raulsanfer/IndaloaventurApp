namespace IndaloaventurApp.SharedUI.Models.Signals;

public sealed record SignalCreateRequest(
    float Latitude,
    float Longitude,
    string Title,
    string Description,
    byte[] Photo1,
    byte[] Photo2,
    bool IsActive,
    int TypeId,
    string Tags);
