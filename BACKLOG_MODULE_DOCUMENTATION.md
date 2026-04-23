# PM_Tool Backlog Module Documentation

## 1. Overview

The backlog module manages hierarchical project work items in Razor Pages.

Primary files:
- `PMTool.Web/Pages/Backlog/Index.cshtml`
- `PMTool.Web/Pages/Backlog/Index.cshtml.cs`

It supports:
- Structured backlog hierarchy
- Inline create/edit
- Drag-drop reorder
- Single and bulk delete
- Change Request handling

---

## 2. Hierarchy Rules

Current enforced hierarchy:

1. `BRD` (root, mandatory, only one per backlog scope)
2. `Epic` (must belong to BRD)
3. `UserStory` (must belong to Epic)
4. `UseCase` (must belong to UserStory)
5. `ChangeRequest` (must belong to UserStory)

Rendered order is built by `OrderedItems` through `BuildOrderedItems()`.

---

## 3. Backlog Scope

Backlog is loaded by:
- `ProjectId` (required)
- `ProductId` (optional, for product-level backlog)
- `Status` (optional)

Data call:
- `IBacklogService.GetBacklogItemsAsync(projectId, productId, subProjectId, status)`

---

## 4. Data Model

### Entity
- `PMTool.Domain/Entities/ProjectBacklog.cs`
  - Includes `ParentBacklogItemId` for self-referencing hierarchy
  - Navigation: `ParentBacklogItem`, `ChildBacklogItems`

### DTOs
- `BacklogItemDTO`
- `CreateBacklogItemRequest`
- `UpdateBacklogFieldRequest`

---

## 5. PageModel Endpoints (`Index.cshtml.cs`)

### `OnGetAsync`
Loads projects, products, owners, and filtered backlog items.

### `OnPostCreateAsync`
Validates and creates new item.

Validation rules:
- Project and title required
- One BRD only
- BRD required before creating non-BRD items
- Parent type validation for Epic/UserStory/UseCase/ChangeRequest

### `OnPostUpdateFieldAsync`
Inline field update for editable cells.

### `OnPostReorderAsync`
Updates priorities from drag-drop order.

### `OnPostDeleteAsync`
Deletes one item.

### `OnPostDeleteManyAsync`
Deletes selected items in bulk.

---

## 6. Change Request Behavior

`ChangeRequest` items:
- Must be linked to a `UserStory`
- Are rendered under `UserStory` in hierarchy
- Share indent level with `UseCase`

### Auto CR Numbering
When creating a `ChangeRequest`, if title does not start with `CR-`, title is prefixed automatically:
- `CR-001 ...`
- `CR-002 ...`

Logic currently uses existing CR count + 1 in the current backlog scope.

---

## 7. UI/UX Features (`Index.cshtml`)

- Table-based backlog visualization
- Hierarchy indent with indicator (`↳`)
- Inline text/select editing
- Dynamic parent dropdown based on selected type
- Drag-drop reorder
- Row hover delete (trash icon)
- Header checkbox (`select all`)
- Bulk delete icon appears only when one or more rows are selected
- Confirmation prompt before bulk delete

---

## 8. Security

- Page uses `[Authorize]`
- Anti-forgery token is included and sent for AJAX POST operations

---

## 9. Known Limitations

- CR numbering is count-based; concurrent creates may produce duplicate numbers
- Parent validation is strongest in create path; update path can be hardened further
- Bulk delete loops item-by-item (not transactional)

---

## 10. Suggested Next Improvements

1. Add dedicated `CRNumber` column (instead of title prefix)
2. Add transactional/sequence-based CR numbering
3. Add server-side parent validation for update-field requests
4. Add guardrails for deleting parent items with children
5. Add audit log for key field changes
6. Add approval workflow for change requests

---

## 11. User Flow (Quick)

1. Open backlog for a project/product
2. Create `BRD`
3. Create `Epic` under BRD
4. Create `UserStory` under Epic
5. Create `UseCase` and `ChangeRequest` under UserStory
6. Reorder with drag handle
7. Use row checkboxes + top bulk trash icon to delete multiple items
