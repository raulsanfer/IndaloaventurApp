namespace IndaloaventurApp.SharedUI.Components.Shell;

using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

public partial class BottomNav
{
    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;

    [Inject]
    private ISessionService SessionService { get; set; } = default!;

    [Parameter]
    public string ActiveHref { get; set; } = "/home";

    protected IReadOnlyList<NavItem> Items
    {
        get
        {
            var items = new List<NavItem>
            {
                new("/home", "bi bi-house-fill", L["shell_nav_home"]),
                new("/signals", "bi bi-exclamation-triangle-fill", L["shell_nav_signals"])
            };

            if (SessionService.CurrentSession?.IsMember == true)
            {
                items.Add(new("/mi-club", "bi bi-people-fill", L["shell_nav_club"]));
            }

            items.Add(new("/mi-cuenta", "bi bi-person-fill", L["shell_nav_my_account"]));
            return items;
        }
    }

    protected sealed record NavItem(string Href, string IconClass, string Label);
}
