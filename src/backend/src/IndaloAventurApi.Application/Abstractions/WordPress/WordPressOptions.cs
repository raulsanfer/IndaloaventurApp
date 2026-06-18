namespace IndaloAventurApi.Application.Abstractions.WordPress;

public sealed class WordPressOptions
{
    public const string SectionName = "WordPress";

    public string BaseUrl { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string ApplicationPassword { get; init; } = string.Empty;
    public string PostsEndpoint { get; init; } = "/wp-json/wp/v2/posts";
    public int DefaultPostsPageSize { get; init; } = 10;
    public int TimeoutSeconds { get; init; } = 30;
    public bool DisableCertificateRevocationCheck { get; init; }
}
