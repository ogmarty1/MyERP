# TASK: UI/UX Polish - Stage 2: Table UX Enhancements & Export Capabilities

## Context
Continuing UI/UX polish for MyERP. Now that Euro (€) formatting and multi-language support (BG/EN) are configured, we need to standardize and enhance all entity data tables (Products, Customers, Suppliers, Orders) with client-side interactive capabilities: fast instant filtering, seamless pagination, status badges, and data export features.

## Tasks to Complete

### 1. DataTables Integration & Localization
- [x] Add DataTables CSS/JS (via CDN) and its Bootstrap 5 theme integration into `_Layout.cshtml` or specific views.
- [x] Initialize DataTables on the main list views:
  - `Products/Index.cshtml`
  - `Customers/Index.cshtml`
  - `Suppliers/Index.cshtml`
  - `Orders/Index.cshtml`
- [x] Configure DataTables to dynamically respect the active UI culture (BG / EN) for UI labels (search input, zero records, pagination).

### 2. Table Visual & UX Improvements
- [x] Ensure all tables maintain Bootstrap 5 styling (`table-hover`, `table-striped`, `align-middle`).
- [x] Preserve existing critical stock highlighting (`table-danger` for low inventory).
- [x] Enhance status indicators in `Orders/Index.cshtml` using Bootstrap Badges (e.g., Completed -> `bg-success`, Pending -> `bg-warning`, Cancelled -> `bg-danger`).
- [x] Add compact action button groups (Edit, Details, Delete) with clear Bootstrap Icons for better mobile responsiveness.

### 3. Data Export Functionality
- [x] Enable DataTables Buttons extension (Copy, Excel, Print/PDF) for key administrative tables (Products and Orders).
- [x] Ensure exported Excel/Print files properly display Euro formatting (€) and correct UTF-8 character encoding.

### 4. Verification & Build Check
- [x] Run `dotnet build` to ensure zero compilation errors or warnings.
- [x] Verify that live search, pagination, and dynamic language switching work smoothly together without breaking server-side model data.