namespace IndaloaventurApp.SharedUI.Components.Club;

using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

public partial class ClubIndexView
{
    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;

    [Inject]
    private ISessionService SessionService { get; set; } = default!;

    protected bool CanAccessFederativeLicenses =>
        SessionService.CurrentSession?.CanAccessOwnFederativeLicenses() == true;
}
