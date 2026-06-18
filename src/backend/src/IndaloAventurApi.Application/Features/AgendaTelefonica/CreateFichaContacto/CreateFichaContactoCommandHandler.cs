using IndaloAventurApi.Application.Abstractions.Phonebook;
using IndaloAventurApi.Domain.FichasContacto;
using MediatR;

namespace IndaloAventurApi.Application.Features.AgendaTelefonica.CreateFichaContacto;

public sealed class CreateFichaContactoCommandHandler(IFichaContactoRepository repository) : IRequestHandler<CreateFichaContactoCommand, Guid>
{
    public async Task<Guid> Handle(CreateFichaContactoCommand request, CancellationToken cancellationToken)
    {
        var ficha = FichaContacto.Crear(request.Nombre, request.Telefono1, request.Telefono2, request.Email, request.Direccion, request.Observaciones);

        await repository.AddAsync(ficha, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return ficha.Id;
    }
}
