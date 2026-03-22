using FCG.Catalog.Domain.Models.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Catalog.Infra.Configuration
{
    public class GameLibraryConfiguration : IEntityTypeConfiguration<GameLibrary>
    {
        public void Configure(EntityTypeBuilder<GameLibrary> builder)
        {
            builder.ToTable("GameLibrary");
            builder.Property(p => p.UserId).HasColumnType("INT").IsRequired();

            builder.HasMany(library => library.Games)
                .WithOne()
                .HasForeignKey("GameLibraryId");

            builder.Navigation(library => library.Games).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
