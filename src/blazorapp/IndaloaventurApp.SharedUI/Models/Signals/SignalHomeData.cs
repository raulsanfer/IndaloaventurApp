namespace IndaloaventurApp.SharedUI.Models.Signals;

public sealed record SignalHomeData(
    IReadOnlyList<SignalCategoryItem> Categories,
    IReadOnlyList<SignalCardItem> Signals);
