# Common Components - Implementation Examples

## Real-World Usage Examples

This document shows actual code examples of how to use common components in your Razor Pages.

---

## 📋 Example 1: Projects/Index.cshtml (Updated with Common Components)

### Before (Old Code)

```razor
@page
@model PMTool.Web.Pages.Projects.IndexModel

<div class="row g-4">
    @foreach (var project in Model.Projects)
    {
        <div class="col-md-6 col-lg-4">
            <div class="card h-100 shadow-sm">
                <div class="card-body">
                    <h5 class="card-title">@project.Name</h5>
                    <p class="card-text">@project.ClientName</p>
                </div>
                <div class="card-footer bg-white">
                    <div class="btn-group w-100" role="group">
                        <a asp-page="./Details" asp-route-id="@project.Id" class="btn btn-sm btn-outline-primary">
                            <i class="bi bi-eye"></i> View
                        </a>
                        @if (canEditProject)
                        {
                            <a asp-page="./Edit" asp-route-id="@project.Id" class="btn btn-sm btn-outline-secondary">
                                <i class="bi bi-pencil"></i> Edit
                            </a>
                        }
                    </div>
                </div>
            </div>
        </div>
    }
</div>
```

### After (Using Common Components)

```razor
@page
@model PMTool.Web.Pages.Projects.IndexModel

<div class="row g-4">
    @foreach (var project in Model.Projects)
    {
        <div class="col-md-6 col-lg-4">
            <div class="card h-100 shadow-sm">
                <div class="card-body">
                    <h5 class="card-title">@project.Name</h5>
                    <p class="card-text">@project.ClientName</p>
                </div>
                <div class="card-footer bg-white">
                    @{
                        ViewData["EntityId"] = project.Id;
                        ViewData["ViewPage"] = "./Details";
                        ViewData["EditPage"] = "./Edit";
                        ViewData["ShowView"] = true;
                        ViewData["ShowEdit"] = true;
                        ViewData["CanEdit"] = canEditProject;
                    }
                    <partial name="Common/_ActionButtonGroup" />
                </div>
            </div>
        </div>
    }
</div>
```

**Benefits:**
- Cleaner code
- Consistent with other pages
- Easy to maintain
- Add/remove buttons without changing page code

---

## 🎯 Example 2: Projects/Create.cshtml (Updated)

### Before

```razor
@page
@model PMTool.Web.Pages.Projects.CreateModel

<div class="card shadow">
    <div class="card-header bg-primary text-white">
        <h4 class="mb-0">Create New Project</h4>
    </div>
    <div class="card-body p-4">
        @if (!string.IsNullOrEmpty(Model.ErrorMessage))
        {
            <div class="alert alert-danger alert-dismissible fade show" role="alert">
                <i class="bi bi-exclamation-circle"></i> @Model.ErrorMessage
                <button type="button" class="btn-close" data-bs-dismiss="alert" 
                        aria-label="Close"></button>
            </div>
        }

        <form method="post">
            <!-- Form fields -->
            <div class="mb-3">
                <label asp-for="Input.Name" class="form-label">Project Name</label>
                <input asp-for="Input.Name" class="form-control form-control-lg" />
            </div>

            <!-- More form fields... -->

            <div class="d-grid gap-2 d-md-flex justify-content-md-end">
                <a asp-page="./Index" class="btn btn-secondary btn-lg">Cancel</a>
                <button type="submit" class="btn btn-primary btn-lg">Create Project</button>
            </div>
        </form>
    </div>
</div>
```

### After

