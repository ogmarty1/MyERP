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

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.Id == id);
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
