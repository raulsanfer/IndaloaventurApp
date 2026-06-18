using IndaloAventurApi.Domain.TrailSignals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndaloAventurApi.Infrastructure.Persistence.Configurations;

public sealed class SignalTypeConfiguration : IEntityTypeConfiguration<SignalType>
{
    public void Configure(EntityTypeBuilder<SignalType> builder)
    {
        builder.ToTable("SignalTypes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Nombre).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Icono).HasMaxLength(255).IsRequired();
    }
}
