# Product Backlog Architecture Diagrams

## System Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        USER INTERFACE                           │
│                   (Razor Pages: Backlog.cshtml)                 │
│  ┌──────────────┐ ┌──────────────┐ ┌──────────────────────────┐ │
│  │   Create UI  │ │   Display    │ │   Action Buttons         │ │
│  │   - Type     │ │   - List     │ │   - Edit icon            │ │
│  │   - Title    │ │   - Status   │ │   - Story Points         │ │
│  │   - Desc     │ │   - Owner    │ │   - Priority icon        │ │
│  │   - Due Date │ │   - Points   │ │   - Delete               │ │
│  │   - Assign   │ │              │ │   - SubProject link      │ │
│  └──────────────┘ └──────────────┘ └──────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
                            ↓
                    [JavaScript/jQuery]
                            ↓
         ┌────────────────────────────────────────┐
         │     HTTP POST/GET Requests             │
         │  ?handler=CreateItem                   │
         │  ?handler=UpdateField                  │
         │  ?handler=Reorder                      │
         │  ?handler=DeleteItem                   │
         │  ?handler=ItemTypes                    │
         │  ?handler=ItemStatuses                 │
         │  ?handler=ActiveUsers                  │
         └────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│              PAGE HANDLER LAYER (Authorize)                    │
│              (BacklogModel - Backlog.cshtml.cs)                │
│                                                                 │
│  OnPostCreateItemAsync()     OnGetItemTypes()                  │
│  OnPostUpdateFieldAsync()    OnGetItemStatuses()               │
│  OnPostReorderAsync()        OnGetActiveUsers()                │
│  OnPostDeleteItemAsync()                                        │
│                                                                 │
│          [Validates Roles & Permissions]                       │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│           SERVICE LAYER (Business Logic)                       │
│      (ProductBacklogService: IProductBacklogService)           │
│                                                                 │
│  CreateBacklogItemAsync()     GetBacklogItemTypes()            │
│  GetBacklogItemsAsync()       GetBacklogItemStatuses()         │
│  UpdateBacklogFieldAsync()    MapToDto()                       │
│  ReorderItemsAsync()                                            │
│  DeleteItemAsync()                                              │
│                                                                 │
│     [Validation • Mapping • Business Rules]                     │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│           REPOSITORY LAYER (Data Access)                       │
│     (ProductBacklogRepository: IProductBacklogRepository)      │
│                                                                 │
│  GetByIdAsync()              CreateAsync()                     │
│  GetByFilterAsync()          UpdateAsync()                     │
│  GetNextPriorityAsync()      UpdateRangeAsync()                │
│  DeleteAsync()                                                  │
│                                                                 │
│        [Entity Framework Core LINQ Queries]                     │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│              DATABASE LAYER                                    │
│           (SQL Server / AppDbContext)                          │
│                                                                 │
│   ┌────────────────────────────────────────────────────┐       │
│   │  ProductBacklogs Table                             │       │
│   │  ┌─────────────────────────────────────────────┐  │       │
│   │  │ Id (PK)                                     │  │       │
│   │  │ ProductId (FK)                              │  │       │
│   │  │ Title                                       │  │       │
│   │  │ Description                                 │  │       │
│   │  │ Type                                        │  │       │
│   │  │ Status                                      │  │       │
│   │  │ Priority                                    │  │       │
│   │  │ OwnerId (FK)                                │  │       │
│   │  │ DueDate                                     │  │       │
│   │  │ StoryPoints ⬅ NEW                           │  │       │
│   │  │ CreatedAt                                   │  │       │
│   │  │ UpdatedAt                                   │  │       │
│   │  └─────────────────────────────────────────────┘  │       │
│   └────────────────────────────────────────────────────┘       │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## Data Flow Diagram

### Create Item Flow

```
┌─────────────────┐
│  Frontend Form  │  {title, description, type, status, ownerId, dueDate, storyPoints}
└────────┬────────┘
         │
         ↓
   [Fetch POST]
         │
         ↓
┌──────────────────────────────────────────┐
│  PageHandler: OnPostCreateItemAsync      │  1. Extract request body
│                                          │  2. Check [Authorize]
│                                          │  3. Call service
└────────┬─────────────────────────────────┘
         │
         ↓
┌──────────────────────────────────────────┐
│  Service: CreateBacklogItemAsync         │  1. Validate inputs
│                                          │  2. GetNextPriority()
│  ✓ ProductId exists?                     │  3. Create entity
│  ✓ Title not empty?                      │  4. Call repository.CreateAsync()
│  ✓ Type valid?                           │  5. MapToDto()
│  ✓ Status valid?                         │  6. Return DTO
│  ✓ StoryPoints >= 0?                     │
│  ✓ OwnerId valid?                        │
└────────┬─────────────────────────────────┘
         │
         ↓
┌──────────────────────────────────────────┐
│  Repository: CreateAsync                 │  1. Add entity to DbSet
│                                          │  2. SaveChangesAsync()
│  await _context.ProductBacklogs.Add()    │  3. Return success
│  await _context.SaveChangesAsync()       │
└────────┬─────────────────────────────────┘
         │
         ↓
┌──────────────────────────────────────────┐
│  Database: ProductBacklogs Table         │  INSERT INTO ProductBacklogs
│                                          │  VALUES (...)
└────────┬─────────────────────────────────┘
         │
         ↓
┌──────────────────────────────────────────┐
│  Response: ProductBacklogItemDTO         │  {id, title, type, typeName, ...}
│                                          │
│  {                                       │
│    "id": "guid",                         │
│    "productId": "guid",                  │
│    "title": "string",                    │
│    "storyPoints": 8,                     │
│    "status": 1,                          │
│    ...                                   │
│  }                                       │
└────────┬─────────────────────────────────┘
         │
         ↓
┌──────────────────────┐
│  Frontend Updates UI │  Display in list
└──────────────────────┘
```

