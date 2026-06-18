using IndaloAventurApi.Application.Abstractions.Phonebook;
using MediatR;

namespace IndaloAventurApi.Application.Features.AgendaTelefonica.UpdateFichaContacto;

public sealed class UpdateFichaContactoCommandHandler(IFichaContactoRepository repository) : IRequestHandler<UpdateFichaContactoCommand, bool>
{
    public async Task<bool> Handle(UpdateFichaContactoCommand request, CancellationToken cancellationToken)
    {
        var ficha = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (ficha is null)
        {
            throw new KeyNotFoundException("La ficha de contacto no existe.");
        }

        ficha.Actualizar(request.Nombre, request.Telefono1, request.Telefono2, request.Email, request.Direccion, request.Observaciones);
        await repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
