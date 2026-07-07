using ERP.BusinessLogic.DTOs;
using ERP.DataAccess.Data;
using ERP.DataAccess.Models;

namespace ERP.BusinessLogic.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
        {
            if (request.Lines.Count == 0)
                throw new InvalidOperationException("An order must contain at least one line.");

            var order = new Order
            {
                OrderNumber = request.OrderNumber,
                CustomerId = request.CustomerId,
                UserId = request.UserId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending
            };

            decimal total = 0m;

            foreach (var line in request.Lines)
            {
                var product = await _context.Products.FindAsync(line.ProductId)
                    ?? throw new InvalidOperationException($"Product with Id {line.ProductId} was not found.");

                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = line.ProductId,
                    Quantity = line.Quantity,
                    UnitPrice = line.UnitPrice,
                    Discount = line.Discount
                });

                total += (line.UnitPrice * line.Quantity) - line.Discount;

                // Продажбата намалява наличността - позволяваме отрицателен резултат (backorder)
                product.QuantityInStock -= line.Quantity;

                _context.InventoryMovements.Add(new InventoryMovement
                {
                    ProductId = line.ProductId,
                    Quantity = line.Quantity,
                    MovementType = MovementType.Outflow,
                    Date = DateTime.UtcNow,
                    Order = order
                });
            }

            order.TotalAmount = total;

            _context.Orders.Add(order);

            // Всички промени (Order, OrderDetails, Product наличности, InventoryMovements)
            // се записват в една транзакция чрез единствения SaveChangesAsync.
            await _context.SaveChangesAsync();

            return order;
        }
    }
}
