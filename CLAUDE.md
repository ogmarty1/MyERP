# ERP System Project Rules & Context

## Tech Stack
- .NET 8 (Web MVC, BusinessLogic, DataAccess)[cite: 3]
- EF Core 8.0.11 + SQL Server Express[cite: 3]

## Architecture Constraints (Optimized)
- **Role Management**: Kept as Many-to-Many (Users can have multiple Roles) via a UserRoles join table for better business flexibility[cite: 1, 3].
- **Delete Behavior**: Strictly use `DeleteBehavior.Restrict` or `NoAction` on major Foreign Keys (Orders, Customers, Users, Products) to prevent accidental cascade deletes of historical data[cite: 1, 3]. `OrderDetails` delete cascaded with `Order`[cite: 1].
- **Data Types**: Financial fields (Prices, TotalAmount) must use precise types (`decimal(18,2)`)[cite: 1, 3]. All stock and order quantities (`QuantityInStock`, `MinimumQuantity`, `Quantity`) must strictly use `int`[cite: 1].

## Current Sprint 4 Status & TODO
- **Completed in Sprint 3**: Fully functional Web UI (`ERP.Web`) with Bootstrap[cite: 2]. Implementation of Products CRUD (with low-stock red highlights), Customers CRUD (with `DbUpdateException` handling for Restrict behavior), and complex dynamic Orders creation using JavaScript/jQuery for client-side calculations and indexed model binding[cite: 1, 2]. All changes compile cleanly and are fully integrated with Sprint 2 Services (`OrderService.CreateOrderAsync` and `InventoryService.AdjustStockAsync`)[cite: 1, 2].
- **Sprint 4 Focus (TODO)**: 
  - Implement User Authentication (Login) and Role-based Authorization tailored for the Many-to-Many setup[cite: 1, 2].
  - Create simple management CRUD layouts for auxiliary nomenclatures (`Categories` and `Suppliers`)[cite: 2].
  - Setup automatic Data Seeding for system Roles (`Admin`, `Manager`, `Employee`) and an initial administrator account upon database startup[cite: 2].