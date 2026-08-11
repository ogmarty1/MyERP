using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ERP.DataAccess.Models;

namespace ERP.DataAccess.Configurations
{
    public class InventoryMovementConfiguration : IEntityTypeConfiguration<InventoryMovement>
    {
        public void Configure(EntityTypeBuilder<InventoryMovement> builder)
        {
            builder.HasOne(im => im.Product)
                .WithMany()
                .HasForeignKey(im => im.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order е опционален - ръчните корекции на склада нямат свързана поръчка
            builder.HasOne(im => im.Order)
                .WithMany()
                .HasForeignKey(im => im.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // PurchaseOrder е опционален - само движенията от получени доставки го задават
            builder.HasOne(im => im.PurchaseOrder)
                .WithMany()
                .HasForeignKey(im => im.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
