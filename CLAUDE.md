# ERP System Project Rules & Context

## Tech Stack
- .NET 8 (Web MVC, BusinessLogic, DataAccess)
- EF Core 8.0.11 + SQL Server Express

## Architecture Constraints (Optimized)
- **Role Management**: Custom Many-to-Many via `UserRoles` join table (Users can have multiple Roles)[cite: 1].
- **Authentication**: Custom Cookie Authentication configured in `Program.cs`. Roles are mapped to `ClaimTypes.Role` at sign-in[cite: 1, 2].
- **Global Security**: A global `AuthorizeFilter` is applied to all endpoints by default, requiring login except for `AccountController`.
- **Delete Behavior**: Strictly use `DeleteBehavior.Restrict` or `NoAction` on major Foreign Keys (Orders, Customers, Users, Products, Categories, Suppliers) to prevent accidental cascade deletes[cite: 1]. `DbUpdateException` must be caught gracefully in controllers to display user-friendly error banners[cite: 1]. `OrderDetails` delete is cascaded with `Order`[cite: 1].
- **Data Types**: Financial fields (Prices, TotalAmount) use precise types (`decimal(18,2)`)[cite: 1]. All stock and order quantities (`QuantityInStock`, `MinimumQuantity`, `Quantity`) strictly use `int`[cite: 1].

## Authorization Matrix
- **Employee**: Access to view products and create dynamic orders only[cite: 2]. Navigation links trimmed dynamically[cite: 1, 2].
- **Manager**: Access to edit products, prices, stock, and manage nomenclatures (`Categories` / `Suppliers`)[cite: 2].
- **Admin**: Full system access, including user and role management[cite: 2].

## Project Status & History
- **Sprint 1 & 2**: Core DB layout and initial services setup.
- **Sprint 3**: Fully functional Products CRUD (with low-stock red highlights)[cite: 1, 2], Customers CRUD (with `DbUpdateException` safety)[cite: 1, 2], and dynamic complex Orders creation via JS/jQuery (indexed model binding, automated stock adjustment, and outflow logging)[cite: 1, 2].
- **Sprint 4 (Completed)**: Custom Cookie Authentication & Many-to-Many Authorization implemented[cite: 2]. Automatic DB data seeding for system roles and primary admin added[cite: 2]. Full CRUD interfaces for `Categories` and `Suppliers` nomenclatures completed with robust error handling for `DeleteBehavior.Restrict`[cite: 1, 2]. Everything builds with 0 warnings/errors.

## Sprint 5 Focus (TODO - Reports & Dashboard)
- Implement main Dashboard stats (Total Sales, Pending Orders, Low Stock Alerts).
- Create business reports (Sales by Category, Top Selling Products).
- Final UI polish and system wrap-up.