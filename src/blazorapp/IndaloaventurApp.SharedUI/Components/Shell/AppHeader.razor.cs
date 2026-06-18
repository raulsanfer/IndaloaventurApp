namespace IndaloaventurApp.SharedUI.Components.Shell;

using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

public partial class AppHeader
{
    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;
}
