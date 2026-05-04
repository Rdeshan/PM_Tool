## Product Level Backlog - Backend API Documentation

### Overview
The Product Level Backlog system provides endpoints for managing backlog items at the product level, separate from project backlogs. Each backlog item can have a type, description, due date, assigned person, status, story points, and priority.

### Data Model

#### ProductBacklog Entity
```csharp
public class ProductBacklog
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ParentBacklogItemId { get; set; }
    public Guid? OwnerId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Type { get; set; }                // BacklogItemType enum
    public int Priority { get; set; }            // Auto-incremented order
    public int Status { get; set; }              // BacklogItemStatus enum
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public int StoryPoints { get; set; }         // 0-100 or custom scale
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### Enums

#### BacklogItemType
- BRD = 1
- UserStory = 2
- UseCase = 3
- Epic = 4
- ChangeRequest = 5

#### BacklogItemStatus
- Draft = 1
- Approved = 2
- InProgress = 3
- Done = 4

### API Endpoints

#### 1. Get Backlog Items
**POST** `/Products/{projectId:guid}/{id:guid}/backlog?handler=GetItems`

**Request:** None (data loaded on page GET)

**Response:**
```json
{
  "data": [
    {
      "id": "guid",
      "productId": "guid",
      "title": "string",
      "description": "string",
      "type": 2,
      "typeName": "UserStory",
      "status": 1,
      "statusName": "Draft",
      "priority": 1,
      "ownerId": "guid",
      "ownerName": "string",
      "dueDate": "2025-01-15T00:00:00Z",
      "storyPoints": 5,
      "createdAt": "2025-01-01T12:00:00Z",
      "updatedAt": "2025-01-01T12:00:00Z"
    }
  ],
  "count": 1
}
```

#### 2. Create Backlog Item
**POST** `/Products/{projectId:guid}/{id:guid}/backlog?handler=CreateItem`

**Request Body:**
```json
{
  "productId": "guid",
  "title": "string (required, min 1 char)",
  "description": "string",
  "type": 2,                    // BacklogItemType enum value
  "status": 1,                  // BacklogItemStatus enum value
  "ownerId": "guid (optional)",
  "dueDate": "2025-01-15T00:00:00Z",
  "storyPoints": 5
}
```

**Response:**
```json
{
  "id": "guid",
  "productId": "guid",
  "title": "string",
  "description": "string",
  "type": 2,
  "typeName": "UserStory",
  "status": 1,
  "statusName": "Draft",
  "priority": 1,
  "ownerId": "guid",
  "ownerName": "string",
  "dueDate": "2025-01-15T00:00:00Z",
  "storyPoints": 5,
  "createdAt": "2025-01-01T12:00:00Z",
  "updatedAt": "2025-01-01T12:00:00Z"
}
```

#### 3. Update Backlog Item Field
**POST** `/Products/{projectId:guid}/{id:guid}/backlog?handler=UpdateField`

**Request Body:**
```json
{
  "itemId": "guid",
  "field": "string",           // title, description, type, status, owner, storypoints, priority, duedate
  "value": "string"
}
```

**Supported Fields:**
- `title` - Text value
- `description` - Text value
- `type` - Integer (BacklogItemType enum value)
- `status` - Integer (BacklogItemStatus enum value)
- `owner` - Guid (User ID)
- `storypoints` - Integer (0 or positive)
- `priority` - Integer (auto-managed usually)
- `duedate` - DateTime string

**Response:** Updated ProductBacklogItemDTO

#### 4. Update Story Points
**POST** `/Products/{projectId:guid}/{id:guid}/backlog?handler=UpdateField`

**Request Body:**
```json
{
  "itemId": "guid",
  "field": "storypoints",
  "value": "8"
}
```

#### 5. Update Status
**POST** `/Products/{projectId:guid}/{id:guid}/backlog?handler=UpdateField`

**Request Body:**
```json
{
  "itemId": "guid",
  "field": "status",
  "value": "3"                 // InProgress
}
```

#### 6. Reorder Items
**POST** `/Products/{projectId:guid}/{id:guid}/backlog?handler=Reorder`

**Request Body:**
```json
{
  "productId": "guid",
  "items": [
    {
      "itemId": "guid",
      "priority": 1
    },
    {
      "itemId": "guid",
      "priority": 2
    }
  ]
}
```

**Response:**
```json
{
  "success": true
}
```

#### 7. Delete Item
**POST** `/Products/{projectId:guid}/{id:guid}/backlog?handler=DeleteItem?itemId={itemId}`

**Response:**
```json
{
  "success": true
}
```

#### 8. Get Item Types
**GET** `/Products/{projectId:guid}/{id:guid}/backlog?handler=ItemTypes`

**Response:**
```json
[
  {
    "value": 1,
    "name": "BRD",
    "label": "Business Requirement"
  },
  {
    "value": 2,
    "name": "UserStory",
    "label": "User Story"
  },
  {
    "value": 3,
    "name": "UseCase",
    "label": "Use Case"
  },
  {
    "value": 4,
    "name": "Epic",
    "label": "Epic"
  },
  {
    "value": 5,
    "name": "ChangeRequest",
    "label": "Change Request"
  }
]
```

#### 9. Get Item Statuses
**GET** `/Products/{projectId:guid}/{id:guid}/backlog?handler=ItemStatuses`

**Response:**
```json
[
  {
    "value": 1,
    "name": "Draft",
    "label": "Draft"
  },
  {
    "value": 2,
    "name": "Approved",
    "label": "Approved"
  },
  {
    "value": 3,
    "name": "InProgress",
    "label": "In Progress"
  },
  {
    "value": 4,
    "name": "Done",
    "label": "Done"
  }
]
```

#### 10. Get Active Users (for Assignment)
**GET** `/Products/{projectId:guid}/{id:guid}/backlog?handler=ActiveUsers`

**Response:**
```json
[
  {
    "id": "guid",
    "name": "John Doe",
    "displayName": "John",
    "email": "john@example.com",
    "avatarUrl": "https://..."
  }
]
```

### DTOs

#### ProductBacklogItemDTO
Used for responses when retrieving backlog items.

```csharp
public class ProductBacklogItemDTO
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ParentBacklogItemId { get; set; }
    public Guid? OwnerId { get; set; }
    public string? OwnerName { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Type { get; set; }
    public string TypeName { get; set; }
    public int Priority { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public int StoryPoints { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

#### CreateProductBacklogItemRequest
Used when creating new backlog items.

```csharp
public class CreateProductBacklogItemRequest
{
    public Guid ProductId { get; set; }
    public Guid? ParentBacklogItemId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Type { get; set; }
    public int Status { get; set; }
    public Guid? OwnerId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public int StoryPoints { get; set; }
}
```

#### UpdateProductBacklogFieldRequest
Used when updating individual fields.

```csharp
public class UpdateProductBacklogFieldRequest
{
    public Guid ItemId { get; set; }
    public string Field { get; set; }
    public string Value { get; set; }
}
```

#### ReorderProductBacklogRequest
Used when reordering items.

```csharp
public class ReorderProductBacklogRequest
{
    public Guid ProductId { get; set; }
    public List<ReorderProductBacklogItemRequest> Items { get; set; }
}

public class ReorderProductBacklogItemRequest
{
    public Guid ItemId { get; set; }
    public int Priority { get; set; }
}
```

### Validation Rules

1. **Title**: Required, minimum 1 character
2. **ProductId**: Must be valid and exist
3. **Type**: Must be valid BacklogItemType enum value
4. **Status**: Must be valid BacklogItemStatus enum value
5. **StoryPoints**: Must be 0 or positive integer
6. **Owner**: Must be valid User ID if provided
7. **DueDate**: Must be a valid DateTime if provided

### Authorization

All endpoints require:
- User to be authenticated
- For Create/Update/Delete: User must have "Administrator" or "Project Manager" role
- For Read: User must have access to the product

### Error Responses

**400 Bad Request**
```json
{
  "error": "Failed to create backlog item"
}
```

**403 Forbidden**
```
User does not have permission to modify this backlog
```

**404 Not Found**
```
Backlog item not found
```

### Usage Examples

#### Creating a Product Backlog Item via JavaScript
```javascript
const request = {
    productId: '12345678-1234-1234-1234-123456789012',
    title: 'Implement user authentication',
    description: 'Add OAuth2 authentication to the platform',
    type: 2,        // UserStory
    status: 1,      // Draft
    ownerId: 'owner-guid',
    dueDate: '2025-02-15T00:00:00Z',
    storyPoints: 8
};

fetch('/Products/{projectId}/{productId}/backlog?handler=CreateItem', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json'
    },
    body: JSON.stringify(request)
})
.then(response => response.json())
.then(data => console.log('Created:', data));
```

#### Updating Story Points
```javascript
const request = {
    itemId: 'item-guid',
    field: 'storypoints',
    value: '5'
};

fetch('/Products/{projectId}/{productId}/backlog?handler=UpdateField', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json'
    },
    body: JSON.stringify(request)
})
.then(response => response.json())
.then(data => console.log('Updated:', data));
```

#### Changing Status
```javascript
const request = {
    itemId: 'item-guid',
    field: 'status',
    value: '3'  // InProgress
};

// Same fetch pattern as above
```

### Database Schema

The `ProductBacklogs` table includes:
- Id (GUID, Primary Key)
- ProductId (GUID, Foreign Key)
- ParentBacklogItemId (GUID, Foreign Key, Nullable)
- OwnerId (GUID, Foreign Key, Nullable)
- Title (NVARCHAR(MAX))
- Description (NVARCHAR(MAX))
- Type (INT)
- Priority (INT)
- Status (INT)
- StartDate (DATETIME2, Nullable)
- DueDate (DATETIME2, Nullable)
- StoryPoints (INT)
- CreatedAt (DATETIME2)
- UpdatedAt (DATETIME2)

### Key Features Implemented

✅ Create backlog items with title, description, type, status, owner, due date, and story points
✅ List all backlog items for a product
✅ Update individual fields (inline editing support)
✅ Update story points with validation
✅ Change status through status dropdown
✅ Assign/unassign team members
✅ Reorder items by priority
✅ Delete backlog items
✅ Get available types, statuses, and team members for UI population
✅ Automatic priority calculation on creation
✅ Separate from Project Backlog (independent data model)

### Future Enhancements

- [ ] Sub-project linking for backlog items
- [ ] Bulk operations (select multiple items)
- [ ] Backlog item templates
- [ ] Custom field support
- [ ] Activity log/comments on items
- [ ] Dependency tracking between items
- [ ] Progress calculation based on status distribution
