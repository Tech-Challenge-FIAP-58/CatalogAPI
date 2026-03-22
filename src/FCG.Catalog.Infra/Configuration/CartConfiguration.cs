using FCG.Catalog.Domain.Models.Cart;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Catalog.Infra.Configuration
{
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.ToTable("Cart");
            builder.Property(p => p.UserId).HasColumnType("INT").IsRequired();
            builder.Property(p => p.Total).HasColumnType("DECIMAL(12,2)").IsRequired();
            builder.Property(p => p.Status).HasColumnType("INT").IsRequired();

            builder.HasMany(c => c.Items)
                .WithOne()
                .HasForeignKey("CartId");

            builder.Navigation(c => c.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
