using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ERP.DataAccess.Models;

namespace ERP.DataAccess.Configurations
{
    public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
        {
            builder.HasOne(po => po.Supplier)
                .WithMany()
                .HasForeignKey(po => po.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(po => po.CreatedByUser)
                .WithMany()
                .HasForeignKey(po => po.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
