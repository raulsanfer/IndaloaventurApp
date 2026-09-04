namespace IndaloaventurApp.SharedUI.Abstractions.Signals;

using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Signals;

/// <summary>
/// Provides signal browsing, detail, comment, category and mutation operations.
/// </summary>
public interface ISignalService
{
    /// <summary>
    /// Gets the combined signal home data for the supplied list query.
    /// </summary>
    Task<ServiceResult<SignalHomeData>> GetSignalHomeDataAsync(SignalListQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the detail information for a signal.
    /// </summary>
    Task<ServiceResult<SignalDetailItem>> GetSignalAsync(Guid signalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the images associated with a signal.
    /// </summary>
    Task<ServiceResult<SignalImagesItem>> GetSignalImagesAsync(Guid signalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets comments associated with a signal.
    /// </summary>
    Task<ServiceResult<IReadOnlyList<SignalCommentItem>>> GetSignalCommentsAsync(Guid signalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a comment for an authorized signal detail view.
    /// </summary>
    Task<ServiceResult<Guid>> CreateSignalCommentAsync(CreateSignalCommentRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the categories available for signal filtering and administration.
    /// </summary>
    Task<ServiceResult<IReadOnlyList<SignalCategoryItem>>> GetSignalCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a signal category.
    /// </summary>
    Task<ServiceResult<int>> CreateSignalCategoryAsync(CreateSignalCategoryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing signal category.
    /// </summary>
    Task<ServiceResult<bool>> UpdateSignalCategoryAsync(UpdateSignalCategoryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an existing signal category.
    /// </summary>
    Task<ServiceResult<bool>> DeleteSignalCategoryAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new signal from the shared UI draft.
    /// </summary>
    Task<ServiceResult<Guid>> CreateSignalAsync(SignalCreateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing signal owned by the current user or allowed role.
    /// </summary>
    Task<ServiceResult<bool>> UpdateSignalAsync(UpdateSignalRequest request, CancellationToken cancellationToken = default);
}
