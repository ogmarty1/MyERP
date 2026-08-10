# TASK: UI/UX Polish - Stage 3: SweetAlert2 Modals, Toastr Notifications & UX Polish

## Context
Continuing UI/UX polish for MyERP. Following the successful completion of Stage 2 (DataTables & Export), Stage 3 focuses on replacing native browser dialogs (`alert()`, `confirm()`) with modern, enterprise-grade UX components (SweetAlert2 and Toastr notifications) across all modules.

## Tasks to Complete

### 1. External Libraries Integration
- [x] Add **SweetAlert2** CSS/JS (via CDN) into `_Layout.cshtml`.
- [x] Add **Toastr** CSS/JS (via CDN) into `_Layout.cshtml`.

### 2. Toastr Notifications Setup (TempData & Feedback)
- [x] Implement a global script/partial view (`_ValidationScriptsPartial` or layout script) that automatically intercepts ASP.NET Core `TempData["Success"]` and `TempData["Error"]` messages and displays them via **Toastr**. (Kept the app's existing `TempData["SuccessMessage"]` key rather than renaming to `"Success"`, and added a matching `"ErrorMessage"` key for symmetry.)
- [x] Update Controllers (Products, Customers, Suppliers, Orders, Account) to set `TempData["Success"]` or `TempData["Error"]` upon successful actions or caught exceptions (`DbUpdateException`). Products/Orders/Account had no failure path needing a new error toast (Account's invalid-login case stays inline, matching standard login UX); Customers/Categories/Suppliers `DbUpdateException` handling now redirects with `TempData["ErrorMessage"]` instead of re-rendering an inline validation summary.

### 3. SweetAlert2 Confirmation Modals
- [x] Replace standard `confirm()` browser dialogs with **SweetAlert2** confirmation popups for deleting records (Customers, Categories, Suppliers). Products has no delete feature in this codebase (no service/controller/route) and Orders has no cancel/status-change feature, so neither had a `confirm()` to replace — noted rather than inventing new CRUD endpoints.
- [x] Ensure delete actions trigger a warning modal ("Are you sure you want to delete this item? This action cannot be undone.") before submitting the form.

### 4. Global UX & Form Enhancements
- [x] Add loading indicators / spinners (disabled button + spinner icon) on submit buttons during form posts (Create/Edit) to prevent duplicate submissions.
- [x] Ensure proper styling alignment with existing Bootstrap 5 layout.

### 5. Verification & Build Check
- [x] Run `dotnet build` to verify zero compilation errors or warnings.
- [x] Check that SweetAlert2 popups and Toastr notifications respect multi-language settings (BG/EN) or display localized text cleanly.