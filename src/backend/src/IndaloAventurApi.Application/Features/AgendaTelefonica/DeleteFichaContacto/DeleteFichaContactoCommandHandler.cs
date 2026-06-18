using IndaloAventurApi.Application.Abstractions.Phonebook;
using MediatR;

namespace IndaloAventurApi.Application.Features.AgendaTelefonica.DeleteFichaContacto;

public sealed class DeleteFichaContactoCommandHandler(IFichaContactoRepository repository) : IRequestHandler<DeleteFichaContactoCommand, bool>
{
    public async Task<bool> Handle(DeleteFichaContactoCommand request, CancellationToken cancellationToken)
    {
        var ficha = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (ficha is null)
        {
            throw new KeyNotFoundException("La ficha de contacto no existe.");
        }

        repository.Remove(ficha);
        await repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}