# Common Components Documentation

This folder contains **reusable partial views** for common UI elements used across the PMTool application. These components follow DRY (Don't Repeat Yourself) principles and maintain consistent styling throughout the app.

## Components Overview

### 1. `_BackButton.cshtml`
Reusable back button component with customizable styling and text.

**Usage:**
```razor
@* Simple usage *@
<partial name="Common/_BackButton" />

@* With custom page route *@
@{
    ViewData["PageRoute"] = "./Index";
    ViewData["ButtonText"] = "Go Back";
    ViewData["CssClass"] = "btn btn-secondary btn-lg";
}
<partial name="Common/_BackButton" />
```

**Available ViewData Properties:**
- `PageRoute` (default: `"./Index"`) - The Razor page to navigate to
- `ButtonText` (default: `"Back"`) - Button display text
- `CssClass` (default: `"btn btn-secondary"`) - Bootstrap CSS classes

**Example in Context:**
```razor
<!-- In Details.cshtml -->
<div class="card-footer">
    <partial name="Common/_BackButton" />
</div>
```

---

### 2. `_CancelButton.cshtml`
Reusable cancel button for forms, typically used alongside submit buttons.

**Usage:**
```razor
@* In a form footer *@
<div class="d-grid gap-2 d-md-flex justify-content-md-end">
    @{
        ViewData["PageRoute"] = "./Index";
        ViewData["ButtonText"] = "Cancel";
    }
    <partial name="Common/_CancelButton" />
    <button type="submit" class="btn btn-primary btn-lg">Save</button>
</div>
```

**Available ViewData Properties:**
- `PageRoute` (default: `"./Index"`) - Page to navigate on cancel
- `ButtonText` (default: `"Cancel"`) - Button display text
- `CssClass` (default: `"btn btn-secondary btn-lg"`) - Bootstrap CSS classes

---

### 3. `_AlertMessage.cshtml`
Reusable alert component for displaying messages (success, warning, danger, info).

**Usage:**
```razor
@* Success message *@
@{
    ViewData["Message"] = "Project created successfully!";
    ViewData["Type"] = "success";
    ViewData["Dismissible"] = true;
}
<partial name="Common/_AlertMessage" />

@* Error message *@
@{
    ViewData["Message"] = "Failed to update project. Please try again.";
    ViewData["Type"] = "danger";
}
<partial name="Common/_AlertMessage" />
```

**Available ViewData Properties:**
- `Message` (default: `""`) - The alert message to display (supports HTML)
- `Type` (default: `"info"`) - Alert type: `info`, `success`, `warning`, or `danger`
- `Dismissible` (default: `true`) - Show close button

**Icon Mapping:**
- `info` → Info circle icon
- `success` → Check circle icon
- `warning` → Exclamation circle icon
- `danger` → Exclamation triangle icon

**Example in Create Form:**
```razor
@if (!string.IsNullOrEmpty(Model.ErrorMessage))
{
    @{
        ViewData["Message"] = Model.ErrorMessage;
        ViewData["Type"] = "danger";
    }
    <partial name="Common/_AlertMessage" />
}
```

---

### 4. `_ActionButtonGroup.cshtml`
Reusable button group for CRUD operations (View, Edit, Delete) with permission-based visibility.

**Usage:**
```razor
@* Show all action buttons *@
@{
    ViewData["EntityId"] = Model.Project.Id;
    ViewData["ViewPage"] = "./Details";
    ViewData["EditPage"] = "./Edit";
    ViewData["ShowView"] = true;
    ViewData["ShowEdit"] = true;
    ViewData["ShowDelete"] = true;
    ViewData["CanEdit"] = User.IsInRole("Administrator") || User.IsInRole("Project Manager");
    ViewData["DeleteHandler"] = "Delete";
}
<partial name="Common/_ActionButtonGroup" />
```

**Available ViewData Properties:**
- `EntityId` - The ID of the entity (required for routing)
- `ViewPage` (default: `"./Details"`) - Route to details page
- `EditPage` (default: `"./Edit"`) - Route to edit page
- `ShowView` (default: `true`) - Show view button
- `ShowEdit` (default: `true`) - Show edit button
- `ShowDelete` (default: `false`) - Show delete button
- `CanEdit` (default: `true`) - Permission to edit/delete
- `DeleteHandler` (default: `"Delete"`) - Page handler method name

**Example in Index Grid:**
```razor
@foreach (var project in Model.Projects)
{
    <div class="card">
        @{
            ViewData["EntityId"] = project.Id;
            ViewData["CanEdit"] = canEditProject;
            ViewData["ShowDelete"] = true;
        }
        <partial name="Common/_ActionButtonGroup" />
    </div>
}
```

---

### 5. `_DeleteConfirmModal.cshtml`
Reusable modal dialog for delete confirmation.

**Usage:**
```razor
@* In your page *@
@{
    ViewData["Title"] = "Confirm Delete";
    ViewData["Message"] = "Are you sure you want to delete this project? This action cannot be undone.";
    ViewData["ItemName"] = Model.Project.Name;
}
<partial name="Common/_DeleteConfirmModal" />

@* In your page model (OnPostDelete handler) *@
public async Task<IActionResult> OnPostDeleteAsync(Guid id)
{
    // Delete logic here
    return RedirectToPage("./Index");
}
```

**Available ViewData Properties:**
- `Title` (default: `"Confirm Delete"`) - Modal title
- `Message` (default: `"Are you sure you want to delete this item?"`) - Confirmation message
- `ItemName` (default: `"Item"`) - Name of item being deleted

---

## Best Practices

### 1. Consistent Navigation
Always use these common components for back buttons:
```razor
<!-- ✅ Good -->
<partial name="Common/_BackButton" />

<!-- ❌ Avoid -->
<a href="/Projects" class="btn btn-secondary">Back</a>
```

### 2. Error/Success Messages
Use `_AlertMessage` component:
```razor
<!-- ✅ Good -->
@{
    ViewData["Message"] = Model.ErrorMessage;
    ViewData["Type"] = "danger";
}
<partial name="Common/_AlertMessage" />

<!-- ❌ Avoid -->
<div class="alert alert-danger">
    <i class="bi bi-exclamation-circle"></i> @Model.ErrorMessage
</div>
```

### 3. Form Buttons
Use `_CancelButton` for consistent form layouts:
```razor
<!-- ✅ Good -->
<div class="d-grid gap-2 d-md-flex justify-content-md-end">
    @{
        ViewData["PageRoute"] = "./Index";
    }
    <partial name="Common/_CancelButton" />
    <button type="submit" class="btn btn-primary btn-lg">Save</button>
</div>
```

### 4. CRUD Operations
Use `_ActionButtonGroup` for consistent action buttons:
```razor
<!-- ✅ Good -->
@{
    ViewData["EntityId"] = project.Id;
    ViewData["CanEdit"] = canEditProject;
    ViewData["ShowDelete"] = true;
}
<partial name="Common/_ActionButtonGroup" />
```

---

## Component Usage Summary Table

| Component | Purpose | Location |
|-----------|---------|----------|
| `_BackButton` | Simple back navigation | Card footers, Details pages |
| `_CancelButton` | Form cancellation | Form buttons |
| `_AlertMessage` | Display notifications | Form pages, alerts |
| `_ActionButtonGroup` | CRUD operations | List/Grid pages |
| `_DeleteConfirmModal` | Confirm deletion | Details pages |

---

## Styling & Customization

All components use **Bootstrap 5** classes and **Bootstrap Icons** (`bi`).

To customize styling, pass CSS classes via `ViewData`:

```razor
@{
    ViewData["CssClass"] = "btn btn-lg btn-outline-danger";
}
<partial name="Common/_BackButton" />
```

---

## Migration Guide

To update existing pages to use common components:

### Before (Old Pattern)
```razor
<!-- In Create.cshtml -->
<a asp-page="./Index" class="btn btn-secondary btn-lg">Cancel</a>
<button type="submit" class="btn btn-primary btn-lg">Create</button>

<!-- In Details.cshtml -->
<a asp-page="./Index" class="btn btn-secondary"><i class="bi bi-arrow-left"></i> Back</a>
```

### After (Using Common Components)
```razor
<!-- In Create.cshtml -->
@{
    ViewData["PageRoute"] = "./Index";
}
<partial name="Common/_CancelButton" />
<button type="submit" class="btn btn-primary btn-lg">Create</button>

<!-- In Details.cshtml -->
<partial name="Common/_BackButton" />
```

---

## Future Enhancements

Consider adding these components in the future:
- `_PageHeader` - Common page header with title and breadcrumbs
- `_FormGroup` - Reusable form field wrapper
- `_LoadingSpinner` - Loading indicator
- `_Pagination` - Common pagination controls
- `_StatusBadge` - Status badges with standardized colors
- `_ConfirmationDialog` - Generic confirmation dialog

---

## Questions or Issues?

If you encounter any issues with these components, refer to the Razor Pages documentation:
- [Partial Views in Razor Pages](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/partial)
- [ASP.NET Core Partial Tags](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/built-in/partial-tag-helper)
