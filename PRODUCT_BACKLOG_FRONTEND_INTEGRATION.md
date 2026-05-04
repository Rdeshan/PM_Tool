# Product Backlog - Frontend Integration Guide

## Overview
This guide explains how to integrate the Product Level Backlog backend with your existing Razor Pages UI.

## UI Components Already in Place

Your `Backlog.cshtml` already has:
- ✅ Type dropdown with icons
- ✅ Description input field
- ✅ Due date button (calendar)
- ✅ Person assignment button
- ✅ Create button
- ✅ Backlog items list table
- ✅ Edit/delete action buttons per item
- ✅ Status badges

## JavaScript Integration

### 1. Load Initial Data

```javascript
// Load backlog items on page load
async function loadBacklogItems() {
    try {
        const response = await fetch(window.location.href);
        const html = await response.text();
        // Items are already loaded in the page via Model.BacklogItems
        console.log('Backlog items loaded');
    } catch (error) {
        console.error('Failed to load backlog items:', error);
    }
}

// Load dropdown options
async function loadEnumOptions() {
    try {
        // Get item types
        const typesResponse = await fetch('?handler=ItemTypes');
        const types = await typesResponse.json();
        populateTypeDropdown(types);

        // Get statuses
        const statusResponse = await fetch('?handler=ItemStatuses');
        const statuses = await statusResponse.json();
        populateStatusOptions(statuses);

        // Get active users
        const usersResponse = await fetch('?handler=ActiveUsers');
        const users = await usersResponse.json();
        populateUserDropdown(users);
    } catch (error) {
        console.error('Failed to load options:', error);
    }
}

document.addEventListener('DOMContentLoaded', () => {
    loadEnumOptions();
});
```

### 2. Create Backlog Item

```javascript
// Get form data from UI
function getCreateItemFormData() {
    const type = document.querySelector('[data-type-selected]')?.dataset.typeSelected || '2'; // Default: UserStory
    const description = document.getElementById('itemDescription')?.value || '';
    const dueDate = document.getElementById('itemDueDate')?.value || null;
    const ownerId = document.querySelector('[data-owner-selected]')?.dataset.ownerSelected || null;

    return {
        productId: window.productId, // Set this from the Razor page model
        title: description, // Using description as title for now
        description: description,
        type: parseInt(type),
        status: 1, // Default: Draft
        ownerId: ownerId ? ownerId : null,
        dueDate: dueDate ? new Date(dueDate).toISOString() : null,
        storyPoints: 0 // Will be edited later
    };
}

async function createBacklogItem() {
    const formData = getCreateItemFormData();

    if (!formData.title.trim()) {
        showNotification('Please enter a title', 'error');
        return;
    }

    try {
        const response = await fetch('?handler=CreateItem', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getCSRFToken()
            },
            body: JSON.stringify(formData)
        });

        if (!response.ok) {
            throw new Error('Failed to create backlog item');
        }

        const newItem = await response.json();
        
        // Add item to table
        addItemToTable(newItem);
        
        // Clear form
        clearCreateForm();
        
        showNotification('Backlog item created successfully', 'success');
    } catch (error) {
        console.error('Error creating backlog item:', error);
        showNotification('Failed to create backlog item', 'error');
    }
}

document.getElementById('createItemBtn')?.addEventListener('click', createBacklogItem);
```

### 3. Update Item Field (Inline Editing)

```javascript
async function updateBacklogField(itemId, field, value) {
    try {
        const response = await fetch('?handler=UpdateField', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getCSRFToken()
            },
            body: JSON.stringify({
                itemId: itemId,
                field: field,
                value: value.toString()
            })
        });

        if (!response.ok) {
            throw new Error('Failed to update field');
        }

        const updatedItem = await response.json();
        
        // Update UI
        updateItemInTable(updatedItem);
        showNotification('Updated successfully', 'success');
        
        return updatedItem;
    } catch (error) {
        console.error('Error updating field:', error);
        showNotification('Failed to update field', 'error');
    }
}

// Update specific fields
async function updateItemStatus(itemId, newStatus) {
    await updateBacklogField(itemId, 'status', newStatus);
}

async function updateStoryPoints(itemId, points) {
    await updateBacklogField(itemId, 'storypoints', points);
}

async function updateItemOwner(itemId, userId) {
    await updateBacklogField(itemId, 'owner', userId);
}

async function updateDueDate(itemId, date) {
    await updateBacklogField(itemId, 'duedate', date);
}

// Event delegation for inline edits
document.addEventListener('click', async (e) => {
    // Status change
    if (e.target.matches('.status-dropdown')) {
        const itemId = e.target.dataset.itemId;
        const newStatus = e.target.value;
        await updateItemStatus(itemId, newStatus);
    }

    // Story points edit
    if (e.target.matches('.story-points-input')) {
        const itemId = e.target.dataset.itemId;
        const points = e.target.value;
        await updateStoryPoints(itemId, points);
    }

    // Owner assignment
    if (e.target.matches('.owner-select')) {
        const itemId = e.target.dataset.itemId;
        const userId = e.target.value;
        await updateItemOwner(itemId, userId);
    }
});
```

