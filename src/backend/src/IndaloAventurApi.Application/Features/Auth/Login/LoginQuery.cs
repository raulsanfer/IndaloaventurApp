using IndaloAventurApi.Application.Abstractions.Cqrs;

namespace IndaloAventurApi.Application.Features.Auth.Login;

public sealed record LoginQuery(string Email, string Password) : IQuery<LoginResponse>;
