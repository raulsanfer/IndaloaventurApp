using IndaloAventurApi.Application.Abstractions.Phonebook;
using MediatR;

namespace IndaloAventurApi.Application.Features.AgendaTelefonica.GetFichaContactoById;

public sealed class GetFichaContactoByIdQueryHandler(IFichaContactoRepository repository) : IRequestHandler<GetFichaContactoByIdQuery, FichaContactoDto>
{
    public async Task<FichaContactoDto> Handle(GetFichaContactoByIdQuery request, CancellationToken cancellationToken)
    {
        var ficha = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (ficha is null)
        {
            throw new KeyNotFoundException("La ficha de contacto no existe.");
        }

        return new FichaContactoDto(
            ficha.Id,
            ficha.FechaAlta,
            ficha.Nombre.Value,
            ficha.Telefono1.Value,
            ficha.Telefono2?.Value,
            ficha.Email?.Value,
            ficha.Direccion?.Value,
            ficha.Observaciones?.Value);
    }
}