### 4. Delete Backlog Item

```javascript
async function deleteBacklogItem(itemId) {
    if (!confirm('Are you sure you want to delete this item?')) {
        return;
    }

    try {
        const response = await fetch(`?handler=DeleteItem&itemId=${itemId}`, {
            method: 'POST',
            headers: {
                'RequestVerificationToken': getCSRFToken()
            }
        });

        if (!response.ok) {
            throw new Error('Failed to delete item');
        }

        // Remove from table
        const row = document.querySelector(`[data-item-id="${itemId}"]`);
        row?.remove();

        showNotification('Item deleted successfully', 'success');
    } catch (error) {
        console.error('Error deleting item:', error);
        showNotification('Failed to delete item', 'error');
    }
}

// Event listener for delete buttons
document.addEventListener('click', (e) => {
    if (e.target.matches('.btn-delete-item')) {
        const itemId = e.target.dataset.itemId;
        deleteBacklogItem(itemId);
    }
});
```

### 5. Reorder Items

```javascript
async function reorderBacklogItems(itemOrderMap) {
    // itemOrderMap: Map of itemId -> newPriority
    
    const items = Array.from(itemOrderMap).map(([itemId, priority]) => ({
        itemId: itemId,
        priority: priority
    }));

    try {
        const response = await fetch('?handler=Reorder', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getCSRFToken()
            },
            body: JSON.stringify({
                productId: window.productId,
                items: items
            })
        });

        if (!response.ok) {
            throw new Error('Failed to reorder items');
        }

        showNotification('Items reordered successfully', 'success');
    } catch (error) {
        console.error('Error reordering items:', error);
        showNotification('Failed to reorder items', 'error');
    }
}

// Enable drag-and-drop reordering
let draggedRow = null;

document.addEventListener('dragstart', (e) => {
    if (e.target.matches('tr[draggable="true"]')) {
        draggedRow = e.target;
        e.dataTransfer.effectAllowed = 'move';
    }
});

document.addEventListener('dragover', (e) => {
    if (e.target.matches('tr')) {
        e.preventDefault();
        e.dataTransfer.dropEffect = 'move';
    }
});

document.addEventListener('drop', async (e) => {
    e.preventDefault();
    
    if (draggedRow && e.target.matches('tr')) {
        const tbody = document.querySelector('tbody');
        const rows = Array.from(tbody.querySelectorAll('tr'));
        
        // Update priorities
        const itemOrderMap = new Map();
        rows.forEach((row, index) => {
            const itemId = row.dataset.itemId;
            itemOrderMap.set(itemId, index + 1);
        });

        await reorderBacklogItems(itemOrderMap);
        draggedRow = null;
    }
});
```

### 6. Helper Functions

