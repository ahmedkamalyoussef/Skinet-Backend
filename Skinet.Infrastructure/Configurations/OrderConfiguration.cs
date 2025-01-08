using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skinet.Core.Entites.OrderAggregate;

namespace Skinet.Infrastructure.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsOne(o => o.ShippingAddress, a => a.WithOwner());
            builder.OwnsOne(o => o.PaymentSummary, a => a.WithOwner());
            builder.Property(o => o.Status).HasConversion(
                    o => o.ToString(),
                    o => (OrderStatus)Enum.Parse(typeof(OrderStatus), o)
            );
            builder.Property(o => o.SubTotal).HasColumnType("decimal(18,2)");
            builder.HasMany(o => o.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Property(o => o.OrderDate).HasConversion(
                d => d.ToUniversalTime(),
                d => DateTime.SpecifyKind(d, DateTimeKind.Utc)
            );
        }
    }
}