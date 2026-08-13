# MyERP

A modular ERP system built on .NET 8, covering products, customers, sales orders, purchase orders, inventory tracking, and reporting with role-based access control.

## Tech Stack

- **.NET 8** — Web (ASP.NET Core MVC), BusinessLogic, DataAccess layers
- **EF Core 8.0.11** with **SQL Server Express**
- **Bootstrap 5**, **Chart.js**, **DataTables** (with export to Excel/PDF/Print)
- Custom Cookie Authentication with many-to-many Role management
- Localization: Bulgarian (`bg`) and English (`en`)

## Solution Structure

| Project | Purpose |
|---|---|
| `ERP.Web` | ASP.NET Core MVC front end — controllers, views, localization resources |
| `ERP.BusinessLogic` | Services and DTOs |
| `ERP.DataAccess` | EF Core `DbContext`, entity models, configurations, migrations |
| `ERP.BusinessLogic.Tests` | Automated test project |

## Features

- **Dashboard** — role-gated KPI cards and Chart.js visualizations (monthly sales trends, top products)
- **Products** — CRUD, low-stock alerts, inventory adjustment, order history per product
- **Customers** — CRUD with filtering and details view
- **Orders** — dynamic multi-line order creation, status workflow, automatic stock adjustment
- **Purchase Orders** — supplier ordering workflow with "Mark as Received" stock inflow
- **Categories & Suppliers** — nomenclature management with related-records views
- **Users** — user and role management (Admin/Manager only)
- **Reports** — sales and purchasing reports with export support

## Authorization Matrix

| Role | Access |
|---|---|
| **Employee** | View products, create orders |
| **Manager** | Edit products/prices/stock, manage categories & suppliers, view reports |
| **Admin** | Full system access, including user/role management and financial metrics |

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server Express (or a compatible SQL Server instance)

### Setup

1. Update the connection string in `ERP.Web/appsettings.json` if needed (defaults to a local `SQLEXPRESS` instance).
2. Apply EF Core migrations:
   ```
   dotnet ef database update --project ERP.DataAccess --startup-project ERP.Web
   ```
3. Run the application:
   ```
   dotnet run --project ERP.Web
   ```

The database is automatically seeded with system roles and a primary admin account on first run.

### Running Tests

```
dotnet test ERP.BusinessLogic.Tests
```

## Currency & Localization

All financial values are stored as `decimal(18,2)` and displayed in **Euro (€)**. The UI supports Bulgarian and English via ASP.NET Core `RequestLocalization`.
