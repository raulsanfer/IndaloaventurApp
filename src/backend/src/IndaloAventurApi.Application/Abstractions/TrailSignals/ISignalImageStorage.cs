namespace IndaloAventurApi.Application.Abstractions.TrailSignals;

public interface ISignalImageStorage
{
    Task<StoredSignalImages> SaveAsync(Guid signalId, byte[] foto1, byte[] foto2, CancellationToken cancellationToken);
    Task<StoredSignalImages> ReplaceAsync(Guid signalId, string currentFoto1Path, string currentFoto2Path, byte[] foto1, byte[] foto2, CancellationToken cancellationToken);
    Task<SignalImageContent> ReadAsync(string foto1Path, string foto2Path, CancellationToken cancellationToken);
    Task DeleteAsync(string foto1Path, string foto2Path, CancellationToken cancellationToken);
}

public sealed record StoredSignalImages(string Foto1Path, string Foto2Path);

public sealed record SignalImageContent(byte[] Foto1, byte[] Foto2);
