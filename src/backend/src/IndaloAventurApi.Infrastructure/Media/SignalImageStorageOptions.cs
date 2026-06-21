namespace IndaloAventurApi.Infrastructure.Media;

public sealed class SignalImageStorageOptions
{
    public const string SectionName = "SignalImageStorage";

    public string RootPath { get; set; } = string.Empty;
}
