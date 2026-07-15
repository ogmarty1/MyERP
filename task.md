# MyERP - Sprint 5 Task List (Analytics, Reports & Dashboard)

## [x] Task 1: Dashboard (Main Page) Implementation
- [x] Create `DashboardViewModel` (or separate DTOs) to hold stats based on user roles.
- [x] Implement `DashboardService` in `ERP.BusinessLogic` to fetch data:
  - For **Employee**: Total count of orders created by the current logged-in user.
  - For **Manager / Admin**: Total Sales Revenue (decimal), Pending Orders count, and Low Stock Alerts (where `QuantityInStock <= MinimumQuantity`).
- [x] Inject service into `HomeController` in `ERP.Web`.
- [x] Update `Index.cshtml` view:
  - Use Bootstrap 5 cards for key metrics.
  - Render a clean table for "Low Stock Alerts" (only visible to Managers/Admins).
  - Hide/show UI elements dynamically using `@User.IsInRole(...)`.

## [x] Task 2: Reports Module (Sales Analytics)
- [x] Implement database queries for sales reports:
  - **Sales by Period**: Total sales revenue and order counts grouped by date/month.
  - **Sales by Category**: Revenue generated per product category.
- [x] Create `ReportsController` and corresponding views with search/filter options (e.g., date range picker).
- [x] Add navigation links to the reports in `_Layout.cshtml` (restricted to Admin and Manager roles).

## [x] Task 3: Final Polish & UI Refinement
- [x] Verify all UI tables and pages look cohesive with Bootstrap.
- [x] Ensure all input validations work properly.
- [x] Run a full end-to-end check to guarantee 0 build warnings/errors.