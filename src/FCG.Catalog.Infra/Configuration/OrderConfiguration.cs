using FCG.Catalog.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Catalog.Infra.Configuration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Order");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnType("INT").UseIdentityColumn();
            builder.Property(p => p.OrderDate).HasColumnType("DATETIME").IsRequired();
            builder.Property(p => p.UserId).HasColumnType("INT").IsRequired();
            builder.Property(p => p.GameId).HasColumnType("INT").IsRequired();
            builder.Property(p => p.Price).HasColumnType("DECIMAL(12,2)").IsRequired();
            builder.Property(p => p.PaymentStatus).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(p => p.CardName).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(p => p.CardNumber).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(p => p.ExpirationDate).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(p => p.Cvv).HasColumnType("VARCHAR(100)").IsRequired();
        }
    }
}
