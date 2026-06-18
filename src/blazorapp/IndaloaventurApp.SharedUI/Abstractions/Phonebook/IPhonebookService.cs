namespace IndaloaventurApp.SharedUI.Abstractions.Phonebook;

using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Phonebook;

public interface IPhonebookService
{
    Task<ServiceResult<IReadOnlyList<PhonebookContact>>> GetContactsAsync(CancellationToken cancellationToken = default);
}