```razor
@page
@model PMTool.Web.Pages.Projects.CreateModel

<div class="card shadow">
    <div class="card-header bg-primary text-white">
        <h4 class="mb-0">Create New Project</h4>
    </div>
    <div class="card-body p-4">
        <!-- Use common alert component -->
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
            <div class="mb-3">
                <label asp-for="Input.Name" class="form-label">Project Name</label>
                <input asp-for="Input.Name" class="form-control form-control-lg" />
            </div>

            <!-- More form fields... -->

            <!-- Use common cancel button -->
            <div class="d-grid gap-2 d-md-flex justify-content-md-end">
                @{
                    ViewData["PageRoute"] = "./Index";
                    ViewData["ButtonText"] = "Cancel";
                }
                <partial name="Common/_CancelButton" />
                <button type="submit" class="btn btn-primary btn-lg">Create Project</button>
            </div>
        </form>
    </div>
</div>
```

**Changes:**
- Line 13-17: Replaced inline alert with `_AlertMessage` component
- Line 36-39: Replaced cancel link with `_CancelButton` component

---

## 📄 Example 3: Projects/Details.cshtml (Updated)

### Before

```razor
@page "{id}"
@model PMTool.Web.Pages.Projects.DetailsModel

<div class="container-fluid mt-4">
    <div class="row">
        <div class="col-md-8">
            <div class="card">
                <div class="card-body">
                    <h1>@Model.Project.Name</h1>
                    <p>@Model.Project.ClientName</p>
                    <p>@Model.Project.Description</p>
                </div>
                <div class="card-footer">
                    <a asp-page="./Index" class="btn btn-secondary">
                        <i class="bi bi-arrow-left"></i> Back
                    </a>
                </div>
            </div>
        </div>
    </div>
</div>
```

### After

```razor
@page "{id}"
@model PMTool.Web.Pages.Projects.DetailsModel

<div class="container-fluid mt-4">
    <div class="row">
        <div class="col-md-8">
            <div class="card">
                <div class="card-body">
                    <h1>@Model.Project.Name</h1>
                    <p>@Model.Project.ClientName</p>
                    <p>@Model.Project.Description</p>
                </div>
                <div class="card-footer">
                    <!-- Use common back button -->
                    <partial name="Common/_BackButton" />
                </div>
            </div>
        </div>
    </div>
</div>
```

**Change:**
- Line 16: Replaced inline back link with `_BackButton` component

---

## 🗑️ Example 4: With Delete Confirmation

### Projects/Details.cshtml with Delete Modal

```razor
@page "{id}"
@model PMTool.Web.Pages.Projects.DetailsModel

<div class="container-fluid mt-4">
    <div class="row">
        <div class="col-md-8">
            <div class="card">
                <div class="card-header">
                    <h3>@Model.Project.Name</h3>
                </div>
                <div class="card-body">
                    <!-- Error messages -->
                    @if (!string.IsNullOrEmpty(Model.ErrorMessage))
                    {
                        @{
                            ViewData["Message"] = Model.ErrorMessage;
                            ViewData["Type"] = "danger";
                        }
                        <partial name="Common/_AlertMessage" />
                    }

                    <!-- Success messages -->
                    @if (!string.IsNullOrEmpty(Model.SuccessMessage))
                    {
                        @{
                            ViewData["Message"] = Model.SuccessMessage;
                            ViewData["Type"] = "success";
                        }
                        <partial name="Common/_AlertMessage" />
                    }

                    <!-- Project Details -->
                    <div class="row">
                        <div class="col-md-6">
                            <p><strong>Client:</strong> @Model.Project.ClientName</p>
                            <p><strong>Status:</strong> @Model.Project.Status</p>
                        </div>
                        <div class="col-md-6">
                            <p><strong>Start:</strong> @Model.Project.StartDate.ToShortDateString()</p>
                            <p><strong>End:</strong> @Model.Project.ExpectedEndDate.ToShortDateString()</p>
                        </div>
                    </div>

                    <!-- Actions -->
                    <div class="mt-4">
                        <a asp-page="./Edit" asp-route-id="@Model.Project.Id" class="btn btn-primary">
                            <i class="bi bi-pencil"></i> Edit
                        </a>
                        <button type="button" class="btn btn-danger" data-bs-toggle="modal" 
                                data-bs-target="#deleteConfirmModal">
                            <i class="bi bi-trash"></i> Delete
                        </button>
                    </div>
                </div>
                <div class="card-footer">
                    <!-- Use common back button -->
                    <partial name="Common/_BackButton" />
                </div>
            </div>
        </div>
    </div>

    <!-- Delete Confirmation Modal -->
    @{
        ViewData["Title"] = "Confirm Delete";
        ViewData["Message"] = "Are you sure you want to delete this project? This action cannot be undone.";
        ViewData["ItemName"] = Model.Project.Name;
    }
    <partial name="Common/_DeleteConfirmModal" />
</div>

@section Scripts {
    <script>
        document.getElementById('confirmDeleteBtn')?.addEventListener('click', function() {
            const form = document.createElement('form');
            form.method = 'post';
            form.action = '@Url.Page("./Details", new { handler = "Delete", id = Model.Project.Id })';
            document.body.appendChild(form);
            form.submit();
        });
    </script>
}
```

