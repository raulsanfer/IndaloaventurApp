using IndaloAventurApi.Application.Abstractions.TrailSignals;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace IndaloAventurApi.Infrastructure.Media;

public sealed class FileSystemSignalImageStorage : ISignalImageStorage
{
    private const string Foto1FileName = "foto1.bin";
    private const string Foto2FileName = "foto2.bin";

    private readonly string _rootPath;

    public FileSystemSignalImageStorage(IOptions<SignalImageStorageOptions> options, IHostEnvironment environment)
    {
        var configuredRoot = options.Value.RootPath?.Trim();
        if (string.IsNullOrWhiteSpace(configuredRoot))
        {
            throw new InvalidOperationException("La configuracion de imagenes de senales requiere 'RootPath'.");
        }

        _rootPath = Path.GetFullPath(
            Path.IsPathRooted(configuredRoot)
                ? configuredRoot
                : Path.Combine(environment.ContentRootPath, configuredRoot));

        Directory.CreateDirectory(_rootPath);
    }

    public async Task<StoredSignalImages> SaveAsync(Guid signalId, byte[] foto1, byte[] foto2, CancellationToken cancellationToken)
    {
        if (foto1.Length == 0)
        {
            throw new InvalidOperationException("La foto 1 de la senal es obligatoria.");
        }

        var signalDirectory = Path.Combine(_rootPath, signalId.ToString("N"));
        Directory.CreateDirectory(signalDirectory);

        var foto1Path = Path.Combine(signalDirectory, Foto1FileName);
        var foto2Path = Path.Combine(signalDirectory, Foto2FileName);

        try
        {
            await File.WriteAllBytesAsync(foto1Path, foto1, cancellationToken);

            if (foto2.Length > 0)
            {
                await File.WriteAllBytesAsync(foto2Path, foto2, cancellationToken);
            }
            else if (File.Exists(foto2Path))
            {
                File.Delete(foto2Path);
            }
        }
        catch
        {
            CleanupPartialFiles(foto1Path, foto2Path, signalDirectory);
            throw;
        }

        return new StoredSignalImages(
            BuildRelativePath(signalId, Foto1FileName),
            foto2.Length > 0 ? BuildRelativePath(signalId, Foto2FileName) : string.Empty);
    }

    public async Task<SignalImageContent> ReadAsync(string foto1Path, string foto2Path, CancellationToken cancellationToken)
    {
        var fullFoto1Path = ResolveAbsolutePath(foto1Path);
        if (!File.Exists(fullFoto1Path))
        {
            throw new InvalidOperationException("No se ha podido resolver la foto 1 de la senal.");
        }

        var foto1 = await File.ReadAllBytesAsync(fullFoto1Path, cancellationToken);
        byte[] foto2 = [];

        if (!string.IsNullOrWhiteSpace(foto2Path))
        {
            var fullFoto2Path = ResolveAbsolutePath(foto2Path);
            if (!File.Exists(fullFoto2Path))
            {
                throw new InvalidOperationException("No se ha podido resolver la foto 2 de la senal.");
            }

            foto2 = await File.ReadAllBytesAsync(fullFoto2Path, cancellationToken);
        }

        return new SignalImageContent(foto1, foto2);
    }

    public async Task<StoredSignalImages> ReplaceAsync(Guid signalId, string currentFoto1Path, string currentFoto2Path, byte[] foto1, byte[] foto2, CancellationToken cancellationToken)
    {
        var storedImages = await SaveAsync(signalId, foto1, foto2, cancellationToken);

        var obsoleteFoto1Path = string.Equals(currentFoto1Path, storedImages.Foto1Path, StringComparison.Ordinal)
            ? string.Empty
            : currentFoto1Path;
        var obsoleteFoto2Path = string.Equals(currentFoto2Path, storedImages.Foto2Path, StringComparison.Ordinal)
            ? string.Empty
            : currentFoto2Path;

        await DeleteAsync(obsoleteFoto1Path, obsoleteFoto2Path, cancellationToken);
        return storedImages;
    }

    public Task DeleteAsync(string foto1Path, string foto2Path, CancellationToken cancellationToken)
    {
        DeleteIfExists(foto1Path);
        DeleteIfExists(foto2Path);

        var commonDirectory = TryGetCommonDirectory(foto1Path, foto2Path);
        if (!string.IsNullOrWhiteSpace(commonDirectory) &&
            Directory.Exists(commonDirectory) &&
            !Directory.EnumerateFileSystemEntries(commonDirectory).Any())
        {
            Directory.Delete(commonDirectory);
        }

        return Task.CompletedTask;
    }

    private string ResolveAbsolutePath(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return string.Empty;
        }

        var combined = Path.GetFullPath(Path.Combine(_rootPath, relativePath));
        if (!combined.StartsWith(_rootPath, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("La ruta de imagen de la senal no es valida.");
        }

        return combined;
    }

    private void DeleteIfExists(string relativePath)
    {
        var absolutePath = ResolveAbsolutePath(relativePath);
        if (!string.IsNullOrWhiteSpace(absolutePath) && File.Exists(absolutePath))
        {
            File.Delete(absolutePath);
        }
    }

    private string? TryGetCommonDirectory(string foto1Path, string foto2Path)
    {
        var first = ResolveAbsolutePath(foto1Path);
        if (string.IsNullOrWhiteSpace(first))
        {
            return null;
        }

        return Path.GetDirectoryName(first);
    }

    private static string BuildRelativePath(Guid signalId, string fileName)
        => Path.Combine(signalId.ToString("N"), fileName).Replace('\\', '/');

    private static void CleanupPartialFiles(string foto1Path, string foto2Path, string signalDirectory)
    {
        if (File.Exists(foto1Path))
        {
            File.Delete(foto1Path);
        }

        if (File.Exists(foto2Path))
        {
            File.Delete(foto2Path);
        }

        if (Directory.Exists(signalDirectory) && !Directory.EnumerateFileSystemEntries(signalDirectory).Any())
        {
            Directory.Delete(signalDirectory);
        }
    }
}
