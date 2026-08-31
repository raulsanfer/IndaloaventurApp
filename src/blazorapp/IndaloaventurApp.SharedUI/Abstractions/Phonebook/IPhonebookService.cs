namespace IndaloaventurApp.SharedUI.Abstractions.Phonebook;

using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Phonebook;

/// <summary>
/// Provides club phonebook contacts for the Mi Club area.
/// </summary>
public interface IPhonebookService
{
    /// <summary>
    /// Gets all phonebook contacts exposed by the backend.
    /// </summary>
    Task<ServiceResult<IReadOnlyList<PhonebookContact>>> GetContactsAsync(CancellationToken cancellationToken = default);
}
