namespace IndaloAventurApi.Application.Features.Auth.Login;

public sealed record LoginResponse(string AccessToken, string TokenType, int ExpiresInSeconds, bool IsMember);