### Update Story Points Flow

```
┌──────────────────────────┐
│  User Clicks Edit Points │
└────────┬─────────────────┘
         │
         ↓
   [Input Modal/Inline]
   Enter: 5
         │
         ↓
   [Fetch POST]
   {itemId, field: "storypoints", value: "5"}
         │
         ↓
┌─────────────────────────────────────────┐
│  PageHandler: OnPostUpdateFieldAsync    │
└────────┬────────────────────────────────┘
         │
         ↓
┌─────────────────────────────────────────┐
│  Service: UpdateBacklogFieldAsync       │
│                                         │
│  case "storypoints":                    │
│    if (int.TryParse(value, out var sp)) │
│      if (sp >= 0)                       │
│        item.StoryPoints = sp            │
│      else                               │
│        return error                     │
└────────┬────────────────────────────────┘
         │
         ↓
┌─────────────────────────────────────────┐
│  Repository: UpdateAsync                │
│                                         │
│  _context.ProductBacklogs.Update()      │
│  _context.SaveChangesAsync()            │
└────────┬────────────────────────────────┘
         │
         ↓
┌─────────────────────────────────────────┐
│  Database: UPDATE ProductBacklogs       │
│                                         │
│  WHERE Id = @itemId                     │
│  SET StoryPoints = @value               │
└────────┬────────────────────────────────┘
         │
         ↓
┌─────────────────────────────────────────┐
│  Response: Updated ProductBacklogItemDTO│
└────────┬────────────────────────────────┘
         │
         ↓
┌──────────────────────────┐
│  Frontend: Update UI     │
│  Show new story points   │
└──────────────────────────┘
```

---

## Entity Relationship Diagram

```
┌─────────────────────┐
│     Product         │
├─────────────────────┤
│ Id (PK)             │
│ Name                │
│ VersionName         │
│ ProjectId (FK)      │
│ ...                 │
└──────────┬──────────┘
           │
         1 │ (1..M)
           │
           ↓
┌──────────────────────────┐
│   ProductBacklog         │
├──────────────────────────┤
│ Id (PK)                  │
│ ProductId (FK) ··→ ●     │
│ ParentBacklogItemId (FK) │ (Self-referencing for hierarchy)
│ OwnerId (FK)         ··→ ●
│ Title                    │
│ Description              │
│ Type                     │
│ Status                   │
│ Priority                 │
│ DueDate                  │
│ StoryPoints ⬅ NEW        │
│ CreatedAt                │
│ UpdatedAt                │
└─────────┬────────────────┘
          │
      1 M │ (Hierarchical)
          │
      ┌───┴──────────────────┐
      │                      │
      ↓                      ↓
   Parent              Child Items
 Backlog Item       (ParentBacklogItemId)
```

---

## Enum Mapping

### Type Enum

```
┌────┬───────────────┬──────────────────────┐
│ ID │ Code Name     │ Display Label        │
├────┼───────────────┼──────────────────────┤
│ 1  │ BRD           │ Business Requirement │
│ 2  │ UserStory     │ User Story           │
│ 3  │ UseCase       │ Use Case             │
│ 4  │ Epic          │ Epic                 │
│ 5  │ ChangeRequest │ Change Request       │
└────┴───────────────┴──────────────────────┘
```

### Status Enum

```
┌────┬─────────────┬──────────────┐
│ ID │ Code Name   │ Display Name │
├────┼─────────────┼──────────────┤
│ 1  │ Draft       │ Draft        │
│ 2  │ Approved    │ Approved     │
│ 3  │ InProgress  │ In Progress  │
│ 4  │ Done        │ Done         │
└────┴─────────────┴──────────────┘
```

---

## DTO Transformation Flow

```
┌────────────────────────────────────┐
│  Database Entity                   │
│  (ProductBacklog)                  │
│                                    │
│  ✓ All fields                      │
│  ✓ Has navigation: Owner (User)    │
└────────────┬───────────────────────┘
             │
             │ MapToDto()
             ↓
┌────────────────────────────────────┐
│  Data Transfer Object              │
│  (ProductBacklogItemDTO)           │
│                                    │
│  ✓ Same core fields                │
│  ✓ TypeName (mapped from Type)     │
│  ✓ StatusName (mapped from Status) │
│  ✓ OwnerName (from navigation)     │
│  ✓ User-friendly display format    │
└────────────────────────────────────┘
             │
             │ Serialized to JSON
             ↓
┌────────────────────────────────────┐
│  Frontend JSON Response            │
│                                    │
│  {                                 │
│    "id": "...",                    │
│    "title": "...",                 │
│    "typeName": "UserStory",        │
│    "statusName": "Draft",          │
│    "ownerName": "John Doe",        │
│    "storyPoints": 8,               │
│    ...                             │
│  }                                 │
└────────────────────────────────────┘
```

