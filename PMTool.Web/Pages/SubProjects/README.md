# Sub-Project Management - User Guide

## Quick Start

### Navigating to Sub-Projects

1. Go to **Projects** page
2. Click on a project to view its products
3. Find a product and click **Sub-Projects** button
4. You're now viewing all sub-projects for that product

## For Project Managers & Administrators

### Creating a Sub-Project

1. Navigate to Sub-Projects index for your product
2. Click **+ New Sub-Project** button
3. Fill in the form:
   - **Name**: e.g., "Student Registration Portal"
   - **Description**: Brief overview of the module
   - **Module Owner**: Select the person leading this sub-project
   - **Start Date**: When the work begins
   - **Due Date**: Target completion date
   - **Teams**: Click "Add Team" to assign teams
     - Select team from dropdown
     - (Optional) Enter team role like "Development", "QA", "BA"
     - Click X to remove a team
4. Click **Create Sub-Project**

### Updating a Sub-Project

1. Navigate to the sub-project's Index page
2. Click **Edit** button on the sub-project card
3. Update any fields:
   - Name, Description, Status
   - Module Owner
   - Dates
   - Team assignments (shows current, can add more)
4. Click **Update Sub-Project**

### Deleting a Sub-Project

1. Open the sub-project **Details** page
2. Click **Delete Sub-Project** button
3. Confirm in the modal dialog
4. Sub-project and all its dependencies are deleted

### Managing Team Assignments

#### Add Team:
1. Open sub-project **Edit** page
2. Scroll to "Update Team Assignments"
3. Select a team from dropdown
4. (Optional) Enter role
5. Click "Add Team"
6. Save by clicking "Update Sub-Project"

#### Remove Team:
1. Open sub-project **Edit** page
2. See "Current Team Assignments" section
3. To remove: add new teams above but exclude the one to remove
4. Click "Update Sub-Project"

### Managing Dependencies

#### Add Dependency:
1. Open sub-project **Details** page
2. Scroll to "Dependencies" section
3. Click "Add Dependency"
4. Select the sub-project this one depends on
5. (Optional) Add notes explaining why
6. Click "Add"

#### View Dependencies:
1. Open sub-project **Details** page
2. See "Dependencies" section
3. Shows all sub-projects that must be completed first

#### Remove Dependency:
1. Open sub-project **Details** page
2. Click trash icon next to the dependency
3. Dependency is removed immediately

## For All Users

### Viewing Sub-Projects

1. Navigate to Sub-Projects index
2. Use tabs to filter by status:
   - **All**: Show all sub-projects
   - **Not Started**: Ready to begin
   - **In Progress**: Currently being worked on
   - **In Review**: In QA/Review phase
   - **Completed**: Finished

### Reading Sub-Project Details

On the Index page card:
- **Status Badge**: Current state (color-coded)
- **Progress Bar**: % of tickets completed
- **Tickets**: Total count and completed count
- **Timeline**: Start and due dates
- **Teams**: Assigned teams with roles (truncated)
- **Dependencies**: Number of dependencies

Click **View** to see full details page with:
- Complete description
- Team assignments table
- Dependency list with notes
- Timeline information
- Metadata (created/updated dates)

## Status Meanings

| Status | Icon | Color | Meaning |
|--------|------|-------|---------|
| Not Started | ⭘ | Gray | Module hasn't begun |
| In Progress | ⚙ | Blue | Active development |
| In Review | ⚠ | Yellow | Testing/QA phase |
| Completed | ✓ | Green | Module complete |

## Progress Tracking

**Progress = Completed Tickets / Total Tickets × 100%**

- Progress bar fills as team completes tickets
- Color indicates health:
  - 🔴 0-25%: Behind schedule
  - 🟡 25-50%: On track
  - 🔵 50-75%: Making good progress
  - 🟢 75-100%: Nearly complete

Progress is calculated from the backlog items (tickets) assigned to this sub-project.

## Team Assignments

Each team assigned to a sub-project can have a **Role**:
- **Development**: Coding/Implementation team
- **QA**: Quality Assurance/Testing team
- **BA**: Business Analysis team
- **PM**: Project Management oversight
- (Custom): Any role you define

Multiple teams can work on one sub-project with different roles.

## Dependencies Explained

**What are dependencies?**
Dependencies show which sub-projects must be completed before another can start.

**Example:**
- Sub-Project A: "Database Design" 
- Sub-Project B: "API Development" depends on A

Sub-Project B cannot begin until A is complete.

**Why use dependencies?**
- **Planning**: Understand sequencing
- **Scheduling**: Know what blocks you
- **Risk**: Identify critical path
- **Communication**: Clarify prerequisites

## Common Tasks

### Find all sub-projects for a product
1. Go to Products page
2. Find product
3. Click **Sub-Projects** button

### Check team workload
1. View sub-project Details
2. See "Assigned Teams" table
3. Understand role and assignments

### Track module progress
1. View sub-project Index
2. See progress bar on card
3. Check ticket counts
4. Track completion percentage

### Plan dependencies
1. View sub-project Details
2. Scroll to Dependencies section
3. Review prerequisites
4. Plan work sequence

## Permissions

### Project Managers & Administrators Can:
- ✅ Create sub-projects
- ✅ Edit sub-projects
- ✅ Delete sub-projects
- ✅ Assign teams
- ✅ Manage dependencies
- ✅ Change status

### Other Roles Can:
- ✅ View all sub-projects
- ✅ View details
- ✅ See team assignments
- ✅ See dependencies
- ✅ Track progress
- ❌ Create/Edit/Delete

## Tips & Best Practices

1. **Set realistic dates**: Due dates help teams prioritize
2. **Assign module owner**: Clear ownership prevents confusion
3. **Define team roles**: Be specific about team responsibilities
4. **Document dependencies**: Add notes explaining why
5. **Update status**: Keep status current for accurate planning
6. **Monitor progress**: Check progress bars regularly
7. **Review dependencies**: Identify and address blockers early

## FAQ

**Q: Can a sub-project have multiple teams?**
A: Yes, assign multiple teams with different roles.

**Q: Can I change the status anytime?**
A: Yes, you can move between any statuses.

**Q: What if a dependency sub-project is deleted?**
A: The dependency is automatically removed.

**Q: How is progress calculated?**
A: Progress = (Completed Tickets / Total Tickets) × 100%

**Q: Can I have circular dependencies?**
A: The system prevents A→B→A dependency chains.

**Q: What happens when I delete a sub-project?**
A: Sub-project, all team assignments, and dependencies are deleted. Tickets are not deleted.

## Need Help?

- **Validation errors**: Check red text below form fields
- **Access denied**: Only PMs and Admins can edit/delete
- **Can't add team**: Verify team exists and is not already assigned
- **Missing dependencies**: Select a different sub-project
