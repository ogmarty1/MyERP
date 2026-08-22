# ERP System Project Rules & Context

## Tech Stack
- .NET 8 (Web MVC, BusinessLogic, DataAccess)
- EF Core 8.0.11 + SQL Server Express (`Microsoft.Extensions.Identity.Core` for identity types)
- Testing: xUnit + EF Core InMemory (`ERP.BusinessLogic.Tests`)
- Frontend: Bootstrap 5 (+ Bootstrap Icons), jQuery (+ jQuery Validation/Unobtrusive), Chart.js, DataTables (Buttons/Select extensions, JSZip + pdfmake for Excel/PDF export), SweetAlert2 (modal confirmations), Toastr (notifications)

## Architecture & Globalization Constraints
- **Role Management**: Custom Many-to-Many via `UserRoles` join table (Users can have multiple Roles).
- **Authentication**: Custom Cookie Authentication configured in `Program.cs`. Roles are mapped to `ClaimTypes.Role` at sign-in.
- **Global Security**: A global `AuthorizeFilter` is applied to all endpoints by default, requiring login except for `AccountController`.
- **Delete Behavior**: Strictly use `DeleteBehavior.Restrict` or `NoAction` on major Foreign Keys (Orders, Customers, Users, Products, Categories, Suppliers) to prevent accidental cascade deletes. `DbUpdateException` must be caught gracefully in controllers to display user-friendly error banners. `OrderDetails` delete is cascaded with `Order`.
- **Data Types & Currency**: Financial fields (Prices, TotalAmount) use precise types (`decimal(18,2)`). The global app currency is strictly **Euro (€ / EUR)** formatted in views as `€` or `EUR`. Forms with `type="number"` inputs must parse decimals safely without culture binding breaks. All stock/order quantities strictly use `int`.
- **Localization & Multi-Language**: Supported cultures are Bulgarian (`bg`) and English (`en`) via ASP.NET Core `RequestLocalization` / `IStringLocalizer`. Standardize UI texts across both languages.

## Authorization Matrix
- **Employee**: Access to view products and create dynamic orders only. Navigation links trimmed dynamically.
- **Manager**: Access to edit products, prices, stock, manage nomenclatures (`Categories` / `Suppliers`), and view business reports.
- **Admin**: Full system access, including user/role management and full financial metrics.

## Project Status & History
- **Sprint 1 & 2**: Core DB layout and initial services setup.
- **Sprint 3**: Fully functional Products CRUD (with low-stock red highlights), Customers CRUD (with `DbUpdateException` safety), and dynamic complex Orders creation via JS/jQuery (indexed model binding, automated stock adjustment, and outflow logging).
- **Sprint 4**: Custom Cookie Authentication & Many-to-Many Authorization implemented. Automatic DB data seeding for system roles and primary admin. Full CRUD for `Categories` and `Suppliers` with `DbUpdateException` safety.
- **Sprint 5 (Completed - Part 1)**: Interactive Dashboard with 4 KPI cards (role-gated financial stats) and Chart.js integration (Monthly Sales Trends & Top 5 Products). Currency updated globally to **Euro (€)** and multi-language support (BG / EN) configured. Builds with 0 warnings/errors.

## Current Stage Focus (UI/UX Polish & Final Touchups)
- DataTables integration (interactive tables, instant search, pagination, dynamic culture support).
- Data Export capabilities (Excel, PDF/Print support).
- Modal dialogues (SweetAlert2 / Bootstrap Modals) for destructive actions.