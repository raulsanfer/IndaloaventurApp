using IndaloAventurApi.Infrastructure.Media;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace IndaloAventurApi.IntegrationTests;

public sealed class SignalImageStorageTests : IDisposable
{
    private readonly string _contentRoot = Path.Combine(Path.GetTempPath(), $"indalo-signal-storage-tests-{Guid.NewGuid():N}");

    [Fact]
    public async Task ReplaceAsync_ShouldPersistNewImages_AndDeleteObsoleteLegacyFiles()
    {
        Directory.CreateDirectory(_contentRoot);

        var rootPath = Path.Combine(_contentRoot, "signals");
        Directory.CreateDirectory(Path.Combine(rootPath, "legacy"));
        await File.WriteAllBytesAsync(Path.Combine(rootPath, "legacy", "foto1.bin"), [1]);
        await File.WriteAllBytesAsync(Path.Combine(rootPath, "legacy", "foto2.bin"), [2]);

        var storage = CreateStorage(rootPath);
        var signalId = Guid.NewGuid();

        var stored = await storage.ReplaceAsync(
            signalId,
            "legacy/foto1.bin",
            "legacy/foto2.bin",
            [10, 11],
            [12, 13],
            CancellationToken.None);

        var images = await storage.ReadAsync(stored.Foto1Path, stored.Foto2Path, CancellationToken.None);

        Assert.Equal($"{signalId:N}/foto1.bin", stored.Foto1Path);
        Assert.Equal($"{signalId:N}/foto2.bin", stored.Foto2Path);
        Assert.Equal([10, 11], images.Foto1);
        Assert.Equal([12, 13], images.Foto2);
        Assert.False(File.Exists(Path.Combine(rootPath, "legacy", "foto1.bin")));
        Assert.False(File.Exists(Path.Combine(rootPath, "legacy", "foto2.bin")));
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveSignalDirectory_WhenItBecomesEmpty()
    {
        Directory.CreateDirectory(_contentRoot);

        var rootPath = Path.Combine(_contentRoot, "signals");
        var storage = CreateStorage(rootPath);
        var signalId = Guid.NewGuid();

        var stored = await storage.SaveAsync(signalId, [7], [8], CancellationToken.None);
        await storage.DeleteAsync(stored.Foto1Path, stored.Foto2Path, CancellationToken.None);

        Assert.False(Directory.Exists(Path.Combine(rootPath, signalId.ToString("N"))));
    }

    public void Dispose()
    {
        if (Directory.Exists(_contentRoot))
        {
            Directory.Delete(_contentRoot, recursive: true);
        }
    }

    private FileSystemSignalImageStorage CreateStorage(string rootPath)
    {
        var options = Options.Create(new SignalImageStorageOptions
        {
            RootPath = rootPath
        });

        return new FileSystemSignalImageStorage(options, new TestHostEnvironment(_contentRoot));
    }

    private sealed class TestHostEnvironment(string contentRootPath) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Development";
        public string ApplicationName { get; set; } = "IndaloAventurApi.IntegrationTests";
        public string ContentRootPath { get; set; } = contentRootPath;
        public IFileProvider ContentRootFileProvider { get; set; } = new PhysicalFileProvider(contentRootPath);
    }
}
