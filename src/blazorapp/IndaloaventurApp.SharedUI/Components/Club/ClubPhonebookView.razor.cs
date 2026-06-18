namespace IndaloaventurApp.SharedUI.Components.Club;

using IndaloaventurApp.SharedUI.Abstractions.Phonebook;
using IndaloaventurApp.SharedUI.Models.Phonebook;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

public partial class ClubPhonebookView
{
    [Inject]
    private IPhonebookService PhonebookService { get; set; } = default!;

    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;

    protected IReadOnlyList<PhonebookContact> Contacts { get; private set; } = Array.Empty<PhonebookContact>();

    protected bool IsLoading { get; private set; } = true;

    protected string? ErrorMessage { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        var result = await PhonebookService.GetContactsAsync();
        IsLoading = false;

        if (result.IsSuccess)
        {
            Contacts = result.Value ?? Array.Empty<PhonebookContact>();
            return;
        }

        ErrorMessage = L["mi_club_phonebook_error"];
    }

    protected static string SanitizePhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            return string.Empty;
        }

        return phone.Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal);
    }
}
