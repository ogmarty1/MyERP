# TASK: UI/UX Polish - Stage 1: Dashboard, KPI Cards & Chart.js Integration

## Context
Finalizing the UI/UX for MyERP. The database (EF Core 8) and business logic are working End-to-End[cite: 1]. We need to enhance the visual representation of the main Dashboard using data visualization and modern info-cards, strictly following the architectural rules in `CLAUDE.md`.

## Tasks to Complete

### 1. Backend Preparation (Controller & ViewModel)
- [x] Update or create the necessary ViewModels in `ERP.BusinessLogic` / `ERP.Web` for Dashboard data (e.g., `DashboardViewModel`).
- [x] Implement logic in `HomeController` (or `DashboardController`) to fetch:
  - **KPIs**: Total monthly revenue, Number of new monthly orders, Total active customers, Number of products with critical inventory levels.
  - **Charts**: Monthly sales data (Sales Trends - last 12 months) and Top 5 best-selling categories/products.
- [x] Apply role-based visibility: Financial KPI cards and revenue charts should only be accessible/visible to `Admin` and `Manager` roles (`@User.IsInRole`).

### 2. Frontend & Visual Polish (Razor View)
- [x] Include Chart.js (via CDN) in `_Layout.cshtml` or locally in the Dashboard view.
- [x] Implement **4 Bootstrap 5 KPI cards** at the top of the Dashboard using icons (Bootstrap Icons / FontAwesome) and clean card styling (`shadow-sm`, hover effects).
- [x] Add card containers containing Canvas elements for the two charts:
  - `Bar` or `Line Chart`: "Monthly Sales Trends (Last 12 Months)".
  - `Doughnut` or `Pie Chart`: "Top 5 Best-Selling Categories/Products".
- [x] Ensure proper currency formatting for all monetary values (`decimal` formatted in BGN/lv.).

### 3. Verification & Build Check
- [x] Ensure there are zero compilation errors or warnings (`dotnet build`).
- [x] Verify that the Dashboard adapts correctly based on different logged-in user roles.