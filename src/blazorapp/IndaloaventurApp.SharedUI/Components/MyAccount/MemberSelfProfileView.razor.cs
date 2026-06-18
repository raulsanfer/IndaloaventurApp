namespace IndaloaventurApp.SharedUI.Components.MyAccount;

using IndaloaventurApp.SharedUI.Abstractions.Member;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Abstractions.Cargos;
using IndaloaventurApp.SharedUI.Components;
using IndaloaventurApp.SharedUI.Models.Member;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;

public partial class MemberSelfProfileView
{
    [Inject]
    private IMemberProfileService MemberProfileService { get; set; } = default!;

    [Inject]
    private ISessionService SessionService { get; set; } = default!;

    [Inject]
    private ICargoAdminService CargoAdminService { get; set; } = default!;

    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;

    protected MemberSelfProfileFormModel? Form { get; private set; }

    protected EditContext? EditContext { get; private set; }

    protected bool IsLoading { get; private set; } = true;

    protected bool IsSubmitting { get; private set; }

    protected bool IsNonMember => SessionService.CurrentSession?.IsMember != true;

    protected bool IsAdmin => SessionService.CurrentSession?.IsInRole("Admin") == true;

    protected string? ErrorMessageKey { get; private set; }

    protected string? StatusMessageKey { get; private set; }

    protected MemberCargoFieldState CargoFieldState { get; private set; } =
        new(string.Empty, Array.Empty<IndaloaventurApp.SharedUI.Models.Cargos.CargoSelectionOption>(), false, false);

    protected override async Task OnInitializedAsync()
    {
        if (IsNonMember)
        {
            IsLoading = false;
            return;
        }

        await LoadAsync();
    }

    protected async Task HandleValidSubmitAsync()
    {
        if (Form is null)
        {
            return;
        }

        ClearMessages();
        IsSubmitting = true;

        var request = new UpdateMemberSelfProfileRequest(
            Form.CargoId,
            NormalizeText(Form.Nombre),
            NormalizeText(Form.Apellidos),
            NormalizeIdentifier(Form.Dni),
            Form.FechaNacimiento,
            NormalizeText(Form.Direccion),
            NormalizePostalCode(Form.CodigoPostal),
            NormalizeText(Form.Poblacion),
            NormalizeText(Form.Provincia),
            NormalizePhone(Form.Tlf),
            NormalizeEmail(Form.Email),
            NormalizeText(Form.Alergias),
            Form.AceptaPoliticaPrivacidad,
            Form.AceptaUsoImagenes,
            Form.AceptaCobroCuenta);

        var result = await MemberProfileService.UpdateMyMemberFileAsync(request);
        IsSubmitting = false;

        if (!result.IsSuccess || result.Value is null)
        {
            ErrorMessageKey = result.Error?.Code switch
            {
                "profile.validation" => "member_file_save_validation_error",
                "profile.not_found" => "member_file_not_found",
                _ => "member_file_save_error"
            };
            return;
        }

        await ApplyProfileAsync(result.Value);
        StatusMessageKey = "member_file_save_success";
    }

    private async Task LoadAsync()
    {
        ClearMessages();
        IsLoading = true;

        var result = await MemberProfileService.GetMyMemberFileAsync();

        IsLoading = false;
        if (!result.IsSuccess || result.Value is null)
        {
            ErrorMessageKey = result.Error?.Code switch
            {
                "profile.not_found" => "member_file_not_found",
                _ => "member_file_load_error"
            };
            return;
        }

        await ApplyProfileAsync(result.Value);
    }

    private async Task ApplyProfileAsync(MemberSelfProfile profile)
    {
        Form = MemberSelfProfileFormModel.FromProfile(profile);
        EditContext = new EditContext(Form);
        CargoFieldState = await MemberCargoFieldComponentBase.BuildAsync(CargoAdminService, profile, IsAdmin);
    }

    private void ClearMessages()
    {
        ErrorMessageKey = null;
        StatusMessageKey = null;
    }

    private static string? NormalizeText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var sanitized = new string(value.Trim().Where(character => !char.IsControl(character) && character is not '<' and not '>').ToArray());
        return string.IsNullOrWhiteSpace(sanitized) ? null : sanitized;
    }

    private static string? NormalizeIdentifier(string? value)
        => NormalizeText(value)?.ToUpperInvariant();

    private static string? NormalizePostalCode(string? value)
        => NormalizeText(value)?.Replace(" ", string.Empty, StringComparison.Ordinal);

    private static string? NormalizePhone(string? value)
        => NormalizeText(value);

    private static string? NormalizeEmail(string? value)
        => NormalizeText(value)?.ToLowerInvariant();
}
