using IndaloAventurApi.Domain.TrailSignals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndaloAventurApi.Infrastructure.Persistence.Configurations;

public sealed class SignalCommentConfiguration : IEntityTypeConfiguration<SignalComment>
{
    public void Configure(EntityTypeBuilder<SignalComment> builder)
    {
        builder.ToTable("SignalComments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.SignalId).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.FechaComentario).IsRequired();
        builder.Property(x => x.Texto).HasMaxLength(2000).IsRequired();

        builder.HasIndex(x => new { x.SignalId, x.FechaComentario });
    }
}
