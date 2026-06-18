using IndaloAventurApi.Domain.FichasContacto;
using IndaloAventurApi.Domain.FichasContacto.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndaloAventurApi.Infrastructure.Persistence.Configurations;

public sealed class FichaContactoConfiguration : IEntityTypeConfiguration<FichaContacto>
{
    public void Configure(EntityTypeBuilder<FichaContacto> builder)
    {
        builder.ToTable("FichasContacto");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.FechaAlta).IsRequired();

        builder.OwnsOne(x => x.Nombre, nombreBuilder =>
        {
            nombreBuilder.Property(x => x.Value)
                .HasColumnName("Nombre")
                .HasMaxLength(NombreContacto.MaxLength)
                .IsRequired();
        });

        builder.OwnsOne(x => x.Telefono1, telefonoBuilder =>
        {
            telefonoBuilder.Property(x => x.Value)
                .HasColumnName("Telefono1")
                .HasMaxLength(TelefonoContacto.MaxLength)
                .IsRequired();
        });

        builder.OwnsOne(x => x.Telefono2, telefonoBuilder =>
        {
            telefonoBuilder.Property(x => x.Value)
                .HasColumnName("Telefono2")
                .HasMaxLength(TelefonoContacto.MaxLength)
                .IsRequired(false);
        });
        builder.Navigation(x => x.Telefono2).IsRequired(false);

        builder.OwnsOne(x => x.Email, emailBuilder =>
        {
            emailBuilder.Property(x => x.Value)
                .HasColumnName("Email")
                .HasMaxLength(EmailContacto.MaxLength)
                .IsRequired(false);
        });
        builder.Navigation(x => x.Email).IsRequired(false);

        builder.OwnsOne(x => x.Direccion, direccionBuilder =>
        {
            direccionBuilder.Property(x => x.Value)
                .HasColumnName("Direccion")
                .HasMaxLength(DireccionContacto.MaxLength)
                .IsRequired(false);
        });
        builder.Navigation(x => x.Direccion).IsRequired(false);

        builder.OwnsOne(x => x.Observaciones, observacionesBuilder =>
        {
            observacionesBuilder.Property(x => x.Value)
                .HasColumnName("Observaciones")
                .HasMaxLength(ObservacionesContacto.MaxLength)
                .IsRequired(false);
        });
        builder.Navigation(x => x.Observaciones).IsRequired(false);
    }
}
