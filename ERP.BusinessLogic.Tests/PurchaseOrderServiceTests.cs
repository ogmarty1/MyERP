using ERP.BusinessLogic.DTOs;
using ERP.BusinessLogic.Services;
using ERP.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ERP.BusinessLogic.Tests
{
    public class PurchaseOrderServiceTests
    {
        [Fact]
        public async Task CreateAsync_DoesNotChangeStockUntilReceived()
        {
            using var context = TestDbContextFactory.Create();
            var category = context.SeedCategory();
            var product = context.SeedProduct(category.Id, quantityInStock: 10, purchasePrice: 5m);
            var supplier = context.SeedSupplier();
            var user = context.SeedUser();
            var service = new PurchaseOrderService(context);

            var po = await service.CreateAsync(new CreatePurchaseOrderRequest
            {
                SupplierId = supplier.Id,
                CreatedByUserId = user.Id,
                Lines = new List<CreatePurchaseOrderLineRequest>
                {
                    new() { ProductId = product.Id, Quantity = 20, UnitPrice = 5m }
                }
            });

            Assert.Equal(10, product.QuantityInStock); // unchanged
            Assert.Equal(100m, po.TotalAmount);
            Assert.Equal(PurchaseOrderStatus.Ordered, po.Status);
            Assert.Empty(context.InventoryMovements);
        }

        [Fact]
        public async Task CreateAsync_ThrowsWhenNoLines()
        {
            using var context = TestDbContextFactory.Create();
            var supplier = context.SeedSupplier();
            var user = context.SeedUser();
            var service = new PurchaseOrderService(context);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(new CreatePurchaseOrderRequest
            {
                SupplierId = supplier.Id,
                CreatedByUserId = user.Id,
                Lines = new List<CreatePurchaseOrderLineRequest>()
            }));
        }

        [Fact]
        public async Task UpdateItemsAsync_QuantityChange_DoesNotTouchStock()
        {
            var seeded = await SeedOrderedPurchaseOrderAsync(quantity: 10, purchasePrice: 5m, stock: 10);
            using var context = seeded.Context;
            var po = seeded.PurchaseOrder;
            var product = seeded.Product;
            var service = new PurchaseOrderService(context);
            var line = po.PurchaseOrderDetails.Single();

            await service.UpdateItemsAsync(new UpdatePurchaseOrderItemsRequest
            {
                PurchaseOrderId = po.Id,
                Lines = new List<UpdatePurchaseOrderLineRequest>
                {
                    new() { PurchaseOrderDetailId = line.Id, ProductId = product.Id, Quantity = 25, UnitPrice = 5m }
                }
            });

            Assert.Equal(10, product.QuantityInStock); // still unaffected pre-receipt
            Assert.Equal(125m, po.TotalAmount);
            Assert.Empty(context.InventoryMovements);
        }

        [Fact]
        public async Task UpdateItemsAsync_ThrowsWhenAlreadyReceived()
        {
            var seeded = await SeedOrderedPurchaseOrderAsync(quantity: 10, purchasePrice: 5m, stock: 10);
            using var context = seeded.Context;
            var po = seeded.PurchaseOrder;
            var product = seeded.Product;
            var service = new PurchaseOrderService(context);
            var line = po.PurchaseOrderDetails.Single();

            await service.MarkAsReceivedAsync(po.Id);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateItemsAsync(new UpdatePurchaseOrderItemsRequest
            {
                PurchaseOrderId = po.Id,
                Lines = new List<UpdatePurchaseOrderLineRequest>
                {
                    new() { PurchaseOrderDetailId = line.Id, ProductId = product.Id, Quantity = 25, UnitPrice = 5m }
                }
            }));
        }

        [Fact]
        public async Task MarkAsReceivedAsync_IncreasesStockAndLogsInflow()
        {
            var seeded = await SeedOrderedPurchaseOrderAsync(quantity: 15, purchasePrice: 5m, stock: 10);
            using var context = seeded.Context;
            var po = seeded.PurchaseOrder;
            var product = seeded.Product;
            var service = new PurchaseOrderService(context);

            await service.MarkAsReceivedAsync(po.Id);

            Assert.Equal(25, product.QuantityInStock);
            Assert.Equal(PurchaseOrderStatus.Received, po.Status);

            var movement = Assert.Single(context.InventoryMovements);
            Assert.Equal(MovementType.Inflow, movement.MovementType);
            Assert.Equal(15, movement.Quantity);
            Assert.Equal(po.Id, movement.PurchaseOrderId);
        }

        [Fact]
        public async Task MarkAsReceivedAsync_ThrowsWhenAlreadyReceived()
        {
            var seeded = await SeedOrderedPurchaseOrderAsync(quantity: 5, purchasePrice: 5m, stock: 10);
            using var context = seeded.Context;
            var po = seeded.PurchaseOrder;
            var service = new PurchaseOrderService(context);

            await service.MarkAsReceivedAsync(po.Id);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.MarkAsReceivedAsync(po.Id));

            // Stock should not have been double-applied by the failed second call.
            Assert.Equal(15, seeded.Product.QuantityInStock);
        }

        private static async Task<(ERP.DataAccess.Data.ApplicationDbContext Context, PurchaseOrder PurchaseOrder, Product Product)> SeedOrderedPurchaseOrderAsync(int quantity, decimal purchasePrice, int stock)
        {
            var context = TestDbContextFactory.Create();
            var category = context.SeedCategory();
            var product = context.SeedProduct(category.Id, quantityInStock: stock, purchasePrice: purchasePrice);
            var supplier = context.SeedSupplier();
            var user = context.SeedUser();
            var service = new PurchaseOrderService(context);

            var po = await service.CreateAsync(new CreatePurchaseOrderRequest
            {
                SupplierId = supplier.Id,
                CreatedByUserId = user.Id,
                Lines = new List<CreatePurchaseOrderLineRequest>
                {
                    new() { ProductId = product.Id, Quantity = quantity, UnitPrice = purchasePrice }
                }
            });

            return (context, po, product);
        }
    }
}
