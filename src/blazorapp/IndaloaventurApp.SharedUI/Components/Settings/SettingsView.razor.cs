namespace IndaloaventurApp.SharedUI.Components.Settings;

using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

public partial class SettingsView
{
    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;

    [Parameter]
    public bool IsAdmin { get; set; }

    [Parameter]
    public string CargoManagementHref { get; set; } = "/configuracion/cargos";

    [Parameter]
    public string UsersManagementHref { get; set; } = "/configuracion/usuarios";

    [Parameter]
    public string FederativeLicensesManagementHref { get; set; } = "/configuracion/licencias-federativas";

    [Parameter]
    public string SignalsManagementHref { get; set; } = "/configuracion/signals";
}
