namespace IndaloaventurApp.Frontend.Tests;

using System.Text.Json;

public sealed class PwaManifestTests
{
    [Fact]
    public void Manifest_UsesExpectedInstalledAppName()
    {
        var manifestPath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            "IndaloaventurApp.Web",
            "IndaloaventurApp.Web",
            "wwwroot",
            "manifest.webmanifest"));

        Assert.True(File.Exists(manifestPath), $"Manifest file was not found at '{manifestPath}'.");

        using var document = JsonDocument.Parse(File.ReadAllText(manifestPath));
        var root = document.RootElement;

        Assert.Equal("Club Indaloaventura", root.GetProperty("name").GetString());
        Assert.Equal("Club Indaloaventura", root.GetProperty("short_name").GetString());
    }
}
