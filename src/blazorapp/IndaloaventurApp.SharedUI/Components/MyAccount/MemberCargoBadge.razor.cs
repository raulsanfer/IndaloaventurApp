namespace IndaloaventurApp.SharedUI.Components.MyAccount;

using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

public partial class MemberCargoBadge
{
    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;

    [Parameter]
    public string? CargoLabel { get; set; }

    [Parameter]
    public int? CargoId { get; set; }

    protected bool IsVisible => CargoId.HasValue || !string.IsNullOrWhiteSpace(CargoLabel);

    protected string CargoText =>
        !string.IsNullOrWhiteSpace(CargoLabel)
            ? CargoLabel.ToUpperInvariant()
            : string.Format(L["mi_cuenta_cargo_default"], CargoId).ToUpperInvariant();
}
