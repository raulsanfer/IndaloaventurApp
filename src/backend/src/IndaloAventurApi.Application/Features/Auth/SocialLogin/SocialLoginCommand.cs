using IndaloAventurApi.Application.Abstractions.Cqrs;
using IndaloAventurApi.Application.Features.Auth.Login;

namespace IndaloAventurApi.Application.Features.Auth.SocialLogin;

public sealed record SocialLoginCommand(string Provider, string Token) : ICommand<LoginResponse>;
