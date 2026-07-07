# ERP System Project Rules & Context

## Tech Stack
- .NET 8 (Web MVC, BusinessLogic, DataAccess)
- EF Core 8.0.11 + SQL Server Express

## Architecture Constraints (Optimized)
- **Role Management**: Kept as Many-to-Many (Users can have multiple Roles) for better business flexibility.
- **Delete Behavior**: Strictly use `DeleteBehavior.Restrict` or `NoAction` on major Foreign Keys (Orders, Customers, Users) to prevent accidental cascade deletes of historical data.
- **Data Types**: All financial fields and stock quantities must use precise types (e.g., `decimal(18,2)` for prices).

## Current Sprint 2 Status & TODO
- Entities created: Category, Product, User, Role, UserRole, Customer, Supplier, Order, OrderDetail, OrderStatus.
- **Missing Core Entity**: `InventoryMovement` must be created to support automatic stock updates.