namespace IndaloaventurApp.Web.Client.Infrastructure.Signals;

using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Abstractions.Signals;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Signals;

public sealed class SignalApiClient(HttpClient httpClient, ISessionService sessionService) : ISignalService
{
    private const string SignalsEndpoint = "/api/signals";
    private const string SignalTypesEndpoint = "/api/signal-types";

    public async Task<ServiceResult<SignalHomeData>> GetSignalHomeDataAsync(SignalListQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var categories = await GetSignalCategoriesAsync(cancellationToken);
            if (!categories.IsSuccess)
            {
                return ServiceResult<SignalHomeData>.Failure(categories.Error!);
            }

            var signals = await GetSignalsAsync(query, cancellationToken);
            if (!signals.IsSuccess)
            {
                return ServiceResult<SignalHomeData>.Failure(signals.Error!);
            }

            var categoryMap = (categories.Value ?? Array.Empty<SignalCategoryItem>())
                .ToDictionary(item => item.Id);

            var cards = await Task.WhenAll(
                (signals.Value ?? Array.Empty<SignalDto>())
                    .Select(signal => MapSignalAsync(signal, categoryMap, cancellationToken)));

            return ServiceResult<SignalHomeData>.Success(
                new SignalHomeData(categories.Value ?? Array.Empty<SignalCategoryItem>(), cards));
        }
        catch (HttpRequestException)
        {
            return ServiceResult<SignalHomeData>.Failure(new ServiceError("signals.unavailable", "No se pudo conectar con las senales."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<SignalHomeData>.Failure(new ServiceError("signals.timeout", "Tiempo de espera agotado."));
        }
        catch (JsonException)
        {
            return ServiceResult<SignalHomeData>.Failure(new ServiceError("signals.invalid_payload", "La respuesta de senales no tiene un formato valido."));
        }
        catch (NotSupportedException)
        {
            return ServiceResult<SignalHomeData>.Failure(new ServiceError("signals.invalid_payload", "La respuesta de senales no tiene un formato valido."));
        }
    }

    public async Task<ServiceResult<SignalDetailItem>> GetSignalAsync(Guid signalId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var request = CreateAuthorizedRequest(HttpMethod.Get, BuildSignalDetailEndpoint(signalId));
            using var response = await httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ServiceResult<SignalDetailItem>.Failure(new ServiceError("auth.session_invalid", "Sesion invalida."));
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return ServiceResult<SignalDetailItem>.Failure(new ServiceError("signals.not_found", "La senal no existe."));
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<SignalDetailItem>.Failure(new ServiceError("signals.unavailable", $"Error HTTP {(int)response.StatusCode}."));
            }

            var payload = await response.Content.ReadFromJsonAsync<SignalDto>(cancellationToken: cancellationToken);
            if (payload is null)
            {
                return ServiceResult<SignalDetailItem>.Failure(new ServiceError("signals.invalid_payload", "La respuesta de senales no tiene un formato valido."));
            }

            var categoriesResult = await GetSignalCategoriesAsync(cancellationToken);
            var categoryMap = categoriesResult.IsSuccess
                ? (categoriesResult.Value ?? Array.Empty<SignalCategoryItem>()).ToDictionary(item => item.Id)
                : new Dictionary<int, SignalCategoryItem>();

            return ServiceResult<SignalDetailItem>.Success(MapSignalDetail(payload, categoryMap));
        }
        catch (HttpRequestException)
        {
            return ServiceResult<SignalDetailItem>.Failure(new ServiceError("signals.unavailable", "No se pudo conectar con las senales."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<SignalDetailItem>.Failure(new ServiceError("signals.timeout", "Tiempo de espera agotado."));
        }
        catch (JsonException)
        {
            return ServiceResult<SignalDetailItem>.Failure(new ServiceError("signals.invalid_payload", "La respuesta de senales no tiene un formato valido."));
        }
        catch (NotSupportedException)
        {
            return ServiceResult<SignalDetailItem>.Failure(new ServiceError("signals.invalid_payload", "La respuesta de senales no tiene un formato valido."));
        }
    }

    public async Task<ServiceResult<IReadOnlyList<SignalCommentItem>>> GetSignalCommentsAsync(Guid signalId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var request = CreateAuthorizedRequest(HttpMethod.Get, BuildSignalCommentsEndpoint(signalId));
            using var response = await httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ServiceResult<IReadOnlyList<SignalCommentItem>>.Failure(new ServiceError("auth.session_invalid", "Sesion invalida."));
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return ServiceResult<IReadOnlyList<SignalCommentItem>>.Failure(new ServiceError("signals.not_found", "La senal no existe."));
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<IReadOnlyList<SignalCommentItem>>.Failure(new ServiceError("signals.comments_unavailable", $"Error HTTP {(int)response.StatusCode}."));
            }

            var payload = await response.Content.ReadFromJsonAsync<SignalCommentDto[]>(cancellationToken: cancellationToken);
            if (payload is null)
            {
                return ServiceResult<IReadOnlyList<SignalCommentItem>>.Failure(new ServiceError("signals.invalid_payload", "La respuesta de senales no tiene un formato valido."));
            }

            return ServiceResult<IReadOnlyList<SignalCommentItem>>.Success(
                payload.Select(static dto => new SignalCommentItem(
                        dto.Id,
                        dto.FechaComentario,
                        string.IsNullOrWhiteSpace(dto.Texto) ? "Sin comentario." : dto.Texto.Trim()))
                    .OrderByDescending(static item => item.CommentedAt)
                    .ToArray());
        }
        catch (HttpRequestException)
        {
            return ServiceResult<IReadOnlyList<SignalCommentItem>>.Failure(new ServiceError("signals.unavailable", "No se pudo conectar con las senales."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<IReadOnlyList<SignalCommentItem>>.Failure(new ServiceError("signals.timeout", "Tiempo de espera agotado."));
        }
        catch (JsonException)
        {
            return ServiceResult<IReadOnlyList<SignalCommentItem>>.Failure(new ServiceError("signals.invalid_payload", "La respuesta de senales no tiene un formato valido."));
        }
        catch (NotSupportedException)
        {
            return ServiceResult<IReadOnlyList<SignalCommentItem>>.Failure(new ServiceError("signals.invalid_payload", "La respuesta de senales no tiene un formato valido."));
        }
    }

    public async Task<ServiceResult<IReadOnlyList<SignalCategoryItem>>> GetSignalCategoriesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var request = CreateAuthorizedRequest(HttpMethod.Get, SignalTypesEndpoint);
            using var response = await httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ServiceResult<IReadOnlyList<SignalCategoryItem>>.Failure(new ServiceError("auth.session_invalid", "Sesion invalida."));
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<IReadOnlyList<SignalCategoryItem>>.Failure(new ServiceError("signals.categories_unavailable", $"Error HTTP {(int)response.StatusCode}."));
            }

