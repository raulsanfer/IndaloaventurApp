namespace IndaloaventurApp.SharedUI.Components.Home;

using IndaloaventurApp.SharedUI.Abstractions.WordPress;
using IndaloaventurApp.SharedUI.Models.WordPress;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

public partial class HomeNewsSection
{
    [Inject]
    private IWordPressPostService WordPressPostService { get; set; } = default!;

    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;

    protected IReadOnlyList<WordPressPost> Posts { get; private set; } = Array.Empty<WordPressPost>();

    protected bool IsLoading { get; private set; } = true;

    protected string? ErrorMessage { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        var result = await WordPressPostService.GetLatestPostsAsync();
        IsLoading = false;

        if (result.IsSuccess)
        {
            Posts = result.Value ?? Array.Empty<WordPressPost>();
            return;
        }

        ErrorMessage = L["home_news_error"];
    }

    protected static string GetPostHref(WordPressPost post)
    {
        return $"/noticias/{Uri.EscapeDataString(post.Slug)}";
    }
}
