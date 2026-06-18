namespace IndaloaventurApp.SharedUI.Models.Member;

public sealed record MemberProfile(
    Guid UserId,
    string FullName,
    string? Email,
    bool IsMember,
    int? CargoId,
    string? CargoLabel,
    string MembershipLabel);
