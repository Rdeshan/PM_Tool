# Product Level Backlog - Backend Implementation Summary

## Overview
Complete backend implementation for Product Level Backlog system with support for creating, reading, updating, and deleting backlog items at the product level. This is completely separate from the ProjectBacklog system.

## Changes Made

### 1. Domain Model Updates

#### Modified: `PMTool.Domain\Entities\ProductBacklog.cs`
- Added `StoryPoints` property (int, default 0) to track story points for each backlog item

### 2. Application Layer - DTOs

#### Modified: `PMTool.Application\DTOs\Backlog\ProductBacklogItemDTO.cs`
- Added `StoryPoints` property to include story points in API responses

#### Modified: `PMTool.Application\DTOs\Backlog\CreateProductBacklogItemRequest.cs`
- Added `StoryPoints` property to allow story points during item creation

#### Created: `PMTool.Application\DTOs\Backlog\BacklogEnumsDTO.cs`
- New DTOs for serializing enums:
  - `BacklogItemTypeDTO`: Represents available work item types
  - `BacklogItemStatusDTO`: Represents available statuses
  - `BacklogItemPriorityDTO`: Template for priority options

#### Created: `PMTool.Application\DTOs\Backlog\ReorderProductBacklogRequest.cs`
- Request DTO for reordering multiple backlog items with new priorities

### 3. Application Layer - Services

#### Modified: `PMTool.Application\Interfaces\IProductBacklogService.cs`
- Added `GetBacklogItemTypes()`: Returns available work item types as DTOs
- Added `GetBacklogItemStatuses()`: Returns available statuses as DTOs

#### Modified: `PMTool.Application\Services\Backlog\ProductBacklogService.cs`
- Updated `CreateBacklogItemAsync()`: Now includes StoryPoints from request
- Updated `UpdateBacklogFieldAsync()`: Added case for "storypoints" field to allow story points updates
- Updated `MapToDto()`: Now includes StoryPoints in DTO mapping
- Implemented `GetBacklogItemTypes()`: Returns all BacklogItemType enum values with labels
- Implemented `GetBacklogItemStatuses()`: Returns all BacklogItemStatus enum values with labels

### 4. Web Layer - Razor Pages

#### Modified: `PMTool.Web\Pages\Products\Backlog.cshtml.cs`
- Updated constructor to inject `IUserAdminService` for user retrieval
- Added `OnPostCreateItemAsync()`: Handler for creating new backlog items
- Added `OnPostUpdateFieldAsync()`: Handler for updating individual backlog item fields
- Added `OnPostReorderAsync()`: Handler for reordering backlog items
- Added `OnPostDeleteItemAsync()`: Handler for deleting backlog items
- Added `OnGetItemTypes()`: Handler to retrieve available work item types
- Added `OnGetItemStatuses()`: Handler to retrieve available statuses
- Added `OnGetActiveUsers()`: Handler to retrieve active users for assignment

### 5. Database

#### Created: Migration `20250105_AddStoryPointsToProductBacklog.cs`
- Adds `StoryPoints` column to `ProductBacklogs` table
- Column type: INT
- Default value: 0
- No null values allowed

## API Endpoints

All endpoints are Page Handlers on `/Products/{projectId:guid}/{id:guid}/backlog`:

| Method | Handler | Purpose |
|--------|---------|---------|
| POST | CreateItem | Create new backlog item |
| POST | UpdateField | Update specific field of backlog item |
| POST | UpdateField (with field="storypoints") | Update story points |
| POST | UpdateField (with field="status") | Update item status |
| POST | Reorder | Reorder items by priority |
| POST | DeleteItem | Delete a backlog item |
| GET | ItemTypes | Get available work item types |
| GET | ItemStatuses | Get available status values |
| GET | ActiveUsers | Get list of active users for assignment |

## Data Flow

### Creating a Backlog Item
1. Frontend sends `CreateProductBacklogItemRequest` with:
   - ProductId, Title, Description, Type, Status, OwnerId, DueDate, StoryPoints
2. `ProductBacklogService.CreateBacklogItemAsync()` validates and creates entity
3. Auto-generated priority based on `GetNextPriorityAsync()`
4. Item persisted to database
5. `ProductBacklogItemDTO` returned to frontend

### Updating Story Points
1. Frontend sends `UpdateProductBacklogFieldRequest` with:
   - ItemId, Field="storypoints", Value="8"
2. Service validates StoryPoints is non-negative integer
3. Item persisted to database
4. Updated `ProductBacklogItemDTO` returned

### Getting Available Options
1. Frontend calls `/backlog?handler=ItemTypes`
2. Service returns all `BacklogItemType` enum values with labels
3. Frontend can populate dropdowns/UI

## Security

- All endpoints require authentication (Authorize attribute)
- Create/Update/Delete require "Administrator" or "Project Manager" roles
- Read operations available to authenticated users with access to product

## Validation

- Title: Required, minimum 1 character
- ProductId: Must be valid GUID and exist
- Type: Valid BacklogItemType enum value
- Status: Valid BacklogItemStatus enum value
- StoryPoints: 0 or positive integer only
- Owner: Valid User ID if provided
- DueDate: Valid DateTime if provided

## Entity Relationships

```
Product (1) -------- (M) ProductBacklog
    ↓
ProductBacklog Items can have parent items (hierarchical structure)
```

## Testing Checklist

- [ ] Create backlog item with all fields populated
- [ ] Create backlog item with minimal fields
- [ ] Update title/description
- [ ] Update story points to various values (0, 5, 13, 21)
- [ ] Change status through dropdown
- [ ] Assign/unassign user as owner
- [ ] Update due date using date picker
- [ ] Reorder items and verify priority changes
- [ ] Delete backlog item
- [ ] Verify enum values returned correctly
- [ ] Verify active users list populated correctly
- [ ] Test authorization (non-PM should not be able to edit)

## Files Modified

- PMTool.Domain\Entities\ProductBacklog.cs
- PMTool.Application\DTOs\Backlog\ProductBacklogItemDTO.cs
- PMTool.Application\DTOs\Backlog\CreateProductBacklogItemRequest.cs
- PMTool.Application\Interfaces\IProductBacklogService.cs
- PMTool.Application\Services\Backlog\ProductBacklogService.cs
- PMTool.Web\Pages\Products\Backlog.cshtml.cs

## Files Created

- PMTool.Infrastructure\Migrations\20250105_AddStoryPointsToProductBacklog.cs
- PMTool.Application\DTOs\Backlog\BacklogEnumsDTO.cs
- PMTool.Application\DTOs\Backlog\ReorderProductBacklogRequest.cs

## Backward Compatibility

✅ All changes are backward compatible
- New fields have default values
- New interface methods are additions only
- Existing functionality preserved

## Notes

1. **Separate from ProjectBacklog**: Product Backlog is a completely independent entity from ProjectBacklog
2. **StoryPoints**: Can be used for planning and progress calculations in future
3. **Priority Management**: Auto-incremented on creation, can be manually managed via Reorder
4. **Enum Mapping**: All enums are properly mapped with user-friendly labels
5. **User Experience**: Supports inline editing through UpdateField endpoint

## Next Steps (Optional Future Features)

1. Add SubProject linking capability
2. Implement backlog item templates
3. Add bulk operations
4. Implement activity log/comments
5. Add dependency tracking between items
6. Calculate aggregate metrics (total story points, completion percentage)
7. Add filtering/search capabilities
8. Implement backlog export functionality
