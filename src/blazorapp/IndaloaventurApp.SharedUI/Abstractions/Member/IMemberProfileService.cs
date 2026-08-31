namespace IndaloaventurApp.SharedUI.Abstractions.Member;

using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Member;

/// <summary>
/// Provides member profile and member file operations for the current user.
/// </summary>
public interface IMemberProfileService
{
    /// <summary>
    /// Gets the lightweight profile for the current authenticated user.
    /// </summary>
    Task<ServiceResult<MemberProfile>> GetMyProfileAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current user's member file.
    /// </summary>
    Task<ServiceResult<MemberSelfProfile>> GetMyMemberFileAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the current user's member file.
    /// </summary>
    Task<ServiceResult<MemberSelfProfile>> UpdateMyMemberFileAsync(UpdateMemberSelfProfileRequest request, CancellationToken cancellationToken = default);
}
