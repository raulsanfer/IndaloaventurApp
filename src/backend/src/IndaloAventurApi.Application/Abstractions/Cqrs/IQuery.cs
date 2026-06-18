using MediatR;

namespace IndaloAventurApi.Application.Abstractions.Cqrs;

public interface IQuery<out TResponse> : IRequest<TResponse>;
