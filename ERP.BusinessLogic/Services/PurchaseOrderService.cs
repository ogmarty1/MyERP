using ERP.BusinessLogic.DTOs;
using ERP.DataAccess.Data;
using ERP.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ERP.BusinessLogic.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly ApplicationDbContext _context;

        public PurchaseOrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PurchaseOrder>> GetAllAsync()
        {
            return await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .OrderByDescending(po => po.OrderDate)
                .ToListAsync();
        }

        public async Task<PurchaseOrder?> GetByIdAsync(int id)
        {
            return await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.CreatedByUser)
                .Include(po => po.PurchaseOrderDetails)
                    .ThenInclude(pod => pod.Product)
                .FirstOrDefaultAsync(po => po.Id == id);
        }

        public async Task<PurchaseOrder> CreateAsync(CreatePurchaseOrderRequest request)
        {
            if (request.Lines.Count == 0)
                throw new InvalidOperationException("A purchase order must contain at least one item.");

            var purchaseOrder = new PurchaseOrder
            {
                SupplierId = request.SupplierId,
                CreatedByUserId = request.CreatedByUserId,
                OrderDate = DateTime.UtcNow,
                Status = PurchaseOrderStatus.Ordered,
                Notes = request.Notes
            };

            decimal total = 0m;

            foreach (var line in request.Lines)
            {
                var productExists = await _context.Products.AnyAsync(p => p.Id == line.ProductId);
                if (!productExists)
                    throw new InvalidOperationException($"Product with Id {line.ProductId} was not found.");

                purchaseOrder.PurchaseOrderDetails.Add(new PurchaseOrderDetail
                {
                    ProductId = line.ProductId,
                    Quantity = line.Quantity,
                    UnitPrice = line.UnitPrice
                });

                total += line.UnitPrice * line.Quantity;
            }

            purchaseOrder.TotalAmount = total;

            // Placing a purchase order does not touch stock - QuantityInStock/InventoryMovements
            // only change once the order is marked as Received.
            _context.PurchaseOrders.Add(purchaseOrder);
            await _context.SaveChangesAsync();

            return purchaseOrder;
        }

        public async Task UpdateItemsAsync(UpdatePurchaseOrderItemsRequest request)
        {
            if (request.Lines.Count == 0)
                throw new InvalidOperationException("A purchase order must contain at least one item.");

            var purchaseOrder = await _context.PurchaseOrders
                .Include(po => po.PurchaseOrderDetails)
                .FirstOrDefaultAsync(po => po.Id == request.PurchaseOrderId)
                ?? throw new InvalidOperationException($"Purchase order with Id {request.PurchaseOrderId} was not found.");

            if (purchaseOrder.Status != PurchaseOrderStatus.Ordered)
                throw new InvalidOperationException("This purchase order has already been received and can no longer be edited.");

            var existingLines = purchaseOrder.PurchaseOrderDetails.ToDictionary(pod => pod.Id);
            var keptLineIds = new HashSet<int>();
            decimal total = 0m;

            foreach (var line in request.Lines)
            {
                if (line.PurchaseOrderDetailId.HasValue && existingLines.TryGetValue(line.PurchaseOrderDetailId.Value, out var existingLine))
                {
                    keptLineIds.Add(existingLine.Id);
                    existingLine.Quantity = line.Quantity;
                    total += existingLine.UnitPrice * existingLine.Quantity;
                }
                else
                {
                    var productExists = await _context.Products.AnyAsync(p => p.Id == line.ProductId);
                    if (!productExists)
                        throw new InvalidOperationException($"Product with Id {line.ProductId} was not found.");

                    var newLine = new PurchaseOrderDetail
                    {
                        ProductId = line.ProductId,
                        Quantity = line.Quantity,
                        UnitPrice = line.UnitPrice
                    };
                    purchaseOrder.PurchaseOrderDetails.Add(newLine);

                    total += newLine.UnitPrice * newLine.Quantity;
                }
            }

            foreach (var removedLine in existingLines.Values.Where(l => !keptLineIds.Contains(l.Id)))
            {
                purchaseOrder.PurchaseOrderDetails.Remove(removedLine);
                _context.PurchaseOrderDetails.Remove(removedLine);
            }

            purchaseOrder.TotalAmount = total;
            purchaseOrder.Notes = request.Notes;

            await _context.SaveChangesAsync();
        }

        public async Task MarkAsReceivedAsync(int id)
        {
            var purchaseOrder = await _context.PurchaseOrders
                .Include(po => po.PurchaseOrderDetails)
                .FirstOrDefaultAsync(po => po.Id == id)
                ?? throw new InvalidOperationException($"Purchase order with Id {id} was not found.");

            if (purchaseOrder.Status == PurchaseOrderStatus.Received)
                throw new InvalidOperationException("This purchase order has already been received.");

            foreach (var line in purchaseOrder.PurchaseOrderDetails)
            {
                var product = await _context.Products.FindAsync(line.ProductId);
                if (product != null)
                    product.QuantityInStock += line.Quantity;

                _context.InventoryMovements.Add(new InventoryMovement
                {
                    ProductId = line.ProductId,
                    Quantity = line.Quantity,
                    MovementType = MovementType.Inflow,
                    Reason = "Purchase order received",
                    Date = DateTime.UtcNow,
                    PurchaseOrderId = purchaseOrder.Id
                });
            }

            purchaseOrder.Status = PurchaseOrderStatus.Received;

            // Stock, InventoryMovements, and the status flip are all committed in one
            // SaveChangesAsync call so the receipt is applied atomically.
            await _context.SaveChangesAsync();
        }
    }
}
