using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ERP.DataAccess.Models;

namespace ERP.DataAccess.Configurations
{
    public class PurchaseOrderDetailConfiguration : IEntityTypeConfiguration<PurchaseOrderDetail>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrderDetail> builder)
        {
            builder.HasOne(pod => pod.PurchaseOrder)
                .WithMany(po => po.PurchaseOrderDetails)
                .HasForeignKey(pod => pod.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pod => pod.Product)
                .WithMany()
                .HasForeignKey(pod => pod.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
