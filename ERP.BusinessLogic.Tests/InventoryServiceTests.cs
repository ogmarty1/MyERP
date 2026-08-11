using ERP.BusinessLogic.DTOs;
using ERP.BusinessLogic.Services;
using ERP.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ERP.BusinessLogic.Tests
{
    public class InventoryServiceTests
    {
        [Fact]
        public async Task AdjustStockAsync_Inflow_IncreasesStockAndLogsMovement()
        {
            using var context = TestDbContextFactory.Create();
            var category = context.SeedCategory();
            var product = context.SeedProduct(category.Id, quantityInStock: 10);
            var service = new InventoryService(context);

            var movement = await service.AdjustStockAsync(new ManualStockAdjustmentRequest
            {
                ProductId = product.Id,
                Quantity = 15,
                MovementType = MovementType.Inflow,
                Reason = "Restock"
            });

            Assert.Equal(25, product.QuantityInStock);
            Assert.Equal(15, movement.Quantity);
            Assert.Equal(MovementType.Inflow, movement.MovementType);
            Assert.Equal("Restock", movement.Reason);
            Assert.Single(context.InventoryMovements);
        }

        [Fact]
        public async Task AdjustStockAsync_Outflow_DecreasesStockAndLogsMovement()
        {
            using var context = TestDbContextFactory.Create();
            var category = context.SeedCategory();
            var product = context.SeedProduct(category.Id, quantityInStock: 10);
            var service = new InventoryService(context);

            var movement = await service.AdjustStockAsync(new ManualStockAdjustmentRequest
            {
                ProductId = product.Id,
                Quantity = 4,
                MovementType = MovementType.Outflow
            });

            Assert.Equal(6, product.QuantityInStock);
            Assert.Equal(4, movement.Quantity);
            Assert.Equal(MovementType.Outflow, movement.MovementType);
        }

        [Fact]
        public async Task AdjustStockAsync_Outflow_ThrowsWhenExceedsAvailableStock()
        {
            using var context = TestDbContextFactory.Create();
            var category = context.SeedCategory();
            var product = context.SeedProduct(category.Id, quantityInStock: 3);
            var service = new InventoryService(context);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.AdjustStockAsync(new ManualStockAdjustmentRequest
            {
                ProductId = product.Id,
                Quantity = 4,
                MovementType = MovementType.Outflow
            }));

            Assert.Equal(3, product.QuantityInStock);
        }

        [Theory]
        [InlineData(MovementType.Inflow)]
        [InlineData(MovementType.Outflow)]
        public async Task AdjustStockAsync_ThrowsWhenQuantityIsZeroOrLess(MovementType movementType)
        {
            using var context = TestDbContextFactory.Create();
            var category = context.SeedCategory();
            var product = context.SeedProduct(category.Id, quantityInStock: 10);
            var service = new InventoryService(context);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.AdjustStockAsync(new ManualStockAdjustmentRequest
            {
                ProductId = product.Id,
                Quantity = 0,
                MovementType = movementType
            }));
        }

        [Fact]
        public async Task AdjustStockAsync_Adjustment_SetsAbsoluteStockLevelAndLogsSignedDelta()
        {
            using var context = TestDbContextFactory.Create();
            var category = context.SeedCategory();
            var product = context.SeedProduct(category.Id, quantityInStock: 10);
            var service = new InventoryService(context);

            // Physical count found 7 units, not the 10 on record - a downward correction.
            var movement = await service.AdjustStockAsync(new ManualStockAdjustmentRequest
            {
                ProductId = product.Id,
                Quantity = 7,
                MovementType = MovementType.Adjustment,
                Reason = "Stocktake"
            });

            Assert.Equal(7, product.QuantityInStock);
            Assert.Equal(-3, movement.Quantity);
            Assert.Equal(MovementType.Adjustment, movement.MovementType);
        }

        [Fact]
        public async Task AdjustStockAsync_Adjustment_AllowsZeroButNotNegative()
        {
            using var context = TestDbContextFactory.Create();
            var category = context.SeedCategory();
            var product = context.SeedProduct(category.Id, quantityInStock: 10);
            var service = new InventoryService(context);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.AdjustStockAsync(new ManualStockAdjustmentRequest
            {
                ProductId = product.Id,
                Quantity = -1,
                MovementType = MovementType.Adjustment
            }));

            var movement = await service.AdjustStockAsync(new ManualStockAdjustmentRequest
            {
                ProductId = product.Id,
                Quantity = 0,
                MovementType = MovementType.Adjustment
            });

            Assert.Equal(0, product.QuantityInStock);
            Assert.Equal(-10, movement.Quantity);
        }

        [Fact]
        public async Task AdjustStockAsync_ThrowsWhenProductNotFound()
        {
            using var context = TestDbContextFactory.Create();
            var service = new InventoryService(context);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.AdjustStockAsync(new ManualStockAdjustmentRequest
            {
                ProductId = 999,
                Quantity = 5,
                MovementType = MovementType.Inflow
            }));
        }
    }
}
