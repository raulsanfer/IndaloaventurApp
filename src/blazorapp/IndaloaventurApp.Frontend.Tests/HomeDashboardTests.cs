namespace IndaloaventurApp.Frontend.Tests;

using Bunit;
using IndaloaventurApp.SharedUI.Abstractions.WordPress;
using IndaloaventurApp.SharedUI.Components.Home;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.WordPress;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

public sealed class HomeDashboardTests : BunitContext
{
    [Fact]
    public void HomeDashboard_RendersFoodAlertsAccess_WithIconAndDestination()
    {
        var wordPressService = new RecordingWordPressPostService
        {
            GetLatestPostsHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<WordPressPost>>.Success(Array.Empty<WordPressPost>()))
        };

        Services.AddSingleton<IWordPressPostService>(wordPressService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();

        var cut = Render<HomeDashboard>();

        Assert.Contains("/alertas-alimentarias", cut.Markup);
        Assert.Contains("food_alerts_home_title", cut.Markup);
        Assert.DoesNotContain("home_card_activities_title", cut.Markup);
        Assert.Contains("bi bi-basket2", cut.Markup);
    }
}
