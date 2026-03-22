using FCG.Catalog.Domain.Models.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Catalog.Infra.Configuration
{
    public class GameLibraryItemConfiguration : IEntityTypeConfiguration<GameLibraryItem>
    {
        public void Configure(EntityTypeBuilder<GameLibraryItem> builder)
        {
            builder.ToTable("GameLibraryItem");

            builder.HasKey("GameLibraryId", "GameId");

            builder.Property(p => p.GameId)
                .ValueGeneratedNever()
                .HasColumnType("uniqueidentifier")
                .IsRequired();

            builder.Property(p => p.Name).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(p => p.Platform).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(p => p.PublisherName).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(p => p.Description).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(p => p.UnitPrice).HasColumnType("DECIMAL(12,2)").IsRequired();
        }
    }
}