---

## 🔗 Example 5: Full Page with All Components

### Products/Index.cshtml (Complete Example)

```razor
@page "{projectId:guid}"
@model PMTool.Web.Pages.Products.IndexModel

@{
    ViewData["Title"] = "Products";
    var canEditProduct = User.IsInRole("Administrator") || User.IsInRole("Project Manager");
}

<div class="container-fluid mt-4">
    <!-- Page Header -->
    <div class="row mb-4">
        <div class="col-12">
            <div class="d-flex justify-content-between align-items-center">
                <h1>Products</h1>
                @if (canEditProduct)
                {
                    <a asp-page="./Create" asp-route-projectId="@Model.ProjectId" class="btn btn-primary">
                        <i class="bi bi-plus-circle"></i> New Product
                    </a>
                }
            </div>
        </div>
    </div>

    <!-- Alert Messages -->
    @if (!string.IsNullOrEmpty(Model.SuccessMessage))
    {
        @{
            ViewData["Message"] = Model.SuccessMessage;
            ViewData["Type"] = "success";
            ViewData["Dismissible"] = true;
        }
        <partial name="Common/_AlertMessage" />
    }

    @if (!string.IsNullOrEmpty(Model.ErrorMessage))
    {
        @{
            ViewData["Message"] = Model.ErrorMessage;
            ViewData["Type"] = "danger";
            ViewData["Dismissible"] = true;
        }
        <partial name="Common/_AlertMessage" />
    }

    <!-- Products Grid -->
    @if (Model.Products.Any())
    {
        <div class="row g-4">
            @foreach (var product in Model.Products)
            {
                <div class="col-md-6 col-lg-4">
                    <div class="card h-100 shadow-sm">
                        <div class="card-header">
                            <h5 class="card-title mb-0">@product.Name</h5>
                            <small class="text-muted">Code: @product.ProductCode</small>
                        </div>
                        <div class="card-body">
                            <p class="card-text">@product.Description</p>
                            <div class="small text-muted">
                                <p><strong>Version:</strong> @product.Version</p>
                                <p><strong>Status:</strong> 
                                    <span class="badge bg-info">@product.Status</span>
                                </p>
                            </div>
                        </div>
                        <div class="card-footer bg-white">
                            @{
                                ViewData["EntityId"] = product.Id;
                                ViewData["ViewPage"] = "./Details";
                                ViewData["EditPage"] = "./Edit";
                                ViewData["ShowView"] = true;
                                ViewData["ShowEdit"] = true;
                                ViewData["ShowDelete"] = false;
                                ViewData["CanEdit"] = canEditProduct;
                            }
                            <partial name="Common/_ActionButtonGroup" />
                        </div>
                    </div>
                </div>
            }
        </div>
    }
    else
    {
        @{
            ViewData["Message"] = "No products found for this project.";
            ViewData["Type"] = "info";
        }
        <partial name="Common/_AlertMessage" />
    }
</div>
```

---

## 📝 Example 6: Edit Page with Validation

