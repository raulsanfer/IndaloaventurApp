namespace IndaloaventurApp.SharedUI.Abstractions.Member;

using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Member;

/// <summary>
/// Provides administrator operations for users and member files.
/// </summary>
public interface IAdminUserManagementService
{
    /// <summary>
    /// Gets managed users, optionally filtered by email.
    /// </summary>
    Task<ServiceResult<IReadOnlyList<ManagedUserItem>>> GetUsersAsync(string? email = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single managed user by identifier.
    /// </summary>
    Task<ServiceResult<ManagedUserItem>> GetUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the member file associated with a managed user.
    /// </summary>
    Task<ServiceResult<MemberSelfProfile>> GetMemberFileAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a member file for a managed user.
    /// </summary>
    Task<ServiceResult<MemberSelfProfile>> CreateMemberFileAsync(Guid userId, string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the member file associated with a managed user.
    /// </summary>
    Task<ServiceResult<MemberSelfProfile>> UpdateMemberFileAsync(Guid userId, UpdateMemberSelfProfileRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivates a managed user account.
    /// </summary>
    Task<ServiceResult<bool>> DeactivateUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reactivates a managed user account.
    /// </summary>
    Task<ServiceResult<bool>> ReactivateUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
