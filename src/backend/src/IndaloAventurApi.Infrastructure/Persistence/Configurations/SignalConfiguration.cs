using IndaloAventurApi.Domain.TrailSignals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndaloAventurApi.Infrastructure.Persistence.Configurations;

public sealed class SignalConfiguration : IEntityTypeConfiguration<Signal>
{
    public void Configure(EntityTypeBuilder<Signal> builder)
    {
        builder.ToTable("Signals");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Latitud).IsRequired();
        builder.Property(x => x.Longitud).IsRequired();
        builder.Property(x => x.Titulo).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.Descripcion).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.Foto1Path).HasMaxLength(2048).IsRequired();
        builder.Property(x => x.Foto2Path).HasMaxLength(2048).IsRequired();
        builder.Property(x => x.Activo).IsRequired();
        builder.Property(x => x.UserIdAlta).IsRequired();
        builder.Property(x => x.FechaAlta).IsRequired();
        builder.Property(x => x.FechaModificacion).IsRequired();
        builder.Property(x => x.UserIdModificacion).IsRequired();
        builder.Property(x => x.Tipo).IsRequired();
        builder.Property(x => x.Tags).HasMaxLength(1500).IsRequired();

        builder.HasOne(x => x.SignalType)
            .WithMany()
            .HasForeignKey(x => x.Tipo)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Comentarios)
            .WithOne(x => x.Signal)
            .HasForeignKey(x => x.SignalId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
