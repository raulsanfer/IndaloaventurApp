namespace IndaloaventurApp.SharedUI.Components.WordPress;

using System.Globalization;
using IndaloaventurApp.SharedUI.Abstractions.WordPress;
using IndaloaventurApp.SharedUI.Models.WordPress;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

public partial class WordPressPostDetailView
{
    [Inject]
    private IWordPressPostService WordPressPostService { get; set; } = default!;

    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;

    [Parameter]
    public string Slug { get; set; } = string.Empty;

    protected WordPressPost? Post { get; private set; }

    protected bool IsLoading { get; private set; } = true;

    protected string? ErrorMessage { get; private set; }

    protected MarkupString ContentMarkup => new(Post?.ContentHtml ?? string.Empty);

    protected override async Task OnParametersSetAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        Post = null;

        var result = await WordPressPostService.GetPostBySlugAsync(Slug);
        IsLoading = false;

        if (result.IsSuccess)
        {
            Post = result.Value;
            return;
        }

        ErrorMessage = L["wordpress_post_error"];
    }

    protected static string FormatDate(DateTimeOffset publishedAtUtc)
    {
        return publishedAtUtc.ToLocalTime().ToString("d 'de' MMMM 'de' yyyy", CultureInfo.GetCultureInfo("es-ES"));
    }
}
