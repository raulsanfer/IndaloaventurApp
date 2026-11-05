using IndaloAventurApi.Domain.Inventario;
using IndaloAventurApi.Domain.Inventario.ReservasMaterial;
using IndaloAventurApi.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndaloAventurApi.Infrastructure.Persistence.Inventario;

public sealed class ReservaMaterialConfiguration : IEntityTypeConfiguration<ReservaMaterial>
{
    public void Configure(EntityTypeBuilder<ReservaMaterial> builder)
    {
        builder.ToTable("ReservasMaterial");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.InventarioMaterialId).IsRequired();
        builder.Property(x => x.CantidadSolicitada).IsRequired();
        builder.Property(x => x.FechaSolicitudUtc).IsRequired();
        builder.Property(x => x.FechaRetiradaPrevista).HasColumnType("date").IsRequired();
        builder.Property(x => x.FechaRetirada).HasColumnType("date").IsRequired(false);
        builder.Property(x => x.FechaDevolucionEstimada).HasColumnType("date").IsRequired();
        builder.Property(x => x.FechaDevolucionEfectiva).HasColumnType("date").IsRequired(false);
        builder.Property(x => x.Observaciones).HasMaxLength(InventarioMaterialTexto.ObservacionesMaxLength).IsRequired(false);
        builder.Property(x => x.ObservacionAdministracion).HasMaxLength(InventarioMaterialTexto.ObservacionesMaxLength).IsRequired(false);
        builder.Property(x => x.Estado).HasConversion<int>().IsRequired();
        builder.Property(x => x.ConfirmacionDevolucionSocio).IsRequired();
        builder.Property(x => x.ConfirmacionDevolucionAdmin).IsRequired();
        builder.Property(x => x.ConcurrencyToken).IsConcurrencyToken().IsRequired();

        builder.HasIndex(x => new { x.InventarioMaterialId, x.Estado });
        builder.HasIndex(x => new { x.UserId, x.FechaSolicitudUtc });

        builder.HasOne<InventarioMaterial>()
            .WithMany()
            .HasForeignKey(x => x.InventarioMaterialId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
