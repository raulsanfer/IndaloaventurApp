using IndaloAventurApi.Application.Abstractions.Inventario;
using IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;
using MediatR;

namespace IndaloAventurApi.Application.Features.Inventario.InventarioMateriales.UpdateInventarioMaterial;

public sealed class UpdateInventarioMaterialCommandHandler(
    IInventarioMaterialRepository materialRepository,
    IInventarioCategoriaRepository categoriaRepository,
    IInventarioEstadoRepository estadoRepository,
    IReservaMaterialRepository reservaRepository)
    : IRequestHandler<UpdateInventarioMaterialCommand, bool>
{
    public async Task<bool> Handle(UpdateInventarioMaterialCommand request, CancellationToken cancellationToken)
    {
        if (await categoriaRepository.GetByIdAsync(request.CategoriaId, cancellationToken) is null)
        {
            throw new KeyNotFoundException("La categoria de inventario no existe.");
        }

        if (await estadoRepository.GetByIdAsync(request.EstadoId, cancellationToken) is null)
        {
            throw new KeyNotFoundException("El estado de inventario no existe.");
        }

        return await reservaRepository.ExecuteSerializableAsync(async transactionCancellationToken =>
        {
            var material = await materialRepository.GetByIdForUpdateAsync(request.Id, transactionCancellationToken)
                ?? throw new KeyNotFoundException("El material de inventario no existe.");
            var cantidadComprometida = await reservaRepository.GetCantidadComprometidaAsync(material.Id, null, transactionCancellationToken);
            if (request.Cantidad < cantidadComprometida)
            {
                throw new InvalidOperationException("No se puede reducir la cantidad por debajo de las unidades comprometidas en reservas activas.");
            }

            material.Actualizar(
                request.Descripcion,
                request.MarcaModelo,
                request.Cantidad,
                request.CategoriaId,
                request.EstadoId,
                request.FechaAdquisicion,
                request.AdquiridoDonado,
                request.Observaciones,
                request.NumeroSerie,
                request.Activo);
            if (material.NumeroSerieNormalizado is not null
                && await materialRepository.ExistsWithNormalizedSerialAsync(material.NumeroSerieNormalizado, material.Id, transactionCancellationToken))
            {
                throw new InvalidOperationException("Ya existe un material con ese numero de serie.");
            }

            await materialRepository.SaveChangesAsync(transactionCancellationToken);
            return true;
        }, cancellationToken);
    }
}
