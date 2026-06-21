namespace IndaloaventurApp.SharedUI.Models.Signals;

public static class SignalImageCodec
{
    public static SignalPhotoDraft CreatePhotoDraft(byte[] content, string? contentType, string fileName)
    {
        ArgumentNullException.ThrowIfNull(content);

        var resolvedContentType = ResolveContentType(content, contentType);
        return new SignalPhotoDraft(
            content,
            resolvedContentType,
            fileName,
            BuildPreviewUrl(content, resolvedContentType));
    }

    public static byte[] NormalizeOptionalPhotoContent(SignalPhotoDraft? photo)
        => NormalizeOptionalPhotoContent(photo?.Content);

    public static byte[] NormalizeOptionalPhotoContent(byte[]? content)
        => content is { Length: > 0 } ? content : Array.Empty<byte>();

    public static string BuildPreviewUrl(byte[] content, string? contentType = null)
    {
        ArgumentNullException.ThrowIfNull(content);

        return $"data:{ResolveContentType(content, contentType)};base64,{Convert.ToBase64String(content)}";
    }

    public static string ResolveContentType(byte[] content, string? declaredContentType = null)
    {
        ArgumentNullException.ThrowIfNull(content);

        return string.IsNullOrWhiteSpace(declaredContentType)
            ? DetectContentType(content)
            : declaredContentType;
    }

    private static string DetectContentType(byte[] content)
    {
        if (content.Length >= 8 &&
            content[0] == 0x89 &&
            content[1] == 0x50 &&
            content[2] == 0x4E &&
            content[3] == 0x47)
        {
            return "image/png";
        }

        if (content.Length >= 3 &&
            content[0] == 0xFF &&
            content[1] == 0xD8 &&
            content[2] == 0xFF)
        {
            return "image/jpeg";
        }

        if (content.Length >= 6 &&
            content[0] == 0x47 &&
            content[1] == 0x49 &&
            content[2] == 0x46)
        {
            return "image/gif";
        }

        if (content.Length >= 12 &&
            content[0] == 0x52 &&
            content[1] == 0x49 &&
            content[2] == 0x46 &&
            content[3] == 0x46 &&
            content[8] == 0x57 &&
            content[9] == 0x45 &&
            content[10] == 0x42 &&
            content[11] == 0x50)
        {
            return "image/webp";
        }

        return "image/jpeg";
    }
}
