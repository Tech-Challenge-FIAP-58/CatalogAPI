using FIAP.FCG.CATALOG.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FIAP.FCG.CATALOG.Infra.Configuration
{
	public class CatalogConfiguration : IEntityTypeConfiguration<Catalog>
	{
		public void Configure(EntityTypeBuilder<Catalog> builder)
		{
			builder.ToTable("Catalog");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnType("INT").UseIdentityColumn();
            builder.Property(p => p.UserId).HasColumnType("INT");
            builder.Property(p => p.GameId).HasColumnType("INT");
            builder.Property(p => p.Price).HasColumnType("DECIMAL(18,2)");
		}
	}
}

