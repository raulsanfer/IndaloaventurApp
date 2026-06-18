namespace IndaloaventurApp.SharedUI.Components.Home;

using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

public partial class HomeDashboard
{
    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;
}
