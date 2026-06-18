namespace IndaloaventurApp.SharedUI.Models.FoodAlerts;

using System.Net;
using System.Text.RegularExpressions;

public static partial class FoodAlertTextFormatter
{
    public static string NormalizeText(string? value, string fallback = "Sin informacion disponible.")
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return fallback;
        }

        var decoded = RemainingTagsRegex().Replace(value, " ");
        decoded = WebUtility.HtmlDecode(decoded).Trim();
        decoded = MultiWhitespaceRegex().Replace(decoded, " ");

        return string.IsNullOrWhiteSpace(decoded) ? fallback : decoded;
    }

    public static string NormalizeDescription(string? html, string fallback = "Sin descripcion disponible.")
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return fallback;
        }

        var normalized = LineBreakTagRegex().Replace(html, "\n");
        normalized = BlockClosingTagRegex().Replace(normalized, "\n");
        normalized = RemainingTagsRegex().Replace(normalized, " ");
        normalized = WebUtility.HtmlDecode(normalized);

        var lines = normalized
            .Replace("\r", string.Empty, StringComparison.Ordinal)
            .Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(static line => MultiWhitespaceRegex().Replace(line, " ").Trim())
            .Where(static line => !string.IsNullOrWhiteSpace(line))
            .ToArray();

        if (lines.Length == 0)
        {
            return fallback;
        }

        return string.Join(Environment.NewLine + Environment.NewLine, lines);
    }

    public static string BuildSummary(string description, int maxLength = 100)
    {
        var normalized = NormalizeText(description, "Sin descripcion disponible.");
        if (normalized.Length <= maxLength)
        {
            return normalized;
        }

        var candidate = normalized[..maxLength];
        var lastSpace = candidate.LastIndexOf(' ');
        if (lastSpace >= maxLength / 2)
        {
            candidate = candidate[..lastSpace];
        }

        return $"{candidate.TrimEnd('.', ',', ';', ':', ' ')}...";
    }

    [GeneratedRegex(@"(?i)<br\s*/?>")]
    private static partial Regex LineBreakTagRegex();

    [GeneratedRegex(@"(?i)</(p|div|li|tr|table|ul|ol|h[1-6])>")]
    private static partial Regex BlockClosingTagRegex();

    [GeneratedRegex(@"(?is)<[^>]+>")]
    private static partial Regex RemainingTagsRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex MultiWhitespaceRegex();
}
