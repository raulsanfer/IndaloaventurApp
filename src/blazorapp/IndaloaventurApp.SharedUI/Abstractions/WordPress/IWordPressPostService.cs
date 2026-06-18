namespace IndaloaventurApp.SharedUI.Abstractions.WordPress;

using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.WordPress;

public interface IWordPressPostService
{
    Task<ServiceResult<IReadOnlyList<WordPressPost>>> GetLatestPostsAsync(CancellationToken cancellationToken = default);

    Task<ServiceResult<WordPressPost>> GetPostBySlugAsync(string slug, CancellationToken cancellationToken = default);
}
