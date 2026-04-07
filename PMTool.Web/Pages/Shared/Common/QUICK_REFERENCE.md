# Common Components - Quick Reference

## 🎯 At a Glance

Use these common components across all Razor Pages to maintain consistency.

---

## 1️⃣ Back Button

**File:** `_BackButton.cshtml`

**When to use:** Details, Edit, or View pages

**Quick Copy-Paste:**
```razor
<partial name="Common/_BackButton" />
```

**With options:**
```razor
@{
    ViewData["PageRoute"] = "./Index";
    ViewData["ButtonText"] = "Go Back";
    ViewData["CssClass"] = "btn btn-secondary";
}
<partial name="Common/_BackButton" />
```

**Result:** `[← Back]` button linking to Index page

---

## 2️⃣ Cancel Button

**File:** `_CancelButton.cshtml`

**When to use:** Form pages (Create, Edit)

**Quick Copy-Paste:**
```razor
<div class="d-grid gap-2 d-md-flex justify-content-md-end">
    @{
        ViewData["PageRoute"] = "./Index";
    }
    <partial name="Common/_CancelButton" />
    <button type="submit" class="btn btn-primary btn-lg">Save</button>
</div>
```

**Result:** Two buttons side by side - Cancel and Save

---

## 3️⃣ Alert Message

**File:** `_AlertMessage.cshtml`

**When to use:** Display error, success, warning, or info messages

**Quick Copy-Paste:**
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

**Types:** `info`, `success`, `warning`, `danger`

**Result:** Styled alert box with appropriate icon

---

## 4️⃣ Action Button Group

**File:** `_ActionButtonGroup.cshtml`

**When to use:** List/Grid pages with View, Edit, Delete buttons

**Quick Copy-Paste:**
```razor
@{
    ViewData["EntityId"] = project.Id;
    ViewData["ViewPage"] = "./Details";
    ViewData["EditPage"] = "./Edit";
    ViewData["ShowView"] = true;
    ViewData["ShowEdit"] = true;
    ViewData["ShowDelete"] = true;
    ViewData["CanEdit"] = User.IsInRole("Administrator");
    ViewData["DeleteHandler"] = "Delete";
}
<partial name="Common/_ActionButtonGroup" />
```

**Result:** Button group: `[View] [Edit] [Delete]`

---

## 5️⃣ Delete Confirm Modal

**File:** `_DeleteConfirmModal.cshtml`

**When to use:** Before confirming deletion on Details page

**Quick Copy-Paste:**
```razor
@{
    ViewData["Title"] = "Confirm Delete";
    ViewData["Message"] = "Are you sure? This cannot be undone.";
    ViewData["ItemName"] = Model.Project.Name;
}
<partial name="Common/_DeleteConfirmModal" />
```

**Result:** Bootstrap modal with delete confirmation

---

## 🔗 Integration Examples

### Before (❌ Old Way - Repeated Code)

```razor
<!-- Projects/Index.cshtml -->
<a asp-page="./Details" asp-route-id="@project.Id" class="btn btn-sm btn-outline-primary">View</a>
<a asp-page="./Edit" asp-route-id="@project.Id" class="btn btn-sm btn-outline-secondary">Edit</a>

<!-- Products/Index.cshtml -->
<a asp-page="./Details" asp-route-id="@product.Id" class="btn btn-sm btn-outline-primary">View</a>
<a asp-page="./Edit" asp-route-id="@product.Id" class="btn btn-sm btn-outline-secondary">Edit</a>

<!-- SubProjects/Index.cshtml -->
<a asp-page="./Details" asp-route-id="@subProject.Id" class="btn btn-sm btn-outline-primary">View</a>
<a asp-page="./Edit" asp-route-id="@subProject.Id" class="btn btn-sm btn-outline-secondary">Edit</a>
```

### After (✅ New Way - DRY Principle)

