namespace IndaloaventurApp.SharedUI.Models.WordPress;

public sealed record WordPressPost(
    long Id,
    string Slug,
    string Title,
    string? Summary,
    string? ContentHtml,
    string? FeaturedImageUrl,
    string? Link,
    DateTimeOffset PublishedAtUtc);
