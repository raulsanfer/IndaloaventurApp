namespace IndaloaventurApp.SharedUI.Abstractions.Member;

using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Member;

public interface IAdminUserManagementService
{
    Task<ServiceResult<IReadOnlyList<ManagedUserItem>>> GetUsersAsync(string? email = null, CancellationToken cancellationToken = default);

    Task<ServiceResult<ManagedUserItem>> GetUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<ServiceResult<MemberSelfProfile>> GetMemberFileAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<ServiceResult<MemberSelfProfile>> CreateMemberFileAsync(Guid userId, string email, CancellationToken cancellationToken = default);

    Task<ServiceResult<MemberSelfProfile>> UpdateMemberFileAsync(Guid userId, UpdateMemberSelfProfileRequest request, CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> DeactivateUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> ReactivateUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
