using FCG.Core.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Infra.Configuration
{
    public class CatalogConfiguration : IEntityTypeConfiguration<Catalog>
    {
        public void Configure(EntityTypeBuilder<Catalog> builder)
        {
            builder.ToTable("Catalog");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Price).HasColumnType("DECIMAL(18,2)");
        }
    }
}