```razor
<!-- All pages use the same component -->
@{
    ViewData["EntityId"] = item.Id;
    ViewData["ShowEdit"] = true;
    ViewData["ShowView"] = true;
    ViewData["CanEdit"] = canEdit;
}
<partial name="Common/_ActionButtonGroup" />
```

---

## 📋 Component Properties Cheat Sheet

### _BackButton
| Property | Default | Type |
|----------|---------|------|
| PageRoute | `"./Index"` | string |
| ButtonText | `"Back"` | string |
| CssClass | `"btn btn-secondary"` | string |

### _CancelButton
| Property | Default | Type |
|----------|---------|------|
| PageRoute | `"./Index"` | string |
| ButtonText | `"Cancel"` | string |
| CssClass | `"btn btn-secondary btn-lg"` | string |

### _AlertMessage
| Property | Default | Type |
|----------|---------|------|
| Message | `""` | string |
| Type | `"info"` | string |
| Dismissible | `true` | bool |

### _ActionButtonGroup
| Property | Default | Type |
|----------|---------|------|
| EntityId | *required* | string |
| ViewPage | `"./Details"` | string |
| EditPage | `"./Edit"` | string |
| ShowView | `true` | bool |
| ShowEdit | `true` | bool |
| ShowDelete | `false` | bool |
| CanEdit | `true` | bool |
| DeleteHandler | `"Delete"` | string |

### _DeleteConfirmModal
| Property | Default | Type |
|----------|---------|------|
| Title | `"Confirm Delete"` | string |
| Message | `"Are you sure you want to delete this item?"` | string |
| ItemName | `"Item"` | string |

---

## 🚀 Common Patterns

### Pattern 1: Details Page with Back Button
```razor
<div class="card-footer bg-white">
    <partial name="Common/_BackButton" />
</div>
```

### Pattern 2: Create/Edit Form with Cancel Button
```razor
<div class="d-grid gap-2 d-md-flex justify-content-md-end">
    @{
        ViewData["PageRoute"] = "./Index";
    }
    <partial name="Common/_CancelButton" />
    <button type="submit" class="btn btn-primary btn-lg">Save</button>
</div>
```

### Pattern 3: Error/Success Message Display
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

### Pattern 4: List Grid with Actions
```razor
@foreach (var item in Model.Items)
{
    <div class="card">
        <!-- Content -->
        @{
            ViewData["EntityId"] = item.Id;
            ViewData["ShowEdit"] = true;
            ViewData["ShowDelete"] = !Model.ShowArchived && canEdit;
            ViewData["CanEdit"] = canEdit;
        }
        <partial name="Common/_ActionButtonGroup" />
    </div>
}
```

### Pattern 5: Confirm Delete on Details Page
```razor
@{
    ViewData["Title"] = "Confirm Delete";
    ViewData["Message"] = "This action cannot be undone.";
    ViewData["ItemName"] = Model.Project.Name;
}
<partial name="Common/_DeleteConfirmModal" />
```

---

## 🎨 Styling & Customization

All components use **Bootstrap 5** classes.

To change button styles:
```razor
@{
    ViewData["CssClass"] = "btn btn-lg btn-outline-danger"; // Custom classes
}
<partial name="Common/_BackButton" />
```

---

## 📞 Need Help?

Refer to the full documentation:
- **README.md** - Complete usage guide
- **FOLDER_STRUCTURE.md** - Project organization

Location: `PMTool.Web/Pages/Shared/Common/`

---

## ✅ Checklist for Page Updates

When updating a page to use common components:

- [ ] Replace all `<a>` back buttons with `_BackButton`
- [ ] Replace form cancel links with `_CancelButton`
- [ ] Replace inline alerts with `_AlertMessage`
- [ ] Replace action buttons with `_ActionButtonGroup`
- [ ] Add `_DeleteConfirmModal` if delete functionality exists
- [ ] Test all navigation and buttons
- [ ] Test with different user roles
- [ ] Test on mobile/responsive views

---

**Created:** Common components folder  
**Purpose:** DRY principle, consistency, maintainability  
**Status:** Ready to use across all Razor Pages
