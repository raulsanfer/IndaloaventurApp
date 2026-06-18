namespace IndaloaventurApp.SharedUI.Components.Settings;

using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

public partial class SignalSettingsHubView
{
    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;
}
