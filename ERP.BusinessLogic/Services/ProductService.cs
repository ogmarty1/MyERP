using ERP.BusinessLogic.DTOs;
using ERP.DataAccess.Data;
using ERP.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ERP.BusinessLogic.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<List<Product>> GetFilteredAsync(ProductFilterRequest filter)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var keyword = filter.Keyword.Trim();
                query = query.Where(p =>
                    EF.Functions.Like(p.Name, $"%{keyword}%") ||
                    EF.Functions.Like(p.SKU, $"%{keyword}%"));
            }

            if (filter.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

            if (filter.SupplierId.HasValue)
                query = query.Where(p => p.SupplierId == filter.SupplierId.Value);

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.SalePrice >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.SalePrice <= filter.MaxPrice.Value);

            if (filter.MinStock.HasValue)
                query = query.Where(p => p.QuantityInStock >= filter.MinStock.Value);

            if (filter.MaxStock.HasValue)
                query = query.Where(p => p.QuantityInStock <= filter.MaxStock.Value);

            if (filter.Status.HasValue)
                query = query.Where(p => p.Status == filter.Status.Value);

            if (filter.LowStockOnly)
                query = query.Where(p => p.QuantityInStock <= p.MinimumQuantity);

            return await query.OrderBy(p => p.Name).ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Order>> GetOrderHistoryAsync(int productId)
        {
            return await _context.Orders
                .Where(o => o.OrderDetails.Any(od => od.ProductId == productId))
                .Include(o => o.Customer)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Product> CreateAsync(CreateProductRequest request)
        {
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                SKU = request.SKU,
                PurchasePrice = request.PurchasePrice,
                SalePrice = request.SalePrice,
                QuantityInStock = request.QuantityInStock,
                MinimumQuantity = request.MinimumQuantity,
                IsActive = request.IsActive,
                Status = request.Status,
                Notes = request.Notes,
                CategoryId = request.CategoryId,
                SupplierId = request.SupplierId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task UpdateAsync(UpdateProductRequest request)
        {
            var product = await _context.Products.FindAsync(request.Id)
                ?? throw new InvalidOperationException($"Product with Id {request.Id} was not found.");

            product.Name = request.Name;
            product.Description = request.Description;
            product.SKU = request.SKU;
            product.PurchasePrice = request.PurchasePrice;
            product.SalePrice = request.SalePrice;
            product.QuantityInStock = request.QuantityInStock;
            product.MinimumQuantity = request.MinimumQuantity;
            product.IsActive = request.IsActive;
            product.Status = request.Status;
            product.Notes = request.Notes;
            product.CategoryId = request.CategoryId;
            product.SupplierId = request.SupplierId;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id)
                ?? throw new InvalidOperationException($"Product with Id {id} was not found.");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}
