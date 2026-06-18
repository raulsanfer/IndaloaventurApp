namespace IndaloaventurApp.SharedUI.Models.Member;

public sealed record ManagedUserItem(
    Guid UserId,
    string Email,
    bool IsMember,
    bool IsActive,
    IReadOnlyList<string> Roles);
