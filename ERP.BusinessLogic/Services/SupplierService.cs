using ERP.BusinessLogic.DTOs;
using ERP.DataAccess.Data;
using ERP.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ERP.BusinessLogic.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ApplicationDbContext _context;

        public SupplierService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Supplier>> GetAllAsync()
        {
            return await _context.Suppliers
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<Supplier?> GetByIdAsync(int id)
        {
            return await _context.Suppliers
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Supplier> CreateAsync(CreateSupplierRequest request)
        {
            var supplier = new Supplier
            {
                Name = request.Name,
                CompanyName = request.CompanyName,
                ContactPerson = request.ContactPerson,
                TaxNumber = request.TaxNumber,
                Email = request.Email,
                Phone = request.Phone,
                IsActive = request.IsActive
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            return supplier;
        }

        public async Task UpdateAsync(UpdateSupplierRequest request)
        {
            var supplier = await _context.Suppliers.FindAsync(request.Id)
                ?? throw new InvalidOperationException($"Supplier with Id {request.Id} was not found.");

            supplier.Name = request.Name;
            supplier.CompanyName = request.CompanyName;
            supplier.ContactPerson = request.ContactPerson;
            supplier.TaxNumber = request.TaxNumber;
            supplier.Email = request.Email;
            supplier.Phone = request.Phone;
            supplier.IsActive = request.IsActive;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id)
                ?? throw new InvalidOperationException($"Supplier with Id {id} was not found.");

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();
        }
    }
}