```javascript
// Get CSRF token
function getCSRFToken() {
    return document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
}

// Show notification
function showNotification(message, type = 'info') {
    // Using Bootstrap toast or your preferred notification library
    const alertClass = type === 'error' ? 'alert-danger' : 
                       type === 'success' ? 'alert-success' : 'alert-info';
    
    const alert = document.createElement('div');
    alert.className = `alert ${alertClass} alert-dismissible fade show`;
    alert.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    
    document.body.insertBefore(alert, document.body.firstChild);
    
    setTimeout(() => alert.remove(), 5000);
}

// Clear create form
function clearCreateForm() {
    document.getElementById('itemDescription').value = '';
    document.getElementById('itemDueDate').value = '';
    document.querySelector('[data-owner-selected]').dataset.ownerSelected = '';
    document.querySelector('[data-type-selected]').dataset.typeSelected = '2';
}

// Populate dropdowns
function populateTypeDropdown(types) {
    const typeDropdown = document.querySelector('.type-dropdown .dropdown-menu');
    typeDropdown.innerHTML = types.map(type => 
        `<li><a class="dropdown-item" href="#" data-type="${type.value}">${type.label}</a></li>`
    ).join('');

    typeDropdown.addEventListener('click', (e) => {
        if (e.target.matches('.dropdown-item')) {
            e.preventDefault();
            const type = e.target.dataset.type;
            document.querySelector('[data-type-selected]').dataset.typeSelected = type;
            e.target.closest('.dropdown').querySelector('button').textContent = e.target.textContent;
        }
    });
}

function populateStatusOptions(statuses) {
    window.statusOptions = statuses; // Store for later use
}

function populateUserDropdown(users) {
    const userDropdown = document.querySelector('.person-dropdown .dropdown-menu');
    userDropdown.innerHTML = users.map(user => 
        `<li><a class="dropdown-item" href="#" data-user-id="${user.id}">${user.name}</a></li>`
    ).join('');

    userDropdown.addEventListener('click', (e) => {
        if (e.target.matches('.dropdown-item')) {
            e.preventDefault();
            const userId = e.target.dataset.userId;
            document.querySelector('[data-owner-selected]').dataset.ownerSelected = userId;
            e.target.closest('.dropdown').querySelector('button').textContent = e.target.textContent;
        }
    });
}

// Add item to table
function addItemToTable(item) {
    const tbody = document.querySelector('tbody');
    const row = document.createElement('tr');
    row.dataset.itemId = item.id;
    row.draggable = true;

    const statusBadgeClass = item.status === 1 ? 'bg-secondary' :
                            item.status === 2 ? 'bg-warning' :
                            item.status === 3 ? 'bg-info' :
                            item.status === 4 ? 'bg-success' : 'bg-light text-dark';

    row.innerHTML = `
        <td><input type="checkbox" value="${item.id}" /></td>
        <td>
            <span class="badge" style="background-color: #d4e6f1;">
                ${item.id.substring(0, 8).toUpperCase()}
            </span>
        </td>
        <td>
            <a href="#" class="text-decoration-none">${escapeHtml(item.title)}</a>
        </td>
        <td>
            <small class="text-muted">${item.typeName}</small>
        </td>
        <td>
            <select class="form-select form-select-sm status-dropdown" data-item-id="${item.id}">
                ${window.statusOptions.map(s => 
                    `<option value="${s.value}" ${s.value === item.status ? 'selected' : ''}>${s.name}</option>`
                ).join('')}
            </select>
        </td>
        <td>
            <small class="text-muted">${item.ownerName || 'Unassigned'}</small>
        </td>
        <td>
            <div class="dropdown">
                <button class="btn btn-sm btn-link text-muted p-0" type="button" data-bs-toggle="dropdown">
                    <i class="bi bi-three-dots-vertical"></i>
                </button>
                <ul class="dropdown-menu dropdown-menu-end shadow border-0">
                    <li><a class="dropdown-item" href="#"><i class="bi bi-pencil"></i> Edit</a></li>
                    <li><a class="dropdown-item" href="#"><i class="bi bi-plus"></i> + SubProject</a></li>
                    <li><a class="dropdown-item" href="#"><i class="bi bi-dash"></i> Story Points</a></li>
                    <li><hr class="dropdown-divider"></li>
                    <li><a class="dropdown-item text-danger btn-delete-item" href="#" data-item-id="${item.id}"><i class="bi bi-trash"></i> Delete</a></li>
                </ul>
            </div>
        </td>
    `;

    tbody.appendChild(row);
}

// Update item in table
function updateItemInTable(item) {
    const row = document.querySelector(`tr[data-item-id="${item.id}"]`);
    if (!row) return;

    // Update only changed fields
    const statusSelect = row.querySelector('.status-dropdown');
    if (statusSelect && statusSelect.value !== item.status) {
        statusSelect.value = item.status;
    }

    const ownerCell = row.querySelector('td:nth-child(6)');
    if (ownerCell) {
        ownerCell.querySelector('small').textContent = item.ownerName || 'Unassigned';
    }
}

// Escape HTML to prevent XSS
function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

// Set product ID on window object (from Razor page)
window.productId = '@Model.ProductId';
```

## Razor Page Integration

Add these to your `Backlog.cshtml`:

```html
<!-- Add draggable attribute to table rows -->
<tr data-item-id="@item.Id" draggable="true">
    ...
</tr>

<!-- Add story points display -->
<td>
    <span class="story-points-badge">@item.StoryPoints pts</span>
    <button class="btn btn-sm btn-link p-0 ms-2 story-points-edit" 
            data-item-id="@item.Id" title="Edit story points">
        <i class="bi bi-dash"></i>
    </button>
</td>

<!-- Add status dropdown -->
<td>
    <select class="form-select form-select-sm status-dropdown" data-item-id="@item.Id">
        <option value="1" @(item.Status == 1 ? "selected" : "")>Todo</option>
        <option value="2" @(item.Status == 2 ? "selected" : "")>In Progress</option>
        <option value="3" @(item.Status == 3 ? "selected" : "")>In Review</option>
        <option value="4" @(item.Status == 4 ? "selected" : "")>Done</option>
    </select>
</td>

<!-- Add priority icon -->
<td>
    <span class="priority-icon" data-priority="@item.Priority" title="Priority">
        <i class="bi bi-arrow-up"></i>
    </span>
</td>
```

## Testing

1. **Create Item**: 
   - Fill type, description, due date, assignee
   - Click Create
   - Verify item appears in list with Draft status
   - Verify story points are 0

2. **Update Status**:
   - Click status dropdown
   - Select "In Progress"
   - Verify status updates without page reload

3. **Update Story Points**:
   - Click minus icon
   - Enter story points value
   - Verify updates

4. **Assign Person**:
   - Click dropdown
   - Select person
   - Verify owner name updates

5. **Reorder**:
   - Drag items in different order
   - Verify order persists

## Performance Optimization

- Use event delegation for handlers (as shown)
- Cache dropdown options after first load
- Use debouncing for frequent updates
- Implement pagination for large backlogs (future)

## Troubleshooting

**Items not saving?**
- Check browser console for errors
- Verify CSRF token is present
- Ensure user has "Project Manager" or "Administrator" role

**Dropdowns not loading?**
- Verify `OnGetItemTypes()`, `OnGetItemStatuses()` page handlers are working
- Check network tab for failed requests

**Page not loading?**
- Restart application (hot reload limitation with interface changes)
- Clear browser cache

