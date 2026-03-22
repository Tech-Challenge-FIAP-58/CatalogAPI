using FCG.Catalog.Domain.Models.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Catalog.Infra.Configuration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Order");
            builder.Property(p => p.OrderDate).HasColumnType("DATETIME").IsRequired();
            builder.Property(p => p.UserId).HasColumnType("INT").IsRequired();
            builder.Property(p => p.Total).HasColumnType("DECIMAL(12,2)").IsRequired();
            builder.Property(p => p.Status).HasColumnType("INT").IsRequired();

            builder.HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey("OrderId");

            builder.Navigation(o => o.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
