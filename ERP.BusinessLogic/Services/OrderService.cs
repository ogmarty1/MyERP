using ERP.BusinessLogic.DTOs;
using ERP.DataAccess.Data;
using ERP.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ERP.BusinessLogic.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<List<Order>> GetFilteredAsync(OrderFilterRequest filter)
        {
            var query = _context.Orders.Include(o => o.Customer).AsQueryable();

            if (filter.OrderId.HasValue)
                query = query.Where(o => o.Id == filter.OrderId.Value);

            if (!string.IsNullOrWhiteSpace(filter.CustomerName))
                query = query.Where(o => EF.Functions.Like(o.Customer.Name, $"%{filter.CustomerName.Trim()}%"));

            if (filter.StartDate.HasValue)
                query = query.Where(o => o.OrderDate.Date >= filter.StartDate.Value.Date);

            if (filter.EndDate.HasValue)
                query = query.Where(o => o.OrderDate.Date <= filter.EndDate.Value.Date);

            if (filter.MinTotal.HasValue)
                query = query.Where(o => o.TotalAmount >= filter.MinTotal.Value);

            if (filter.MaxTotal.HasValue)
                query = query.Where(o => o.TotalAmount <= filter.MaxTotal.Value);

            if (filter.Status.HasValue)
                query = query.Where(o => o.Status == filter.Status.Value);

            return await query.OrderByDescending(o => o.OrderDate).ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
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

        public async Task ChangeStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(orderId)
                ?? throw new InvalidOperationException($"Order with Id {orderId} was not found.");

            if (order.Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("This order has been cancelled and its status can no longer be changed.");

            order.Status = status;

            if (status == OrderStatus.Shipped && order.ShippedDate == null)
                order.ShippedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task CancelOrderAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == orderId)
                ?? throw new InvalidOperationException($"Order with Id {orderId} was not found.");

            if (order.Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("This order has already been cancelled.");

            foreach (var line in order.OrderDetails)
            {
                var product = await _context.Products.FindAsync(line.ProductId);
                if (product != null)
                    product.QuantityInStock += line.Quantity;

                _context.InventoryMovements.Add(new InventoryMovement
                {
                    ProductId = line.ProductId,
                    Quantity = line.Quantity,
                    MovementType = MovementType.Inflow,
                    Reason = "Order cancelled",
                    Date = DateTime.UtcNow,
                    OrderId = order.Id
                });
            }

            order.Status = OrderStatus.Cancelled;

            // Stock reversal, InventoryMovements, and the status flip are all committed in one
            // SaveChangesAsync call so cancellation is applied atomically.
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOrderItemsAsync(UpdateOrderItemsRequest request)
        {
            if (request.Lines.Count == 0)
                throw new InvalidOperationException("An order must contain at least one item.");

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == request.OrderId)
                ?? throw new InvalidOperationException($"Order with Id {request.OrderId} was not found.");

            if (order.Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("This order has been cancelled and can no longer be edited.");

            var existingLines = order.OrderDetails.ToDictionary(od => od.Id);
            var keptLineIds = new HashSet<int>();
            decimal total = 0m;

            foreach (var line in request.Lines)
            {
                var product = await _context.Products.FindAsync(line.ProductId)
                    ?? throw new InvalidOperationException($"Product with Id {line.ProductId} was not found.");

                if (line.OrderDetailId.HasValue && existingLines.TryGetValue(line.OrderDetailId.Value, out var existingLine))
                {
                    keptLineIds.Add(existingLine.Id);

                    var delta = line.Quantity - existingLine.Quantity;
                    if (delta != 0)
                    {
                        // Позволяваме отрицателен резултат (backorder), както при създаване на поръчка.
                        product.QuantityInStock -= delta;

                        _context.InventoryMovements.Add(new InventoryMovement
                        {
                            ProductId = line.ProductId,
                            Quantity = Math.Abs(delta),
                            MovementType = delta > 0 ? MovementType.Outflow : MovementType.Inflow,
                            Reason = "Order item quantity adjusted",
                            Date = DateTime.UtcNow,
                            OrderId = order.Id
                        });
                    }

                    existingLine.Quantity = line.Quantity;
                    total += (existingLine.UnitPrice * existingLine.Quantity) - existingLine.Discount;
                }
                else
                {
                    var newLine = new OrderDetail
                    {
                        ProductId = line.ProductId,
                        Quantity = line.Quantity,
                        UnitPrice = product.SalePrice,
                        Discount = 0m
                    };
                    order.OrderDetails.Add(newLine);

                    product.QuantityInStock -= line.Quantity;

                    _context.InventoryMovements.Add(new InventoryMovement
                    {
                        ProductId = line.ProductId,
                        Quantity = line.Quantity,
                        MovementType = MovementType.Outflow,
                        Reason = "Order item added",
                        Date = DateTime.UtcNow,
                        OrderId = order.Id
                    });

                    total += (newLine.UnitPrice * newLine.Quantity) - newLine.Discount;
                }
            }

            foreach (var removedLine in existingLines.Values.Where(l => !keptLineIds.Contains(l.Id)))
            {
                var product = await _context.Products.FindAsync(removedLine.ProductId);
                if (product != null)
                    product.QuantityInStock += removedLine.Quantity;

                _context.InventoryMovements.Add(new InventoryMovement
                {
                    ProductId = removedLine.ProductId,
                    Quantity = removedLine.Quantity,
                    MovementType = MovementType.Inflow,
                    Reason = "Order item removed",
                    Date = DateTime.UtcNow,
                    OrderId = order.Id
                });

                order.OrderDetails.Remove(removedLine);
                _context.OrderDetails.Remove(removedLine);
            }

            order.TotalAmount = total;
            order.Notes = request.Notes;

            await _context.SaveChangesAsync();
        }
    }
}
