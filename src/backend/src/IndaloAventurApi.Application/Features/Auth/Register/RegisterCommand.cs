using IndaloAventurApi.Application.Abstractions.Cqrs;

namespace IndaloAventurApi.Application.Features.Auth.Register;

public sealed record RegisterCommand(string Email, string Password) : ICommand<bool>;
