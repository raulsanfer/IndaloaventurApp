using IndaloAventurApi.Application.Abstractions.Cqrs;
using IndaloAventurApi.Application.Abstractions.Phonebook;

namespace IndaloAventurApi.Application.Features.AgendaTelefonica.ListFichasContacto;

public sealed record ListFichasContactoQuery() : IQuery<IReadOnlyCollection<FichaContactoDto>>;