---

## Request/Response Sequence Diagram

```
Frontend                Page Handler         Service            Repository      Database
   │                         │                  │                   │              │
   ├─ POST CreateItem ──────→│                  │                   │              │
   │                         │                  │                   │              │
   │                    [Authorize]             │                   │              │
   │                         │                  │                   │              │
   │                         ├─ CreateBacklog ─→│                   │              │
   │                         │                  │                   │              │
   │                         │              [Validate]              │              │
   │                         │                  │                   │              │
   │                         │              [GetNextPriority]       │              │
   │                         │                  ├─ Query ──────────→│              │
   │                         │                  │←─ Returns max id ─┤              │
   │                         │                  │                   │              │
   │                         │              [Create Entity]         │              │
   │                         │                  │                   │              │
   │                         │                  ├─ SaveChanges ────→│              │
   │                         │                  │                   ├─ INSERT ────→│
   │                         │                  │←─ Success ────────┤              │
   │                         │←─ Return DTO ───│                   │              │
   │←─ JSON Response ────────│                  │                   │              │
   │                         │                  │                   │              │
   ├─ Update UI             │                  │                   │              │
   └─ Display Item          │                  │                   │              │

```

---

## Priority Calculation Flow

```
New Item Created
       │
       ↓
GetNextPriorityAsync()
       │
       ├─ Query: MAX(Priority) from ProductBacklogs WHERE ProductId = @productId
       │
       ├─ If result is NULL or 0, next priority = 1
       │
       ├─ Else, next priority = MAX + 1
       │
       ↓
Return: 1, 2, 3, 4, 5, ...

Example:
Product A has 3 items with priorities 1, 2, 3
       ↓
New item gets priority 4
       ↓
If reordered, priorities can be: 1, 3, 5, 2 (any order)
```

---

## Authorization Flow

```
Request arrives at PageHandler
       │
       ├─ Is user authenticated?
       │   └─ NO → 401 Unauthorized
       │
       ├─ Does [Authorize] attribute match?
       │   └─ NO (anonymous) → 403 Forbidden
       │
       ├─ For Create/Update/Delete:
       │   │
       │   ├─ Is user "Administrator"? → ALLOW
       │   │
       │   ├─ Is user "Project Manager"? → ALLOW
       │   │
       │   └─ Else → 403 Forbidden
       │
       ├─ For Read/Get:
       │   │
       │   ├─ Is authenticated? → ALLOW
       │   │
       │   └─ Else → 401 Unauthorized
       │
       ↓
Process Request
```

---

## Story Points Validation

```
User enters StoryPoints value
       │
       ↓
Service receives value
       │
       ├─ Is it a valid integer?
       │   └─ NO → Parsing fails, use 0
       │
       ├─ Is it >= 0?
       │   ├─ YES → Accept value
       │   └─ NO → Validation fails, don't update
       │
       ↓
Update or Error
```

---

## Update Operation Flow

```
GET /Products/...?handler=UpdateField

Request: {
  itemId: "guid",
  field: "status",
  value: "3"
}

       │
       ↓
Service.UpdateBacklogFieldAsync()
       │
       ├─ Load item from DB
       │
       ├─ Switch on field name:
       │   ├─ "title" → item.Title = value
       │   ├─ "description" → item.Description = value
       │   ├─ "type" → item.Type = int.Parse(value)
       │   ├─ "status" → item.Status = int.Parse(value)
       │   ├─ "owner" → item.OwnerId = Guid.Parse(value)
       │   ├─ "storypoints" → item.StoryPoints = int.Parse(value)
       │   ├─ "duedate" → item.DueDate = DateTime.Parse(value)
       │   └─ default → No change
       │
       ├─ item.UpdatedAt = DateTime.UtcNow
       │
       ├─ Save to database
       │
       ├─ Map to DTO
       │
       ↓
Return updated DTO to frontend
```

---

## Reorder Flow

```
User reorders items (drag & drop)
       │
       ↓
Send POST with new order:
{
  productId: "guid",
  items: [
    { itemId: "id1", priority: 1 },
    { itemId: "id2", priority: 2 },
    { itemId: "id3", priority: 3 }
  ]
}
       │
       ↓
Service.ReorderItemsAsync()
       │
       ├─ Get all items for product
       │
       ├─ Create map: itemId → newPriority
       │
       ├─ For each item in collection:
       │   ├─ Look up new priority
       │   ├─ Update item.Priority
       │   └─ Set UpdatedAt
       │
       ├─ Save all updates
       │
       ↓
Return success confirmation
```

---

This comprehensive visual representation shows the complete architecture and data flows of the Product Backlog system.
