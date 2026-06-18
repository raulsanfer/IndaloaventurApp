namespace IndaloaventurApp.SharedUI.Models.Signals;

public sealed class SignalCreateDraft
{
    public int? TypeId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Tags { get; set; } = string.Empty;

    public float? Latitude { get; set; }

    public float? Longitude { get; set; }

    public SignalPhotoDraft? Photo1 { get; set; }

    public SignalPhotoDraft? Photo2 { get; set; }
}
