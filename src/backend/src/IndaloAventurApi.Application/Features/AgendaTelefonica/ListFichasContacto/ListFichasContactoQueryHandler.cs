using IndaloAventurApi.Application.Abstractions.Phonebook;
using MediatR;

namespace IndaloAventurApi.Application.Features.AgendaTelefonica.ListFichasContacto;

public sealed class ListFichasContactoQueryHandler(IFichaContactoRepository repository) : IRequestHandler<ListFichasContactoQuery, IReadOnlyCollection<FichaContactoDto>>
{
    public async Task<IReadOnlyCollection<FichaContactoDto>> Handle(ListFichasContactoQuery request, CancellationToken cancellationToken)
    {
        var fichas = await repository.ListAsync(cancellationToken);
        return fichas
            .Select(ficha => new FichaContactoDto(
                ficha.Id,
                ficha.FechaAlta,
                ficha.Nombre.Value,
                ficha.Telefono1.Value,
                ficha.Telefono2?.Value,
                ficha.Email?.Value,
                ficha.Direccion?.Value,
                ficha.Observaciones?.Value))
            .ToArray();
    }
}