            var payload = await response.Content.ReadFromJsonAsync<SignalTypeDto[]>(cancellationToken: cancellationToken);
            if (payload is null)
            {
                return ServiceResult<IReadOnlyList<SignalCategoryItem>>.Failure(new ServiceError("signals.categories_empty", "Respuesta vacia."));
            }

            return ServiceResult<IReadOnlyList<SignalCategoryItem>>.Success(
                payload.Select(dto => new SignalCategoryItem(
                        dto.Id,
                        string.IsNullOrWhiteSpace(dto.Nombre) ? $"Tipo {dto.Id}" : dto.Nombre.Trim(),
                        NormalizeIconName(dto.Icono)))
                    .OrderBy(item => item.Name, StringComparer.CurrentCultureIgnoreCase)
                    .ToArray());
        }
        catch (HttpRequestException)
        {
            return ServiceResult<IReadOnlyList<SignalCategoryItem>>.Failure(new ServiceError("signals.unavailable", "No se pudo conectar con las senales."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<IReadOnlyList<SignalCategoryItem>>.Failure(new ServiceError("signals.timeout", "Tiempo de espera agotado."));
        }
        catch (JsonException)
        {
            return ServiceResult<IReadOnlyList<SignalCategoryItem>>.Failure(new ServiceError("signals.invalid_payload", "La respuesta de senales no tiene un formato valido."));
        }
        catch (NotSupportedException)
        {
            return ServiceResult<IReadOnlyList<SignalCategoryItem>>.Failure(new ServiceError("signals.invalid_payload", "La respuesta de senales no tiene un formato valido."));
        }
    }

    public async Task<ServiceResult<Guid>> CreateSignalAsync(SignalCreateRequest requestModel, CancellationToken cancellationToken = default)
    {
        try
        {
            using var request = CreateAuthorizedRequest(HttpMethod.Post, SignalsEndpoint);
            request.Content = JsonContent.Create(
                new CreateSignalDto(
                    requestModel.Latitude,
                    requestModel.Longitude,
                    requestModel.Title,
                    requestModel.Description,
                    requestModel.Photo1,
                    requestModel.Photo2,
                    requestModel.IsActive,
                    requestModel.TypeId,
                    requestModel.Tags));

            using var response = await httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ServiceResult<Guid>.Failure(new ServiceError("auth.session_invalid", "Sesion invalida."));
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return ServiceResult<Guid>.Failure(new ServiceError("signals.create_validation", "La senal no supera la validacion."));
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<Guid>.Failure(new ServiceError("signals.create_failed", $"Error HTTP {(int)response.StatusCode}."));
            }

            var createdId = await response.Content.ReadFromJsonAsync<Guid>(cancellationToken: cancellationToken);
            return ServiceResult<Guid>.Success(createdId);
        }
        catch (HttpRequestException)
        {
            return ServiceResult<Guid>.Failure(new ServiceError("signals.unavailable", "No se pudo conectar con las senales."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<Guid>.Failure(new ServiceError("signals.timeout", "Tiempo de espera agotado."));
        }
        catch (JsonException)
        {
            return ServiceResult<Guid>.Failure(new ServiceError("signals.invalid_payload", "La respuesta de senales no tiene un formato valido."));
        }
        catch (NotSupportedException)
        {
            return ServiceResult<Guid>.Failure(new ServiceError("signals.invalid_payload", "La respuesta de senales no tiene un formato valido."));
        }
    }

    public async Task<ServiceResult<bool>> UpdateSignalAsync(UpdateSignalRequest requestModel, CancellationToken cancellationToken = default)
    {
        try
        {
            using var request = CreateAuthorizedRequest(HttpMethod.Put, BuildSignalDetailEndpoint(requestModel.SignalId));
            request.Content = JsonContent.Create(
                new UpdateSignalDto(
                    requestModel.Title,
                    requestModel.Description,
                    requestModel.IsActive));

            using var response = await httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ServiceResult<bool>.Failure(new ServiceError("auth.session_invalid", "Sesion invalida."));
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return ServiceResult<bool>.Failure(new ServiceError("signals.update_forbidden", "No tienes permisos para editar esta senal."));
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return ServiceResult<bool>.Failure(new ServiceError("signals.not_found", "La senal no existe."));
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return ServiceResult<bool>.Failure(new ServiceError("signals.update_validation", "La senal no supera la validacion."));
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<bool>.Failure(new ServiceError("signals.update_failed", $"Error HTTP {(int)response.StatusCode}."));
            }

            return ServiceResult<bool>.Success(true);
        }
        catch (HttpRequestException)
        {
            return ServiceResult<bool>.Failure(new ServiceError("signals.unavailable", "No se pudo conectar con las senales."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<bool>.Failure(new ServiceError("signals.timeout", "Tiempo de espera agotado."));
        }
    }

    public async Task<ServiceResult<int>> CreateSignalCategoryAsync(CreateSignalCategoryRequest requestModel, CancellationToken cancellationToken = default)
    {
        try
        {
            using var request = CreateAuthorizedRequest(HttpMethod.Post, SignalTypesEndpoint);
            request.Content = JsonContent.Create(new CreateSignalTypeDto(requestModel.Name, requestModel.IconName));

            using var response = await httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ServiceResult<int>.Failure(new ServiceError("auth.session_invalid", "Sesion invalida."));
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return ServiceResult<int>.Failure(new ServiceError("signals.categories.validation", "La categoria no supera la validacion."));
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<int>.Failure(new ServiceError("signals.categories.create_failed", $"Error HTTP {(int)response.StatusCode}."));
            }

            var createdId = await response.Content.ReadFromJsonAsync<int>(cancellationToken: cancellationToken);
            return ServiceResult<int>.Success(createdId);
        }
        catch (HttpRequestException)
        {
            return ServiceResult<int>.Failure(new ServiceError("signals.unavailable", "No se pudo conectar con las senales."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<int>.Failure(new ServiceError("signals.timeout", "Tiempo de espera agotado."));
        }
        catch (JsonException)
        {
            return ServiceResult<int>.Failure(new ServiceError("signals.invalid_payload", "La respuesta de senales no tiene un formato valido."));
        }
        catch (NotSupportedException)
        {
            return ServiceResult<int>.Failure(new ServiceError("signals.invalid_payload", "La respuesta de senales no tiene un formato valido."));
        }
    }

    public async Task<ServiceResult<bool>> UpdateSignalCategoryAsync(UpdateSignalCategoryRequest requestModel, CancellationToken cancellationToken = default)
    {
        try
        {
            using var request = CreateAuthorizedRequest(HttpMethod.Put, $"{SignalTypesEndpoint}/{requestModel.Id}");
            request.Content = JsonContent.Create(new UpdateSignalTypeDto(requestModel.Name, requestModel.IconName));

            using var response = await httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ServiceResult<bool>.Failure(new ServiceError("auth.session_invalid", "Sesion invalida."));
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return ServiceResult<bool>.Failure(new ServiceError("signals.categories.validation", "La categoria no supera la validacion."));
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return ServiceResult<bool>.Failure(new ServiceError("signals.categories.not_found", "La categoria no existe."));
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<bool>.Failure(new ServiceError("signals.categories.update_failed", $"Error HTTP {(int)response.StatusCode}."));
            }

            return ServiceResult<bool>.Success(true);
        }
        catch (HttpRequestException)
        {
            return ServiceResult<bool>.Failure(new ServiceError("signals.unavailable", "No se pudo conectar con las senales."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<bool>.Failure(new ServiceError("signals.timeout", "Tiempo de espera agotado."));
        }
    }

    public async Task<ServiceResult<bool>> DeleteSignalCategoryAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            using var request = CreateAuthorizedRequest(HttpMethod.Delete, $"{SignalTypesEndpoint}/{id}");

            using var response = await httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ServiceResult<bool>.Failure(new ServiceError("auth.session_invalid", "Sesion invalida."));
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return ServiceResult<bool>.Failure(new ServiceError("signals.categories.not_found", "La categoria no existe."));
            }

            if (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.Conflict)
            {
                return ServiceResult<bool>.Failure(new ServiceError("signals.categories.delete_blocked", "La categoria no se puede eliminar."));
            }

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult<bool>.Failure(new ServiceError("signals.categories.delete_failed", $"Error HTTP {(int)response.StatusCode}."));
            }

            return ServiceResult<bool>.Success(true);
        }
        catch (HttpRequestException)
        {
            return ServiceResult<bool>.Failure(new ServiceError("signals.unavailable", "No se pudo conectar con las senales."));
        }
        catch (TaskCanceledException)
        {
            return ServiceResult<bool>.Failure(new ServiceError("signals.timeout", "Tiempo de espera agotado."));
        }
    }

    private async Task<ServiceResult<IReadOnlyList<SignalDto>>> GetSignalsAsync(SignalListQuery query, CancellationToken cancellationToken)
    {
        using var request = CreateAuthorizedRequest(HttpMethod.Get, BuildSignalsEndpoint(query));
        using var response = await httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return ServiceResult<IReadOnlyList<SignalDto>>.Failure(new ServiceError("auth.session_invalid", "Sesion invalida."));
        }

        if (!response.IsSuccessStatusCode)
        {
            return ServiceResult<IReadOnlyList<SignalDto>>.Failure(new ServiceError("signals.unavailable", $"Error HTTP {(int)response.StatusCode}."));
        }

        var payload = await response.Content.ReadFromJsonAsync<SignalDto[]>(cancellationToken: cancellationToken);
        if (payload is null)
        {
            return ServiceResult<IReadOnlyList<SignalDto>>.Failure(new ServiceError("signals.empty", "Respuesta vacia."));
        }

        return ServiceResult<IReadOnlyList<SignalDto>>.Success(payload);
    }

    private HttpRequestMessage CreateAuthorizedRequest(HttpMethod method, string endpoint)
    {
        var request = new HttpRequestMessage(method, endpoint);

        if (sessionService.IsAuthenticated && !string.IsNullOrWhiteSpace(sessionService.CurrentSession?.AccessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(
                sessionService.CurrentSession!.TokenType,
                sessionService.CurrentSession.AccessToken);
        }

        return request;
    }

    private static string BuildSignalsEndpoint(SignalListQuery query)
    {
        var parameters = new List<string>();

        if (query.OnlyActive)
        {
            parameters.Add("activo=true");
        }

        if (query.CategoryId is not null)
        {
            parameters.Add($"tipo={query.CategoryId.Value.ToString(CultureInfo.InvariantCulture)}");
        }

        if (!string.IsNullOrWhiteSpace(query.SearchText))
        {
            parameters.Add($"descripcion={Uri.EscapeDataString(query.SearchText.Trim())}");
        }

        return parameters.Count == 0
            ? SignalsEndpoint
            : $"{SignalsEndpoint}?{string.Join("&", parameters)}";
    }

    private static string BuildSignalDetailEndpoint(Guid signalId)
        => $"{SignalsEndpoint}/{signalId:D}";

    private static string BuildSignalCommentsEndpoint(Guid signalId)
        => $"{SignalsEndpoint}/{signalId:D}/comments";

    private static string BuildSignalImagesEndpoint(Guid signalId)
        => $"{SignalsEndpoint}/{signalId:D}/images";

    private async Task<SignalCardItem> MapSignalAsync(
        SignalDto dto,
        IReadOnlyDictionary<int, SignalCategoryItem> categoryMap,
        CancellationToken cancellationToken)
    {
        var imageUrl = await GetPrimaryImageUrlAsync(dto.Id, cancellationToken);
        return MapSignal(dto, categoryMap, imageUrl);
    }

    private async Task<string?> GetPrimaryImageUrlAsync(Guid signalId, CancellationToken cancellationToken)
    {
        try
        {
            using var request = CreateAuthorizedRequest(HttpMethod.Get, BuildSignalImagesEndpoint(signalId));
            using var response = await httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var payload = await response.Content.ReadFromJsonAsync<SignalImagesDto>(cancellationToken: cancellationToken);
            var imageBytes = payload?.Foto1?.Length > 0
                ? payload.Foto1
                : payload?.Foto2?.Length > 0
                    ? payload.Foto2
                    : null;

            if (imageBytes is null || imageBytes.Length == 0)
            {
                return null;
            }

            return BuildImageDataUrl(imageBytes);
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (JsonException)
        {
            return null;
        }
        catch (NotSupportedException)
        {
            return null;
        }
    }

    private static SignalCardItem MapSignal(SignalDto dto, IReadOnlyDictionary<int, SignalCategoryItem> categoryMap, string? imageUrl)
    {
        var category = categoryMap.TryGetValue(dto.Tipo, out var resolvedCategory)
            ? resolvedCategory
            : new SignalCategoryItem(dto.Tipo, "Senal", "exclamation-triangle-fill");

        var trimmedDescription = string.IsNullOrWhiteSpace(dto.Descripcion)
            ? "Senal sin descripcion."
            : dto.Descripcion.Trim();

        var title = string.IsNullOrWhiteSpace(dto.Titulo)
            ? BuildTitle(trimmedDescription)
            : dto.Titulo.Trim();
        var summary = BuildSummary(trimmedDescription, title);
        var timestamp = dto.FechaModificacion == default ? dto.FechaAlta : dto.FechaModificacion;

        return new SignalCardItem(
            dto.Id,
            title,
            summary,
            dto.Tipo,
            category.Name,
            category.IconName,
            timestamp,
            BuildMetaLabel(dto.Tags, dto.Latitud, dto.Longitud, dto.Activo),
            dto.Activo,
            imageUrl,
            dto.Tags,
            dto.Latitud,
            dto.Longitud);
    }

    private static SignalDetailItem MapSignalDetail(SignalDto dto, IReadOnlyDictionary<int, SignalCategoryItem> categoryMap)
    {
        var category = categoryMap.TryGetValue(dto.Tipo, out var resolvedCategory)
            ? resolvedCategory
            : new SignalCategoryItem(dto.Tipo, "Senal", "exclamation-triangle-fill");

        var description = string.IsNullOrWhiteSpace(dto.Descripcion)
            ? "Senal sin descripcion."
            : dto.Descripcion.Trim();

        return new SignalDetailItem(
            dto.Id,
            string.IsNullOrWhiteSpace(dto.Titulo) ? BuildTitle(description) : dto.Titulo.Trim(),
            description,
            dto.Tipo,
            category.Name,
            category.IconName,
            dto.Activo,
            dto.FechaAlta,
            dto.FechaModificacion == default ? dto.FechaAlta : dto.FechaModificacion,
            string.IsNullOrWhiteSpace(dto.Tags) ? null : dto.Tags.Trim(),
            dto.Latitud,
            dto.Longitud,
            dto.UserIdAlta);
    }

    private static string BuildTitle(string description)
    {
        var endOfSentence = description.IndexOfAny(['.', '!', '?', '\n', '\r']);
        var candidate = endOfSentence > 0
            ? description[..endOfSentence]
            : description;

        candidate = candidate.Trim();

        if (candidate.Length > 72)
        {
            candidate = $"{candidate[..69].TrimEnd()}...";
        }

        return string.IsNullOrWhiteSpace(candidate)
            ? "Senal del club"
            : candidate;
    }

    private static string BuildSummary(string description, string title)
    {
        if (string.Equals(description, title, StringComparison.Ordinal))
        {
            return description.Length > 140
                ? $"{description[..137].TrimEnd()}..."
                : description;
        }

        if (description.StartsWith(title, StringComparison.Ordinal))
        {
            description = description[title.Length..].TrimStart('.', ' ', ':');
        }

        if (description.Length > 160)
        {
            description = $"{description[..157].TrimEnd()}...";
        }

        return string.IsNullOrWhiteSpace(description)
            ? title
            : description;
    }

    private static string BuildImageDataUrl(byte[] bytes)
        => $"data:{GetImageMimeType(bytes)};base64,{Convert.ToBase64String(bytes)}";

    private static string GetImageMimeType(byte[] bytes)
    {
        if (bytes.Length >= 8 &&
            bytes[0] == 0x89 &&
            bytes[1] == 0x50 &&
            bytes[2] == 0x4E &&
            bytes[3] == 0x47)
        {
            return "image/png";
        }

        if (bytes.Length >= 3 &&
            bytes[0] == 0xFF &&
            bytes[1] == 0xD8 &&
            bytes[2] == 0xFF)
        {
            return "image/jpeg";
        }

        if (bytes.Length >= 6 &&
            bytes[0] == 0x47 &&
            bytes[1] == 0x49 &&
            bytes[2] == 0x46)
        {
            return "image/gif";
        }

        if (bytes.Length >= 12 &&
            bytes[0] == 0x52 &&
            bytes[1] == 0x49 &&
            bytes[2] == 0x46 &&
            bytes[3] == 0x46 &&
            bytes[8] == 0x57 &&
            bytes[9] == 0x45 &&
            bytes[10] == 0x42 &&
            bytes[11] == 0x50)
        {
            return "image/webp";
        }

        return "image/jpeg";
    }

    private static string BuildMetaLabel(string? tags, float latitude, float longitude, bool isActive)
    {
        if (!string.IsNullOrWhiteSpace(tags))
        {
            var firstTag = tags.Split(
                    [',', ';', '|'],
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(firstTag))
            {
                return firstTag;
            }
        }

        if (latitude != 0 || longitude != 0)
        {
            return $"{latitude.ToString("0.000", CultureInfo.InvariantCulture)}, {longitude.ToString("0.000", CultureInfo.InvariantCulture)}";
        }

        return isActive ? "Activa" : "Archivada";
    }

    private static string? NormalizeIconName(string? iconName)
    {
        if (string.IsNullOrWhiteSpace(iconName))
        {
            return null;
        }

        var normalized = iconName.Trim();
        if (normalized.StartsWith("bi ", StringComparison.OrdinalIgnoreCase))
        {
            return normalized["bi ".Length..].Trim();
        }

        return normalized.StartsWith("bi-", StringComparison.OrdinalIgnoreCase)
            ? normalized
            : $"bi-{normalized}";
    }

    private sealed record SignalDto(
        Guid Id,
        float Latitud,
        float Longitud,
        string? Titulo,
        string? Descripcion,
        bool Activo,
        Guid UserIdAlta,
        DateTime FechaAlta,
        DateTime FechaModificacion,
        Guid UserIdModificacion,
        int Tipo,
        string? Tags);

    private sealed record SignalImagesDto(
        Guid SignalId,
        byte[]? Foto1,
        byte[]? Foto2);

    private sealed record SignalTypeDto(
        int Id,
        string? Nombre,
        string? Icono);

    private sealed record SignalCommentDto(
        Guid Id,
        Guid SignalId,
        Guid UserId,
        DateTime FechaComentario,
        string? Texto);

    private sealed record CreateSignalTypeDto(
        string Nombre,
        string? Icono);

    private sealed record UpdateSignalTypeDto(
        string Nombre,
        string? Icono);

    private sealed record CreateSignalDto(
        float Latitud,
        float Longitud,
        string Titulo,
        string Descripcion,
        byte[] Foto1,
        byte[] Foto2,
        bool Activo,
        int Tipo,
        string Tags);

    private sealed record UpdateSignalDto(
        string Titulo,
        string Descripcion,
        bool Activo);
}
