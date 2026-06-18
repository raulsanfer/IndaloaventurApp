namespace IndaloaventurApp.SharedUI.Abstractions.Signals;

using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Signals;

public interface ISignalService
{
    Task<ServiceResult<SignalHomeData>> GetSignalHomeDataAsync(SignalListQuery query, CancellationToken cancellationToken = default);

    Task<ServiceResult<SignalDetailItem>> GetSignalAsync(Guid signalId, CancellationToken cancellationToken = default);

    Task<ServiceResult<IReadOnlyList<SignalCommentItem>>> GetSignalCommentsAsync(Guid signalId, CancellationToken cancellationToken = default);

    Task<ServiceResult<IReadOnlyList<SignalCategoryItem>>> GetSignalCategoriesAsync(CancellationToken cancellationToken = default);

    Task<ServiceResult<int>> CreateSignalCategoryAsync(CreateSignalCategoryRequest request, CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> UpdateSignalCategoryAsync(UpdateSignalCategoryRequest request, CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> DeleteSignalCategoryAsync(int id, CancellationToken cancellationToken = default);

    Task<ServiceResult<Guid>> CreateSignalAsync(SignalCreateRequest request, CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> UpdateSignalAsync(UpdateSignalRequest request, CancellationToken cancellationToken = default);
}
