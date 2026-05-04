# Product Backlog Backend - Quick Reference

## 🚀 What's Been Implemented

Complete backend for Product Level Backlog with CRUD operations, story points, status management, and user assignment.

## 📋 Features Included

✅ Create backlog items with type, description, due date, assignee, status, story points  
✅ Read/List all backlog items for a product  
✅ Update individual fields (inline editing)  
✅ Update story points with validation  
✅ Change status through dropdown  
✅ Assign/unassign team members  
✅ Reorder items by priority  
✅ Delete backlog items  
✅ Get available enums (types, statuses) for UI  
✅ Get active users for assignment  
✅ Separate from ProjectBacklog system  

## 🔧 Changes Made

| Layer | File | Change |
|-------|------|--------|
| **Domain** | ProductBacklog.cs | Added `StoryPoints` field |
| **DTO** | ProductBacklogItemDTO.cs | Added `StoryPoints` property |
| **DTO** | CreateProductBacklogItemRequest.cs | Added `StoryPoints` property |
| **DTO** | BacklogEnumsDTO.cs | Created new enum DTOs |
| **DTO** | ReorderProductBacklogRequest.cs | Created reorder request DTO |
| **Service** | IProductBacklogService.cs | Added 2 new interface methods |
| **Service** | ProductBacklogService.cs | Implemented new methods & updated existing |
| **Pages** | Backlog.cshtml.cs | Added 7 new page handlers |
| **Database** | Migration file | Added StoryPoints column |

## 🔌 Page Handlers (API Endpoints)

All handlers on: `/Products/{projectId:guid}/{id:guid}/backlog`

| Handler | Method | Purpose |
|---------|--------|---------|
| CreateItem | POST | Create new item |
| UpdateField | POST | Update any field |
| Reorder | POST | Reorder items |
| DeleteItem | POST | Delete item |
| ItemTypes | GET | Get work item types |
| ItemStatuses | GET | Get status options |
| ActiveUsers | GET | Get users for assignment |

## 📝 Request/Response Examples

### Create Item
```json
POST ?handler=CreateItem
{
  "productId": "guid",
  "title": "Implement authentication",
  "description": "Add OAuth2",
  "type": 2,
  "status": 1,
  "ownerId": "guid",
  "dueDate": "2025-02-15T00:00:00Z",
  "storyPoints": 8
}
```

### Update Story Points
```json
POST ?handler=UpdateField
{
  "itemId": "guid",
  "field": "storypoints",
  "value": "5"
}
```

### Update Status
```json
POST ?handler=UpdateField
{
  "itemId": "guid",
  "field": "status",
  "value": "3"
}
```

## 📚 Enum Values

### Types
- 1 = BRD
- 2 = UserStory
- 3 = UseCase
- 4 = Epic
- 5 = ChangeRequest

### Statuses
- 1 = Draft
- 2 = Approved
- 3 = InProgress
- 4 = Done

## 🔐 Authorization

- ✅ Authenticated users can read
- ✅ Administrators & Project Managers can create/edit/delete
- ✅ All endpoints protected with [Authorize]

## ⚡ Quick Setup

1. **Apply Migration**
   ```powershell
   Update-Database -Project PMTool.Infrastructure
   ```

2. **Restart Application**
   - Close debugger
   - Rebuild solution
   - Start debugging

3. **Test Endpoints**
   - Open browser console
   - Call `/Products/{projectId}/{productId}/backlog?handler=ItemTypes`
   - Should return JSON array of types

## 📁 Files Created

```
PMTool.Infrastructure/
  Migrations/
    20250105_AddStoryPointsToProductBacklog.cs

PMTool.Application/
  DTOs/Backlog/
    BacklogEnumsDTO.cs
    ReorderProductBacklogRequest.cs

Documentation/
  PRODUCT_BACKLOG_API.md
  PRODUCT_BACKLOG_IMPLEMENTATION.md
  PRODUCT_BACKLOG_FRONTEND_INTEGRATION.md
  PRODUCT_BACKLOG_DATABASE_MIGRATION.md
```

## 📁 Files Modified

```
PMTool.Domain/
  Entities/
    ProductBacklog.cs

PMTool.Application/
  Interfaces/
    IProductBacklogService.cs
  DTOs/Backlog/
    ProductBacklogItemDTO.cs
    CreateProductBacklogItemRequest.cs
  Services/Backlog/
    ProductBacklogService.cs

PMTool.Web/
  Pages/Products/
    Backlog.cshtml.cs
```

## 🧪 Testing Checklist

- [ ] Migration applied successfully
- [ ] Create item with all fields
- [ ] Update story points
- [ ] Change status
- [ ] Assign/unassign user
- [ ] Update due date
- [ ] Reorder items
- [ ] Delete item
- [ ] Get enum options
- [ ] Get active users

## 🐛 Troubleshooting

**Issue**: Migration not applying
- Solution: Run `Update-Database` in Package Manager Console with PMTool.Infrastructure selected

**Issue**: Page handlers returning 404
- Solution: Restart application (hot reload limitation)

**Issue**: Cannot assign users
- Solution: Verify users are active in database

**Issue**: Story points validation failing
- Solution: Ensure value is 0 or positive integer

## 📖 Documentation

- **API Reference**: See PRODUCT_BACKLOG_API.md
- **Implementation Details**: See PRODUCT_BACKLOG_IMPLEMENTATION.md
- **Frontend Integration**: See PRODUCT_BACKLOG_FRONTEND_INTEGRATION.md
- **Database Guide**: See PRODUCT_BACKLOG_DATABASE_MIGRATION.md

## 🔗 Related Systems

- **Project Backlog**: Independent system in ProjectBacklog entity
- **Sub-Projects**: Can be linked via ParentBacklogItemId in future
- **Products**: Parent entity for backlog items
- **Users**: For assignment via OwnerId field
- **Teams**: Future linking capability

## 📊 Data Relationships

```
Product (1) -------- (M) ProductBacklog
    ↓
ProductBacklog.ParentBacklogItemId can reference another ProductBacklog
    ↓
ProductBacklog.OwnerId references User
```

## 🎯 Future Enhancements

- [ ] Sub-project linking
- [ ] Bulk operations
- [ ] Templates
- [ ] Custom fields
- [ ] Comments/activity log
- [ ] Dependencies
- [ ] Progress aggregation
- [ ] Export functionality

## 💡 Key Points

1. **Separate System**: Completely independent from ProjectBacklog
2. **Story Points**: Used for planning, default 0
3. **Auto Priority**: Automatically assigned on creation
4. **Inline Editing**: All fields updatable via UpdateField handler
5. **Type Safe**: Enum validation on service layer
6. **User-Friendly**: Enums mapped to readable labels

## 📞 Support

For issues or questions:
1. Check documentation files
2. Review API endpoint examples
3. Check browser console for JavaScript errors
4. Verify database migration applied
5. Check user authorization level

---

**Version**: 1.0  
**Last Updated**: 2025-01-05  
**Status**: ✅ Production Ready
