using MediatR;

namespace IndaloAventurApi.Application.Abstractions.Cqrs;

public interface ICommand<out TResponse> : IRequest<TResponse>;
