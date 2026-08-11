# MyERP - Tasks & Implementation Roadmap

## Architectural Context & Rules (Strictly Follow)
- **Tech Stack**: .NET 8, EF Core 8.0.11, SQL Server Express, Bootstrap 5, Chart.js, DataTables.
- **Data Integrity**: Financials strictly `decimal(18,2)`. Quantities strictly `int`.
- **Delete Behavior**: `DeleteBehavior.Restrict` on primary FKs (catch `DbUpdateException` in controllers for Toastr errors).
- **Globalization**: Euro (`€` / `EUR`) as primary currency. Support multi-language (`bg`/`en`).
- **Authorization**: Custom Cookie Auth with `UserRoles`. Audit fields (`CreatedBy`, `CreatedDate`) strictly use the logged-in user identity.

---

## Task List for Implementation

### 1. Header & Navigation Fixes
- [x] **1.2 Branding**: Update navigation header in `_Layout.cshtml` from `ERP.Web` to **MyERP**.

---

### 2. Dashboard Integration (Home Page)
- [x] **1.3 Dashboard Interactivity**:
  - [x] Link "Monthly Revenue" card to Open Reports scoped to the current month.
  - [x] Link "New Orders" card to Orders list pre-filtered for current month.
  - [x] Link "Active Customers" card to Customer Index.
  - [x] Link "Low Stock Alerts" card to Product Index pre-filtered by low stock status.

---

### 3. Products Module Enhancements
- [x] **Data Model & Migrations**:
  - [x] Add `SKU` (string, unique), `Status` (enum/string: Active, Discontinued, OutOfStock), and `Notes` (string, nullable) to `Product` entity.
  - [x] Generate and apply EF Core migration.
- [x] **Product Index Grid**:
  - [x] Column order: Checkbox | SKU (clickable) | Name | Category | Supplier | Sale Price | Stock Qty | Status | Actions.
  - [x] Add multi-select checkboxes for export/print actions (remove Copy option from DataTables; limit Excel/PDF/Print to selected rows if checked).
  - [x] Replace grid action buttons: Remove Edit/Delete from main row; keep single "Adjust Inventory" quick-action button.
  - [x] Filters panel: Keyword, Category dropdown, Supplier dropdown, Price range (Min/Max), Stock range, Status filter.
- [x] **Inventory Adjustment Modal**:
  - [x] Modal dialogue for quick stock adjustment (Quantity change, Reason, Type: Inflow/Outflow/Adjustment) updating `QuantityInStock` and logging to `InventoryMovements`.
- [x] **Product Details & In-Place Edit (View Page)**:
  - [x] Clickable SKU navigates to Product Details (`/Products/Details/{id}`).
  - [x] UI Style: Implement colorful field containers (custom fieldsets/floating borders with field titles overlaid on top borders).
  - [x] Toggle Edit Mode: Display view-only mode by default. "Edit" button toggles inputs to editable mode, revealing "Save" and "Cancel" buttons. Include Delete button inside the view page with confirmation modal.
  - [x] Order History Tab/Table: Paginated DataTables listing past orders containing this product (Columns: OrderID [clickable], Order Date, Total Amount, Status).

---

### 4. Customers Module Enhancements
- [x] **Data Model & Migrations**:
  - [x] Add `Notes` (string, nullable) and `Company` (string, nullable) to `Customer` entity. Remove `Status` if applicable.
  - [x] Apply EF Core migration.
- [x] **Customer Index & Filtering**:
  - [x] Filters: Name, Company, Phone, Email, Address.
  - [x] DataTables export/print with checkbox selection support.
- [x] **Customer Details & Actions**:
  - [x] Clickable Customer row/Name opens Details view (`/Customers/Details/{id}`).
  - [x] Apply colorful field styling (floating borders with top labels).
  - [x] In-place Edit toggle (Edit button enables input fields; reveals Save/Cancel). Delete button inside details view with `DbUpdateException` handling.

---

### 5. Orders Module Overhaul
- [x] **Data Model & Migrations**:
  - [x] Add `Status` (Pending, Shipped, Completed, On Hold), `ShippedDate` (DateTime?), `Notes` (string?), and `CreatedByUserId` (FK to User) to `Order` entity.
  - [x] Apply EF Core migration.
- [x] **Create Order Enhancements**:
  - [x] Automatically set `CreatedBy` to logged-in user (remove dropdown selection).
  - [x] Add inline "Create New Customer" modal without leaving order form.
  - [x] Apply colorful field styling to order creation form.
