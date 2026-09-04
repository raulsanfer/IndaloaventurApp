namespace IndaloaventurApp.SharedUI.Abstractions.WordPress;

using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.WordPress;

/// <summary>
/// Provides WordPress news posts for the home and detail views.
/// </summary>
public interface IWordPressPostService
{
    /// <summary>
    /// Gets the latest WordPress posts for the home dashboard.
    /// </summary>
    Task<ServiceResult<IReadOnlyList<WordPressPost>>> GetLatestPostsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a WordPress post by slug.
    /// </summary>
    Task<ServiceResult<WordPressPost>> GetPostBySlugAsync(string slug, CancellationToken cancellationToken = default);
}
