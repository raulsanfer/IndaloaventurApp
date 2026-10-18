using IndaloAventurApi.Domain.Inventario;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndaloAventurApi.Infrastructure.Persistence.Inventario;

public sealed class InventarioCategoriaConfiguration : IEntityTypeConfiguration<InventarioCategoria>
{
    public void Configure(EntityTypeBuilder<InventarioCategoria> builder)
    {
        builder.ToTable("InventarioCategorias");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Nombre).HasMaxLength(InventarioNombre.MaxLength).IsRequired();
        builder.Property(x => x.NombreNormalizado).HasMaxLength(InventarioNombre.MaxLength).IsRequired();
        builder.Property(x => x.CategoriaPadreId).IsRequired(false);

        builder.HasOne<InventarioCategoria>()
            .WithMany()
            .HasForeignKey(x => x.CategoriaPadreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.CategoriaPadreId, x.NombreNormalizado })
            .IsUnique()
            .HasFilter("[CategoriaPadreId] IS NOT NULL");
        builder.HasIndex(x => x.NombreNormalizado)
            .IsUnique()
            .HasFilter("[CategoriaPadreId] IS NULL");
    }
}