- [x] **Order Grid & Filtering**:
  - [x] Filters: OrderID, Customer, OrderDate range, Total Amount range, Status.
  - [x] Export/Print actions with multi-select checkboxes (remove Copy option).
- [x] **Order Details View (`/Orders/Details/{id}`)**:
  - [x] Clickable OrderID opens Details.
  - [x] Top Action Bar: Status change actions (Pending, Shipped, Completed, On Hold) with instant DB update.
  - [x] Display Sections (Colorful floating borders):
    - [x] Customer Info Card (Name, Company, Shipping Address).
    - [x] Order Meta Card (OrderID, Status, Order Date, Shipped Date, Created By).
    - [x] Products Table (SKU, Name, Unit Price, Quantity, Subtotal).
    - [x] Financial Summary Card (Total Amount).
    - [x] Notes Section.
  - [x] Toggle Edit Mode: Allow adding/removing items or editing quantities.
  - [x] Inventory Delta Handling: Adjusting order quantities in edit mode automatically adds/subtracts delta to/from `QuantityInStock` and logs to `InventoryMovements`.

---

### 6. User Management Module (New)
- [x] **Page Creation (`/Users`)**:
  - [x] Restrict access to `Admin` / `Manager` roles via `[Authorize(Roles = "Admin,Manager")]`.
  - [x] Index Grid: User ID, Full Name (First + Last Name), Email, Role, Contact Details, Actions.
- [x] **Authentication Update**:
  - [x] Enforce Login strictly via **Email** and Password. Ensure Unique Index on `Email`.
- [x] **Create & Edit User Details**:
  - [x] Form fields: First Name, Last Name, Email, Password, Assigned Role, Phone/Contact details. Auto-generated System ID.
  - [x] Apply colorful field styling on User Details and Create pages.

---

### 7. Purchase Orders (PO) Module (New Module)
- [x] **Data Model & Database Setup**:
  - [x] Create `PurchaseOrder` entity: `Id`, `SupplierId`, `OrderDate`, `TotalAmount`, `Status` (Ordered, Received), `Notes`, `CreatedByUserId`.
  - [x] Create `PurchaseOrderDetail` entity: `Id`, `PurchaseOrderId`, `ProductId`, `Quantity`, `UnitPrice`.
  - [x] Register DbSets in `ERP.DataAccess` and run EF Core migration.
- [x] **Purchase Orders Index Grid**:
  - [x] Columns: PO ID (clickable), Supplier, Order Date, Total Amount, Status, Actions.
  - [x] Export/Print with multi-select checkboxes.
- [x] **Create Purchase Order**:
  - [x] Form for selecting Supplier, adding Product line items + Quantities + Purchase Prices, Notes.
  - [x] `CreatedBy` automatically assigned to logged-in user.
  - [x] Apply colorful field styling.
- [x] **PO Details & Workflow (`/PurchaseOrders/Details/{id}`)**:
  - [x] Display PO Meta, Supplier details, Ordered Products table, Total Amount.
  - [x] Toggle Edit Mode: Products and quantities editable while status is `Ordered`.
  - [x] "Mark as Received" Action:
    - [x] Transactional update: Changes status to `Received`, adds quantities to Product `QuantityInStock`, creates `Inflow` log in `InventoryMovements`.
    - [x] Lock editing once status is `Received`.

---

### 8. Categories & Suppliers Enhancements
- [x] **Categories**:
  - [x] Clickable Category opens Details view.
  - [x] Details view contains table of assigned Products (SKU, Name, Price, Stock).
  - [x] Edit toggle and Delete actions inside Details view.
- [x] **Suppliers**:
  - [x] Filters: Name, Company, Contact Person, Phone, Email.
  - [x] Supplier Details view with colorful fields UI.
  - [x] Table of related Purchase Orders inside Supplier details (clickable PO IDs).
  - [x] Edit toggle and Delete actions inside Details view.

---

### 9. Reports Page UI Modernization
- [x] **Visual Polish**: Add color-coded charts, vibrant Bootstrap badge cards, and enhanced CSS styling.
- [x] **Export Capabilities**: Integrate DataTables Export extensions (Excel, PDF, Print) on report results.

---

## Suggestions & Enhancements Incorporated
- **Inventory Delta Logic**: Automatically handles stock adjustments when existing Orders or POs are modified.
- **Stock Lock on Received POs**: Prevents double-stocking by locking PO edits after marked as Received.
- **Unified UI Component**: Colorful field borders implemented via standard reusable CSS class `.erp-field-group` with floating label overlay.