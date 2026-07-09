using ERP.BusinessLogic.DTOs;
using ERP.DataAccess.Data;
using ERP.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ERP.BusinessLogic.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;

        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            return await _context.Customers
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Customer> CreateAsync(CreateCustomerRequest request)
        {
            var customer = new Customer
            {
                Name = request.Name,
                CompanyName = request.CompanyName,
                TaxNumber = request.TaxNumber,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                IsActive = request.IsActive
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return customer;
        }

        public async Task UpdateAsync(UpdateCustomerRequest request)
        {
            var customer = await _context.Customers.FindAsync(request.Id)
                ?? throw new InvalidOperationException($"Customer with Id {request.Id} was not found.");

            customer.Name = request.Name;
            customer.CompanyName = request.CompanyName;
            customer.TaxNumber = request.TaxNumber;
            customer.Email = request.Email;
            customer.Phone = request.Phone;
            customer.Address = request.Address;
            customer.IsActive = request.IsActive;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id)
                ?? throw new InvalidOperationException($"Customer with Id {id} was not found.");

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }
    }
}
