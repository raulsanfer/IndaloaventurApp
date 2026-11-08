namespace IndaloaventurApp.SharedUI.Models.Member;

/// <summary>
/// Represents the complete editable administrative state of a managed user.
/// </summary>
/// <param name="Email">The user's current email address.</param>
/// <param name="IsMember">Whether the user currently meets the member condition.</param>
/// <param name="Roles">The complete set of roles that must remain assigned to the user.</param>
public sealed record UpdateManagedUserRequest(
    string Email,
    bool IsMember,
    IReadOnlyList<string> Roles);
