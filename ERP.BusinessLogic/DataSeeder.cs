using ERP.BusinessLogic.DTOs;
using ERP.BusinessLogic.Services;
using ERP.DataAccess.Data;
using ERP.DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ERP.BusinessLogic
{
    public static class DataSeeder
    {
        public const string AdminUsername = "admin";
        public const string ManagerUsername = "manager";
        public const string EmployeeUsername = "employee";

        // Dev-only default passwords for the seeded demo accounts; change on first login in a real deployment.
        private const string DefaultAdminPassword = "Admin@12345";
        private const string DefaultManagerPassword = "Manager@12345";
        private const string DefaultEmployeePassword = "Employee@12345";

        public static async Task SeedAsync(
            ApplicationDbContext context,
            ICategoryService categoryService,
            ISupplierService supplierService,
            IProductService productService,
            ICustomerService customerService,
            IOrderService orderService)
        {
            await SeedRolesAsync(context);

            var adminUser = await SeedUserAsync(context, AdminUsername, "admin@myerp.local", DefaultAdminPassword, "Admin");
            var managerUser = await SeedUserAsync(context, ManagerUsername, "manager@myerp.local", DefaultManagerPassword, "Manager");
            var employeeUser = await SeedUserAsync(context, EmployeeUsername, "employee@myerp.local", DefaultEmployeePassword, "Employee");

            // Demo nomenclatures/products/customers/orders are seeded together, gated on Categories
            // being empty, so a fresh database gets a full presentable dataset exactly once.
            if (!await context.Categories.AnyAsync())
            {
                await SeedDemoDataAsync(categoryService, supplierService, productService, customerService, orderService, managerUser.Id, employeeUser.Id);
            }
        }

        private static async Task SeedRolesAsync(ApplicationDbContext context)
        {
            var roleNames = new[] { "Admin", "Manager", "Employee" };

            foreach (var roleName in roleNames)
            {
                var roleExists = await context.Roles.AnyAsync(r => r.Name == roleName);
                if (!roleExists)
                {
                    context.Roles.Add(new Role { Name = roleName });
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task<User> SeedUserAsync(ApplicationDbContext context, string username, string email, string password, string roleName)
        {
            var user = await context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                user = new User
                {
                    Username = username,
                    Email = email,
                    IsActive = true
                };

                var hasher = new PasswordHasher<User>();
                user.PasswordHash = hasher.HashPassword(user, password);

                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            var hasRole = await context.UserRoles
                .AnyAsync(ur => ur.UserId == user.Id && ur.Role.Name == roleName);

            if (!hasRole)
            {
                var role = await context.Roles.FirstAsync(r => r.Name == roleName);
                context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });
                await context.SaveChangesAsync();
            }

            return user;
        }

        private static async Task SeedDemoDataAsync(
            ICategoryService categoryService,
            ISupplierService supplierService,
            IProductService productService,
            ICustomerService customerService,
            IOrderService orderService,
            int managerUserId,
            int employeeUserId)
        {
            var electronics = await categoryService.CreateAsync(new CreateCategoryRequest
            {
                Name = "Electronics",
                Description = "Computers, peripherals and accessories"
            });
            var officeSupplies = await categoryService.CreateAsync(new CreateCategoryRequest
            {
                Name = "Office Supplies",
                Description = "Stationery and everyday office consumables"
            });
            var furniture = await categoryService.CreateAsync(new CreateCategoryRequest
            {
                Name = "Furniture",
                Description = "Desks, chairs and office furniture"
            });

            var techSupplier = await supplierService.CreateAsync(new CreateSupplierRequest
            {
                Name = "TechSource Ltd.",
                CompanyName = "TechSource Ltd.",
                ContactPerson = "Ivan Petrov",
                TaxNumber = "BG123456789",
                Email = "sales@techsource.example",
                Phone = "+359888100200",
                IsActive = true
            });
            var officeSupplier = await supplierService.CreateAsync(new CreateSupplierRequest
            {
                Name = "OfficePlus EOOD",
                CompanyName = "OfficePlus EOOD",
                ContactPerson = "Maria Ivanova",
                TaxNumber = "BG987654321",
                Email = "orders@officeplus.example",
                Phone = "+359888300400",
                IsActive = true
            });

            var mouse = await productService.CreateAsync(new CreateProductRequest
            {
                Name = "Wireless Mouse",
                Description = "Ergonomic 2.4GHz wireless mouse",
                SKU = "DEMO-EL-001",
                PurchasePrice = 8.50m,
                SalePrice = 14.99m,
                QuantityInStock = 40,
                MinimumQuantity = 10,
                IsActive = true,
                CategoryId = electronics.Id,
                SupplierId = techSupplier.Id
            });
            var keyboard = await productService.CreateAsync(new CreateProductRequest
            {
                Name = "Mechanical Keyboard",
                Description = "Backlit mechanical keyboard, blue switches",
                SKU = "DEMO-EL-002",
                PurchasePrice = 25.00m,
                SalePrice = 44.90m,
                QuantityInStock = 15,
                MinimumQuantity = 5,
                IsActive = true,
                CategoryId = electronics.Id,
                SupplierId = techSupplier.Id
            });
            await productService.CreateAsync(new CreateProductRequest
            {
                Name = "24\" Monitor",
                Description = "Full HD IPS monitor",
                SKU = "DEMO-EL-003",
                PurchasePrice = 95.00m,
                SalePrice = 159.00m,
                QuantityInStock = 3,
                MinimumQuantity = 5, // below minimum on purpose, to demo low-stock highlighting
                IsActive = true,
                CategoryId = electronics.Id,
                SupplierId = techSupplier.Id
            });
            var paper = await productService.CreateAsync(new CreateProductRequest
            {
                Name = "A4 Paper Ream",
                Description = "500 sheets, 80gsm",
                SKU = "DEMO-OF-001",
                PurchasePrice = 3.20m,
                SalePrice = 5.50m,
                QuantityInStock = 200,
                MinimumQuantity = 50,
                IsActive = true,
                CategoryId = officeSupplies.Id,
                SupplierId = officeSupplier.Id
            });
            await productService.CreateAsync(new CreateProductRequest
            {
                Name = "Ballpoint Pen Pack (10)",
                Description = "Blue ink ballpoint pens, pack of 10",
                SKU = "DEMO-OF-002",
                PurchasePrice = 1.80m,
                SalePrice = 3.20m,
                QuantityInStock = 8,
                MinimumQuantity = 20, // below minimum on purpose, to demo low-stock highlighting
                IsActive = true,
                CategoryId = officeSupplies.Id,
                SupplierId = officeSupplier.Id
            });
            var chair = await productService.CreateAsync(new CreateProductRequest
            {
                Name = "Ergonomic Office Chair",
                Description = "Adjustable height and lumbar support",
                SKU = "DEMO-FU-001",
                PurchasePrice = 60.00m,
                SalePrice = 119.00m,
                QuantityInStock = 12,
                MinimumQuantity = 3,
                IsActive = true,
                CategoryId = furniture.Id,
                SupplierId = officeSupplier.Id
            });

            var sofiaTrading = await customerService.CreateAsync(new CreateCustomerRequest
            {
                Name = "Sofia Trading Ltd.",
                CompanyName = "Sofia Trading Ltd.",
                TaxNumber = "BG111222333",
                Email = "office@sofiatrading.example",
                Phone = "+359888555111",
                Address = "12 Vitosha Blvd, Sofia",
                IsActive = true
            });
            var plovdivRetail = await customerService.CreateAsync(new CreateCustomerRequest
            {
                Name = "Plovdiv Retail Group",
                CompanyName = "Plovdiv Retail Group",
                TaxNumber = "BG444555666",
                Email = "contact@plovdivretail.example",
                Phone = "+359888555222",
                Address = "5 Central Square, Plovdiv",
                IsActive = true
            });

            await orderService.CreateOrderAsync(new CreateOrderRequest
            {
                OrderNumber = "ORD-SEED-001",
                CustomerId = sofiaTrading.Id,
                UserId = employeeUserId,
                Lines = new List<CreateOrderLineRequest>
                {
                    new() { ProductId = mouse.Id, Quantity = 5, UnitPrice = mouse.SalePrice, Discount = 0m },
                    new() { ProductId = paper.Id, Quantity = 10, UnitPrice = paper.SalePrice, Discount = 0m }
                }
            });

            await orderService.CreateOrderAsync(new CreateOrderRequest
            {
                OrderNumber = "ORD-SEED-002",
                CustomerId = plovdivRetail.Id,
                UserId = managerUserId,
                Lines = new List<CreateOrderLineRequest>
                {
                    new() { ProductId = keyboard.Id, Quantity = 2, UnitPrice = keyboard.SalePrice, Discount = 5.00m },
                    new() { ProductId = chair.Id, Quantity = 1, UnitPrice = chair.SalePrice, Discount = 0m }
                }
            });
        }
    }
}
