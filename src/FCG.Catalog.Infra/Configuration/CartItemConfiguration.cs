using FCG.Catalog.Domain.Models.Cart;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Catalog.Infra.Configuration
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.ToTable("CartItem");

            builder.HasKey("CartId", "GameId");

            builder.Property(p => p.GameId)
                .ValueGeneratedNever()
                .HasColumnType("uniqueidentifier")
                .IsRequired();

            builder.Property(p => p.Name).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(p => p.Platform).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(p => p.PublisherName).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(p => p.Description).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(p => p.UnitPrice).HasColumnType("DECIMAL(12,2)").IsRequired();
            builder.Property(p => p.Quantity).HasColumnType("INT").IsRequired();

            builder.Ignore(p => p.Total);
        }
    }
}
