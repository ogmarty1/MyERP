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
            if (request.Quantity <= 0)
                throw new InvalidOperationException("Adjustment quantity must be greater than zero.");

            var product = await _context.Products.FindAsync(request.ProductId)
                ?? throw new InvalidOperationException($"Product with Id {request.ProductId} was not found.");

            // Ръчна корекция без поръчка - OrderId остава null
            var signedQuantity = request.MovementType == MovementType.Inflow
                ? request.Quantity
                : -request.Quantity;
            product.QuantityInStock += signedQuantity;

            var movement = new InventoryMovement
            {
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                MovementType = request.MovementType,
                Date = DateTime.UtcNow,
                OrderId = null
            };

            _context.InventoryMovements.Add(movement);

            await _context.SaveChangesAsync();

            return movement;
        }
    }
}
