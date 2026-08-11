using ERP.BusinessLogic.DTOs;
using ERP.BusinessLogic.Services;
using ERP.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ERP.BusinessLogic.Tests
{
    public class OrderServiceTests
    {
        [Fact]
        public async Task CreateOrderAsync_DecreasesStockAndLogsOutflow()
        {
            using var context = TestDbContextFactory.Create();
            var category = context.SeedCategory();
            var product = context.SeedProduct(category.Id, quantityInStock: 20, salePrice: 10m);
            var customer = context.SeedCustomer();
            var user = context.SeedUser();
            var service = new OrderService(context);

            var order = await service.CreateOrderAsync(new CreateOrderRequest
            {
                OrderNumber = "ORD-1",
                CustomerId = customer.Id,
                UserId = user.Id,
                Lines = new List<CreateOrderLineRequest>
                {
                    new() { ProductId = product.Id, Quantity = 5, UnitPrice = 10m, Discount = 0m }
                }
            });

            Assert.Equal(15, product.QuantityInStock);
            Assert.Equal(50m, order.TotalAmount);
            Assert.Equal(OrderStatus.Pending, order.Status);

            var movement = Assert.Single(context.InventoryMovements);
            Assert.Equal(MovementType.Outflow, movement.MovementType);
            Assert.Equal(5, movement.Quantity);
            Assert.Equal(order.Id, movement.OrderId);
        }

        [Fact]
        public async Task CreateOrderAsync_AllowsNegativeStockAsBackorder()
        {
            using var context = TestDbContextFactory.Create();
            var category = context.SeedCategory();
            var product = context.SeedProduct(category.Id, quantityInStock: 2, salePrice: 10m);
            var customer = context.SeedCustomer();
            var user = context.SeedUser();
            var service = new OrderService(context);

            await service.CreateOrderAsync(new CreateOrderRequest
            {
                OrderNumber = "ORD-2",
                CustomerId = customer.Id,
                UserId = user.Id,
                Lines = new List<CreateOrderLineRequest>
                {
                    new() { ProductId = product.Id, Quantity = 5, UnitPrice = 10m, Discount = 0m }
                }
            });

            Assert.Equal(-3, product.QuantityInStock);
        }

        [Fact]
        public async Task CreateOrderAsync_ThrowsWhenNoLines()
        {
            using var context = TestDbContextFactory.Create();
            var customer = context.SeedCustomer();
            var user = context.SeedUser();
            var service = new OrderService(context);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateOrderAsync(new CreateOrderRequest
            {
                OrderNumber = "ORD-3",
                CustomerId = customer.Id,
                UserId = user.Id,
                Lines = new List<CreateOrderLineRequest>()
            }));
        }

        [Fact]
        public async Task UpdateOrderItemsAsync_QuantityIncrease_DecreasesStockFurther()
        {
            var seeded = await SeedOrderWithOneLineAsync(quantity: 5, stock: 20);
            using var context = seeded.Context;
            var order = seeded.Order;
            var product = seeded.Product;
            var service = new OrderService(context);

            var line = order.OrderDetails.Single();
            await service.UpdateOrderItemsAsync(new UpdateOrderItemsRequest
            {
                OrderId = order.Id,
                Lines = new List<UpdateOrderLineRequest>
                {
                    new() { OrderDetailId = line.Id, ProductId = product.Id, Quantity = 8 }
                }
            });

            Assert.Equal(12, product.QuantityInStock); // started at 15 after the initial order of 5 from 20
            var outflowAdjustment = context.InventoryMovements.Single(m => m.Reason == "Order item quantity adjusted");
            Assert.Equal(MovementType.Outflow, outflowAdjustment.MovementType);
            Assert.Equal(3, outflowAdjustment.Quantity);
        }

        [Fact]
        public async Task UpdateOrderItemsAsync_QuantityDecrease_RestoresStock()
        {
            var seeded = await SeedOrderWithOneLineAsync(quantity: 5, stock: 20);
            using var context = seeded.Context;
            var order = seeded.Order;
            var product = seeded.Product;
            var service = new OrderService(context);

            var line = order.OrderDetails.Single();
            await service.UpdateOrderItemsAsync(new UpdateOrderItemsRequest
            {
                OrderId = order.Id,
                Lines = new List<UpdateOrderLineRequest>
                {
                    new() { OrderDetailId = line.Id, ProductId = product.Id, Quantity = 2 }
                }
            });

            Assert.Equal(18, product.QuantityInStock); // 20 - 5 (order) + 3 (reduced back)
            var inflowAdjustment = context.InventoryMovements.Single(m => m.Reason == "Order item quantity adjusted");
            Assert.Equal(MovementType.Inflow, inflowAdjustment.MovementType);
            Assert.Equal(3, inflowAdjustment.Quantity);
        }

        [Fact]
        public async Task UpdateOrderItemsAsync_AddingNewLine_DecreasesStockForNewLine()
        {
            using var context = TestDbContextFactory.Create();
            var category = context.SeedCategory();
            var product1 = context.SeedProduct(category.Id, sku: "SKU-1", quantityInStock: 20, salePrice: 10m);
            var product2 = context.SeedProduct(category.Id, sku: "SKU-2", quantityInStock: 30, salePrice: 20m);
            var customer = context.SeedCustomer();
            var user = context.SeedUser();
            var service = new OrderService(context);

            var order = await service.CreateOrderAsync(new CreateOrderRequest
            {
                OrderNumber = "ORD-ADD",
                CustomerId = customer.Id,
                UserId = user.Id,
                Lines = new List<CreateOrderLineRequest>
                {
                    new() { ProductId = product1.Id, Quantity = 5, UnitPrice = 10m, Discount = 0m }
                }
            });
            var existingLine = order.OrderDetails.Single();

            await service.UpdateOrderItemsAsync(new UpdateOrderItemsRequest
            {
                OrderId = order.Id,
                Lines = new List<UpdateOrderLineRequest>
                {
                    new() { OrderDetailId = existingLine.Id, ProductId = product1.Id, Quantity = 5 },
                    new() { OrderDetailId = null, ProductId = product2.Id, Quantity = 3 }
                }
            });

            Assert.Equal(15, product1.QuantityInStock); // unchanged
            Assert.Equal(27, product2.QuantityInStock); // 30 - 3 (new line)
            Assert.Equal(2, order.OrderDetails.Count);
        }

        [Fact]
        public async Task UpdateOrderItemsAsync_RemovedLine_RestoresStockAndDeletesRow()
        {
            using var context = TestDbContextFactory.Create();
            var category = context.SeedCategory();
            var product1 = context.SeedProduct(category.Id, sku: "SKU-1", quantityInStock: 20, salePrice: 10m);
            var product2 = context.SeedProduct(category.Id, sku: "SKU-2", quantityInStock: 30, salePrice: 20m);
            var customer = context.SeedCustomer();
            var user = context.SeedUser();
            var service = new OrderService(context);

            var order = await service.CreateOrderAsync(new CreateOrderRequest
            {
                OrderNumber = "ORD-REMOVE",
                CustomerId = customer.Id,
                UserId = user.Id,
                Lines = new List<CreateOrderLineRequest>
                {
                    new() { ProductId = product1.Id, Quantity = 5, UnitPrice = 10m, Discount = 0m },
                    new() { ProductId = product2.Id, Quantity = 3, UnitPrice = 20m, Discount = 0m }
                }
            });
            var lineToKeep = order.OrderDetails.First(l => l.ProductId == product1.Id);

            // Only resubmit the first line - the second is implicitly removed.
            await service.UpdateOrderItemsAsync(new UpdateOrderItemsRequest
            {
                OrderId = order.Id,
                Lines = new List<UpdateOrderLineRequest>
                {
                    new() { OrderDetailId = lineToKeep.Id, ProductId = product1.Id, Quantity = 5 }
                }
            });

            Assert.Equal(30, product2.QuantityInStock); // fully restored
            Assert.Single(order.OrderDetails);
            var removalMovement = context.InventoryMovements.Single(m => m.Reason == "Order item removed");
            Assert.Equal(MovementType.Inflow, removalMovement.MovementType);
            Assert.Equal(3, removalMovement.Quantity);
        }

        [Fact]
        public async Task ChangeStatusAsync_ToShipped_SetsShippedDateOnce()
        {
            var seeded = await SeedOrderWithOneLineAsync(quantity: 1, stock: 10);
            using var context = seeded.Context;
            var order = seeded.Order;
            var service = new OrderService(context);

            await service.ChangeStatusAsync(order.Id, OrderStatus.Shipped);

            Assert.Equal(OrderStatus.Shipped, order.Status);
            Assert.NotNull(order.ShippedDate);
            var firstShippedDate = order.ShippedDate;

            // Changing away and back to Shipped should not overwrite the original ShippedDate.
            await service.ChangeStatusAsync(order.Id, OrderStatus.OnHold);
            await service.ChangeStatusAsync(order.Id, OrderStatus.Shipped);

            Assert.Equal(firstShippedDate, order.ShippedDate);
        }

        [Fact]
        public async Task ChangeStatusAsync_ThrowsOnceOrderIsCancelled()
        {
            var seeded = await SeedOrderWithOneLineAsync(quantity: 1, stock: 10);
            using var context = seeded.Context;
            var order = seeded.Order;
            var service = new OrderService(context);

            await service.CancelOrderAsync(order.Id);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.ChangeStatusAsync(order.Id, OrderStatus.Shipped));
        }

        [Fact]
        public async Task CancelOrderAsync_RestoresStockForAllLinesAndLogsInflow()
        {
            using var context = TestDbContextFactory.Create();
            var category = context.SeedCategory();
            var product1 = context.SeedProduct(category.Id, sku: "SKU-1", quantityInStock: 20, salePrice: 10m);
            var product2 = context.SeedProduct(category.Id, sku: "SKU-2", quantityInStock: 30, salePrice: 20m);
            var customer = context.SeedCustomer();
            var user = context.SeedUser();
            var service = new OrderService(context);

            var order = await service.CreateOrderAsync(new CreateOrderRequest
            {
                OrderNumber = "ORD-CANCEL",
                CustomerId = customer.Id,
                UserId = user.Id,
                Lines = new List<CreateOrderLineRequest>
                {
                    new() { ProductId = product1.Id, Quantity = 5, UnitPrice = 10m, Discount = 0m },
                    new() { ProductId = product2.Id, Quantity = 3, UnitPrice = 20m, Discount = 0m }
                }
            });

            await service.CancelOrderAsync(order.Id);

            Assert.Equal(OrderStatus.Cancelled, order.Status);
            Assert.Equal(20, product1.QuantityInStock);
            Assert.Equal(30, product2.QuantityInStock);

            var cancelMovements = context.InventoryMovements.Where(m => m.Reason == "Order cancelled").ToList();
            Assert.Equal(2, cancelMovements.Count);
            Assert.All(cancelMovements, m => Assert.Equal(MovementType.Inflow, m.MovementType));
        }

        [Fact]
        public async Task CancelOrderAsync_ThrowsWhenAlreadyCancelled()
        {
            var seeded = await SeedOrderWithOneLineAsync(quantity: 1, stock: 10);
            using var context = seeded.Context;
            var order = seeded.Order;
            var service = new OrderService(context);

            await service.CancelOrderAsync(order.Id);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.CancelOrderAsync(order.Id));
        }

        [Fact]
        public async Task UpdateOrderItemsAsync_ThrowsOnceOrderIsCancelled()
        {
            var seeded = await SeedOrderWithOneLineAsync(quantity: 1, stock: 10);
            using var context = seeded.Context;
            var order = seeded.Order;
            var product = seeded.Product;
            var service = new OrderService(context);
            var line = order.OrderDetails.Single();

            await service.CancelOrderAsync(order.Id);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateOrderItemsAsync(new UpdateOrderItemsRequest
            {
                OrderId = order.Id,
                Lines = new List<UpdateOrderLineRequest>
                {
                    new() { OrderDetailId = line.Id, ProductId = product.Id, Quantity = 2 }
                }
            }));
        }

        private static async Task<(ERP.DataAccess.Data.ApplicationDbContext Context, Order Order, Product Product)> SeedOrderWithOneLineAsync(int quantity, int stock)
        {
            var context = TestDbContextFactory.Create();
            var category = context.SeedCategory();
            var product = context.SeedProduct(category.Id, quantityInStock: stock, salePrice: 10m);
            var customer = context.SeedCustomer();
            var user = context.SeedUser();
            var service = new OrderService(context);

            var order = await service.CreateOrderAsync(new CreateOrderRequest
            {
                OrderNumber = "ORD-SEED",
                CustomerId = customer.Id,
                UserId = user.Id,
                Lines = new List<CreateOrderLineRequest>
                {
                    new() { ProductId = product.Id, Quantity = quantity, UnitPrice = 10m, Discount = 0m }
                }
            });

            return (context, order, product);
        }
    }
}
