using ERP.BusinessLogic.DTOs;
using ERP.DataAccess.Data;
using ERP.DataAccess.Models;

namespace ERP.BusinessLogic.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext _context;

        public InventoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<InventoryMovement> AdjustStockAsync(ManualStockAdjustmentRequest request)
        {
            if (request.MovementType == MovementType.Adjustment && request.Quantity < 0)
                throw new InvalidOperationException("The new stock level cannot be negative.");

            if (request.MovementType != MovementType.Adjustment && request.Quantity <= 0)
                throw new InvalidOperationException("Adjustment quantity must be greater than zero.");

            var product = await _context.Products.FindAsync(request.ProductId)
                ?? throw new InvalidOperationException($"Product with Id {request.ProductId} was not found.");

            int loggedQuantity;

            if (request.MovementType == MovementType.Adjustment)
            {
                // Adjustment: Quantity is the new absolute stock level (e.g. after a physical count).
                loggedQuantity = request.Quantity - product.QuantityInStock;
                product.QuantityInStock = request.Quantity;
            }
            else if (request.MovementType == MovementType.Outflow)
            {
                if (request.Quantity > product.QuantityInStock)
                    throw new InvalidOperationException("Cannot remove more stock than is currently available.");

                loggedQuantity = request.Quantity;
                product.QuantityInStock -= request.Quantity;
            }
            else
            {
                loggedQuantity = request.Quantity;
                product.QuantityInStock += request.Quantity;
            }

            // Ръчна корекция без поръчка - OrderId остава null
            var movement = new InventoryMovement
            {
                ProductId = request.ProductId,
                Quantity = loggedQuantity,
                MovementType = request.MovementType,
                Reason = request.Reason,
                Date = DateTime.UtcNow,
                OrderId = null
            };

            _context.InventoryMovements.Add(movement);

            await _context.SaveChangesAsync();

            return movement;
        }
    }
}
