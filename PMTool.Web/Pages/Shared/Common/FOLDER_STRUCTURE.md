# Common Components Folder Structure

## Directory Layout

```
PMTool.Web/
│
├── Pages/
│   ├── Shared/
│   │   ├── Common/                          ← NEW FOLDER
│   │   │   ├── _BackButton.cshtml           ← Back button component
│   │   │   ├── _CancelButton.cshtml         ← Cancel button component
│   │   │   ├── _AlertMessage.cshtml         ← Alert message component
│   │   │   ├── _ActionButtonGroup.cshtml    ← Action buttons for CRUD
│   │   │   ├── _DeleteConfirmModal.cshtml   ← Delete confirmation modal
│   │   │   └── README.md                    ← This documentation
│   │   │
│   │   ├── _Layout.cshtml
│   │   ├── _ValidationScriptsPartial.cshtml
│   │   └── Error.cshtml
│   │
│   ├── Projects/
│   │   ├── Index.cshtml
│   │   ├── Create.cshtml
│   │   ├── Details.cshtml
│   │   └── Edit.cshtml
│   │
│   ├── Products/
│   │   ├── Index.cshtml
│   │   ├── Create.cshtml
│   │   ├── Details.cshtml
│   │   └── Edit.cshtml
│   │
│   └── SubProjects/
│       ├── Index.cshtml
│       ├── Create.cshtml
│       ├── Details.cshtml
│       └── Edit.cshtml
│
└── wwwroot/
    └── css/
        └── site.css
```

## What Goes in the Common Folder?

The `Pages/Shared/Common/` folder contains **reusable partial views** for common UI elements:

✅ **Include Here:**
- Navigation buttons (Back, Cancel, Next, etc.)
- Alert/notification messages
- Action buttons (View, Edit, Delete)
- Common modals (Confirmations, dialogs)
- Badges and status indicators
- Form helpers and components
- Pagination components
- Loading spinners

❌ **Do NOT Include Here:**
- Page-specific components
- Layout files (go in `Pages/Shared/`)
- CSS/JavaScript files (go in `wwwroot/`)
- Entity-specific views

## How to Use in Your Pages

### Example 1: Projects/Details.cshtml

```razor
@page "{id}"
@model PMTool.Web.Pages.Projects.DetailsModel

<div class="container-fluid mt-4">
    <!-- Page content -->
    
    <div class="card-footer bg-white">
        <!-- Use common back button -->
        <partial name="Common/_BackButton" />
    </div>
</div>
```

### Example 2: Projects/Create.cshtml

```razor
@page
@model PMTool.Web.Pages.Projects.CreateModel

<div class="card-body p-4">
    <!-- Display error if any -->
    @if (!string.IsNullOrEmpty(Model.ErrorMessage))
    {
        @{
            ViewData["Message"] = Model.ErrorMessage;
            ViewData["Type"] = "danger";
        }
        <partial name="Common/_AlertMessage" />
    }
    
    <form method="post">
        <!-- Form fields -->
        
        <div class="d-grid gap-2 d-md-flex justify-content-md-end">
            <!-- Use common cancel button -->
            @{
                ViewData["PageRoute"] = "./Index";
            }
            <partial name="Common/_CancelButton" />
            
            <button type="submit" class="btn btn-primary btn-lg">Create</button>
        </div>
    </form>
</div>
```

### Example 3: Projects/Index.cshtml (Using Action Buttons)

```razor
@page
@model PMTool.Web.Pages.Projects.IndexModel

<div class="card h-100 shadow-sm">
    <div class="card-body">
        <!-- Card content -->
    </div>
    
    <div class="card-footer bg-white">
        <!-- Use common action button group -->
        @{
            ViewData["EntityId"] = project.Id;
            ViewData["ViewPage"] = "./Details";
            ViewData["EditPage"] = "./Edit";
            ViewData["ShowView"] = true;
            ViewData["ShowEdit"] = true;
            ViewData["ShowDelete"] = true;
            ViewData["CanEdit"] = User.IsInRole("Administrator");
        }
        <partial name="Common/_ActionButtonGroup" />
    </div>
</div>
```

## Quick Reference

### Partial Names to Use in Code

```csharp
// Use these exact names in your @page code:

<partial name="Common/_BackButton" />
<partial name="Common/_CancelButton" />
<partial name="Common/_AlertMessage" />
<partial name="Common/_ActionButtonGroup" />
<partial name="Common/_DeleteConfirmModal" />
```

### Setting ViewData

```csharp
// Pattern to follow when setting component configuration:

@{
    ViewData["PropertyName"] = value;
}
<partial name="Common/_ComponentName" />
```

## File Purposes

| File | Purpose | Used In |
|------|---------|---------|
| `_BackButton.cshtml` | Navigate back to previous page | Details, Edit pages |
| `_CancelButton.cshtml` | Cancel form submission | Create, Edit form pages |
| `_AlertMessage.cshtml` | Display error/success messages | All pages with messages |
| `_ActionButtonGroup.cshtml` | Show View/Edit/Delete buttons | List/Grid pages |
| `_DeleteConfirmModal.cshtml` | Confirm before deleting | Details pages |
| `README.md` | Usage documentation | Reference |

## Benefits of This Approach

✅ **DRY Principle** - Don't repeat the same HTML code
✅ **Consistency** - Same styling everywhere
✅ **Maintainability** - Update once, applies everywhere
✅ **Scalability** - Easy to add new features
✅ **Reusability** - Use across all Razor Pages
✅ **Testing** - Test component once
✅ **Performance** - Compiled once, used many times

## Next Steps

1. ✅ Common folder created with reusable components
2. ⏳ Update existing pages to use common components
3. ⏳ Test all pages to ensure components work correctly
4. ⏳ Add more common components as needed
