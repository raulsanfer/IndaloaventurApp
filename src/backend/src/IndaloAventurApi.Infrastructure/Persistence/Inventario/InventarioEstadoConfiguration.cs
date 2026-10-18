using IndaloAventurApi.Domain.Inventario;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndaloAventurApi.Infrastructure.Persistence.Inventario;

public sealed class InventarioEstadoConfiguration : IEntityTypeConfiguration<InventarioEstado>
{
    public void Configure(EntityTypeBuilder<InventarioEstado> builder)
    {
        builder.ToTable("InventarioEstados");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Nombre).HasMaxLength(InventarioNombre.MaxLength).IsRequired();
        builder.Property(x => x.NombreNormalizado).HasMaxLength(InventarioNombre.MaxLength).IsRequired();
        builder.HasIndex(x => x.NombreNormalizado).IsUnique();
    }
}
