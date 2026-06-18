namespace IndaloaventurApp.SharedUI.Abstractions.Member;

using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Member;

public interface IMemberProfileService
{
    Task<ServiceResult<MemberProfile>> GetMyProfileAsync(CancellationToken cancellationToken = default);

    Task<ServiceResult<MemberSelfProfile>> GetMyMemberFileAsync(CancellationToken cancellationToken = default);

    Task<ServiceResult<MemberSelfProfile>> UpdateMyMemberFileAsync(UpdateMemberSelfProfileRequest request, CancellationToken cancellationToken = default);
}