### SubProjects/Edit.cshtml (Complete Example)

```razor
@page "{id:guid}/{productId:guid}"
@model PMTool.Web.Pages.SubProjects.EditModel

@{
    ViewData["Title"] = "Edit Sub-Project";
}

<div class="container-fluid mt-4">
    <div class="row justify-content-center">
        <div class="col-md-8">
            <div class="card shadow">
                <div class="card-header bg-primary text-white">
                    <h4 class="mb-0">Edit Sub-Project</h4>
                </div>
                <div class="card-body p-4">
                    <!-- Error Alert -->
                    @if (!string.IsNullOrEmpty(Model.ErrorMessage))
                    {
                        @{
                            ViewData["Message"] = Model.ErrorMessage;
                            ViewData["Type"] = "danger";
                        }
                        <partial name="Common/_AlertMessage" />
                    }

                    <!-- Validation Summary -->
                    @if (!ViewData.ModelState.IsValid)
                    {
                        @{
                            ViewData["Message"] = "Please correct the errors below:";
                            ViewData["Type"] = "warning";
                        }
                        <partial name="Common/_AlertMessage" />
                    }

                    <form method="post">
                        <!-- Sub-Project Name -->
                        <div class="mb-3">
                            <label asp-for="Input.Name" class="form-label">
                                Name <span class="text-danger">*</span>
                            </label>
                            <input asp-for="Input.Name" class="form-control form-control-lg" 
                                   placeholder="Enter sub-project name" />
                            <span asp-validation-for="Input.Name" class="text-danger small"></span>
                        </div>

                        <!-- Description -->
                        <div class="mb-3">
                            <label asp-for="Input.Description" class="form-label">Description</label>
                            <textarea asp-for="Input.Description" class="form-control" rows="3"
                                      placeholder="Enter description"></textarea>
                            <span asp-validation-for="Input.Description" class="text-danger small"></span>
                        </div>

                        <!-- Status -->
                        <div class="mb-3">
                            <label asp-for="Input.Status" class="form-label">
                                Status <span class="text-danger">*</span>
                            </label>
                            <select asp-for="Input.Status" class="form-select form-select-lg">
                                <option value="1">Not Started</option>
                                <option value="2">In Progress</option>
                                <option value="3">In Review</option>
                                <option value="4">Completed</option>
                            </select>
                            <span asp-validation-for="Input.Status" class="text-danger small"></span>
                        </div>

                        <!-- Form Buttons -->
                        <div class="d-grid gap-2 d-md-flex justify-content-md-end">
                            @{
                                ViewData["PageRoute"] = "~/Pages/SubProjects/Index";
                                ViewData["ButtonText"] = "Cancel";
                            }
                            <partial name="Common/_CancelButton" />
                            <button type="submit" class="btn btn-primary btn-lg">Save Changes</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
```

---

## 🎨 Example 7: Custom Component Styling

### Custom Button Styling

```razor
<!-- Default Back Button -->
<partial name="Common/_BackButton" />

<!-- Large, Dark Back Button -->
@{
    ViewData["CssClass"] = "btn btn-lg btn-dark";
    ViewData["ButtonText"] = "Return to List";
}
<partial name="Common/_BackButton" />

<!-- Outline Button -->
@{
    ViewData["CssClass"] = "btn btn-outline-secondary";
}
<partial name="Common/_BackButton" />

<!-- Wide Button -->
@{
    ViewData["CssClass"] = "btn btn-secondary w-100";
}
<partial name="Common/_BackButton" />
```

---

## 💾 Key Takeaways

1. **Consistency** - Same components across all pages
2. **DRY** - Don't repeat the same code
3. **Maintainability** - Update component once, applies everywhere
4. **Reusability** - Use in any page that needs the functionality
5. **Scalability** - Easy to add new components

---

**For more details, see:**
- README.md - Full documentation
- QUICK_REFERENCE.md - Quick lookup
- VISUAL_GUIDE.md - Visual examples
