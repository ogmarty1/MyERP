using ERP.DataAccess.Data;
using ERP.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ERP.BusinessLogic.Tests
{
    /// <summary>
    /// Creates isolated in-memory ApplicationDbContext instances (one unique database per call)
    /// and seeds the minimal reference data most service tests need.
    /// </summary>
    public static class TestDbContextFactory
    {
        public static ApplicationDbContext Create()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        public static Category SeedCategory(this ApplicationDbContext context, string name = "Electronics")
        {
            var category = new Category { Name = name };
            context.Categories.Add(category);
            context.SaveChanges();
            return category;
        }

        public static Supplier SeedSupplier(this ApplicationDbContext context, string name = "Test Supplier")
        {
            var supplier = new Supplier { Name = name, TaxNumber = "BG000000000" };
            context.Suppliers.Add(supplier);
            context.SaveChanges();
            return supplier;
        }

        public static Customer SeedCustomer(this ApplicationDbContext context, string name = "Test Customer")
        {
            var customer = new Customer { Name = name, TaxNumber = "BG111111111" };
            context.Customers.Add(customer);
            context.SaveChanges();
            return customer;
        }

        public static Product SeedProduct(
            this ApplicationDbContext context,
            int categoryId,
            int? supplierId = null,
            string sku = "SKU-001",
            int quantityInStock = 100,
            decimal purchasePrice = 5m,
            decimal salePrice = 10m)
        {
            var product = new Product
            {
                Name = "Test Product",
                SKU = sku,
                CategoryId = categoryId,
                SupplierId = supplierId,
                PurchasePrice = purchasePrice,
                SalePrice = salePrice,
                QuantityInStock = quantityInStock,
                MinimumQuantity = 5
            };
            context.Products.Add(product);
            context.SaveChanges();
            return product;
        }

        public static User SeedUser(this ApplicationDbContext context, string email = "user@test.local")
        {
            var user = new User
            {
                FirstName = "Test",
                LastName = "User",
                Email = email,
                PasswordHash = "placeholder"
            };
            context.Users.Add(user);
            context.SaveChanges();
            return user;
        }

        public static Role SeedRole(this ApplicationDbContext context, string name = "Employee")
        {
            var role = new Role { Name = name };
            context.Roles.Add(role);
            context.SaveChanges();
            return role;
        }
    }
}
