namespace IndaloaventurApp.SharedUI.Models.Member;

using IndaloaventurApp.SharedUI.Models.Cargos;

public sealed record MemberCargoFieldState(
    string DisplayName,
    IReadOnlyList<CargoSelectionOption> Options,
    bool CanEdit,
    bool HasOptions);
