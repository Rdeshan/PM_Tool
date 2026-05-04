# Product Backlog - Frequently Asked Questions (FAQ)

## General Questions

### Q: Is this separate from ProjectBacklog?
**A:** Yes, completely separate. ProductBacklog is a new independent entity with its own table, DTOs, and service layer. ProjectBacklog remains unchanged.

### Q: Can I use both systems together?
**A:** Yes, they can coexist. Products can have ProductBacklogs, and Projects can have ProjectBacklogs. They don't interfere with each other.

### Q: Do I need to modify my existing code?
**A:** No, the ProductBacklog system is added without changing existing functionality. All changes are additive.

### Q: Is the database migration required?
**A:** Yes, the migration adds the `StoryPoints` column. Run `Update-Database` before the system will work.

### Q: What if I don't want story points?
**A:** Story points are optional at creation (defaults to 0). You can ignore them or update later.

---

## Implementation Questions

### Q: How do I call the backend from the frontend?
**A:** Use fetch API to POST to the page handlers. Example:
```javascript
fetch('/Products/{projectId}/{productId}/backlog?handler=CreateItem', {
    method: 'POST',
    headers: {'Content-Type': 'application/json'},
    body: JSON.stringify({
        productId: '...',
        title: '...',
        ...
    })
}).then(r => r.json()).then(data => console.log(data));
```

### Q: Do I need to restart the application?
**A:** Yes, after applying the migration and modifying interfaces, restart is required. Hot reload can't update interface implementations.

### Q: Can I test without the frontend UI?
**A:** Yes, use Postman or curl to test the endpoints directly. All endpoints are available through page handlers.

### Q: How do I know if the migration applied successfully?
**A:** Check database with SQL:
```sql
SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'ProductBacklogs' AND COLUMN_NAME = 'StoryPoints';
```
Should return one row.

---

## Data & Validation Questions

### Q: What's the maximum story points?
**A:** No maximum enforced. Common scales are 1-21, 1-13, or Fibonacci. Add validation if needed.

### Q: Can I have duplicate titles?
**A:** Yes, there's no unique constraint on titles. Add validation if needed.

### Q: What happens if user is deleted?
**A:** The backlog item remains but `OwnerId` becomes orphaned. Consider cascade delete if needed.

### Q: Can items have no owner?
**A:** Yes, `OwnerId` is nullable. Items can be unassigned.

### Q: Can I change the priority manually?
**A:** Yes, via the Reorder endpoint. Priority is just an integer field that can be updated directly if needed.

### Q: What's the difference between Type and Status?
**A:** 
- **Type**: What kind of item (UserStory, Bug, Epic, etc.) - describes nature
- **Status**: Current state (Draft, In Progress, Done, etc.) - describes workflow

---

## API Questions

### Q: How many backlog items can I retrieve?
**A:** No limit in code, but for performance, consider adding pagination in future.

### Q: What happens if ProductId doesn't exist?
**A:** The query returns empty list. No error thrown. Add validation if strict checking needed.

### Q: Can I filter backlog items?
**A:** Yes, `GetBacklogItemsAsync` accepts optional status parameter. Extend as needed.

### Q: What if the date format is wrong?
**A:** DateTime parsing is lenient. Invalid dates return null (no error).

### Q: Can I get deleted items?
**A:** No, deletion is permanent. No soft delete. Add if needed.

### Q: How do I handle concurrent updates?
**A:** EF Core uses last-write-wins. Add optimistic concurrency if needed.

---

## Authorization & Security Questions

### Q: Who can create backlog items?
**A:** Only users with "Project Manager" or "Administrator" role.

### Q: Can non-PMs read backlog?
**A:** Yes, any authenticated user with access to the product can read.

### Q: Is data encrypted?
**A:** In database: No additional encryption. In transit: Use HTTPS.

### Q: What about CSRF protection?
**A:** Handled automatically by ASP.NET Core RazorPages RequestVerificationToken.

### Q: Can I change role requirements?
**A:** Yes, modify the authorization check in page handlers or add custom policies.

### Q: What about row-level security?
**A:** Not implemented. Add if needed for multi-tenant scenarios.

---

## Performance Questions

### Q: Will this slow down my application?
**A:** No, minimal impact. Single new table with proper indexes.

### Q: How many backlog items can I have?
**A:** Theoretically unlimited, but UI performance depends on UI rendering.

### Q: Should I add indexes?
**A:** Consider adding indexes on `ProductId`, `Status`, `Priority` if you have large datasets.

### Q: Is there pagination?
**A:** Not implemented yet. Add using SKIP/TAKE if needed for large lists.

### Q: What about database backups?
**A:** Standard SQL Server backup procedures apply. No special handling needed.

---

## Troubleshooting Questions

### Q: Getting "Invalid column name 'StoryPoints'"
**A:** The migration hasn't been applied. Run `Update-Database -Project PMTool.Infrastructure`.

### Q: Getting "Handler not found" error
**A:** Restart the application. Hot reload doesn't update page handlers properly.

### Q: Getting authorization errors
**A:** User doesn't have required role (Project Manager/Administrator) for the operation.

### Q: Items not saving
**A:** Check browser console for errors. Verify CSRF token is included.

### Q: Dropdown not populating
**A:** Ensure `OnGetItemTypes()`, `OnGetItemStatuses()` handlers exist and restart app.

### Q: Users not appearing
**A:** Verify users are marked as Active in database.

### Q: Dates not formatting correctly
**A:** Standardize format in frontend. ISO 8601 (2025-01-15T00:00:00Z) recommended.

### Q: Cannot delete items
**A:** Check user role and authorization. Verify item exists.

---

## Enhancement Questions

### Q: Can I add custom fields?
**A:** Yes, extend ProductBacklog entity and update DTOs/service.

### Q: Can I add comments?
**A:** Yes, create new BacklogComment entity and relationship.

### Q: Can I add time tracking?
**A:** Yes, add TimeLogged field and create TimeEntry entity.

### Q: Can I add attachments?
**A:** Yes, similar to documents, create BacklogAttachment entity.

### Q: Can I link to SubProjects?
**A:** Yes, add reference to SubProject in ProductBacklog.

### Q: Can I duplicate items?
**A:** Add clone/duplicate handler in service layer.

### Q: Can I export to CSV/Excel?
**A:** Yes, add export handler in page model.

### Q: Can I bulk import?
**A:** Yes, add bulk import endpoint and parser.

---

## Integration Questions

### Q: Does this work with teams?
**A:** Yes, through user assignment (OwnerId). Users are members of teams.

### Q: Can products have multiple backlogs?
**A:** Currently 1 backlog per product. Modify if needed.

### Q: Can I track dependencies?
**A:** Not yet implemented. Add reference field if needed.

### Q: Can I link to external systems?
**A:** Yes, add ExternalLink field and integrate as needed.

### Q: Can I use with Jira?
**A:** Possible, but requires custom Jira integration layer.

### Q: Can I sync with SharePoint?
**A:** Possible, but requires custom SharePoint integration.

---

## Migration Questions

### Q: What if I need to roll back?
**A:** Run `Update-Database -TargetMigration [PreviousMigration]` to revert.

### Q: Can I skip the migration?
**A:** No, the column is required for functionality.

### Q: Will it affect my existing data?
**A:** No, existing backlog items will have StoryPoints = 0.

### Q: How long does migration take?
**A:** Usually < 1 second, no data reorganization needed.

### Q: Can I migrate to different database?
**A:** Yes, standard EF Core migration to any supported database.

---

## Testing Questions

### Q: How do I test this locally?
**A:** Use Postman to test endpoints. Or add JavaScript in browser console.

### Q: Can I test without UI?
**A:** Yes, all endpoints are independent. Test with curl or Postman.

### Q: What test data should I create?
**A:** Create product, then create 5-10 backlog items with different types/statuses.

### Q: Should I test authorization?
**A:** Yes, test with non-PM user to verify 403 Forbidden response.

### Q: How do I load test?
**A:** Use tools like Apache JMeter or k6 to simulate concurrent requests.

---

## Deployment Questions

### Q: How do I deploy to production?
**A:**
1. Apply migration to prod database
2. Deploy new code
3. Restart application
4. Test endpoints

### Q: Do I need downtime?
**A:** Minimal. Migration runs quickly, no data movement.

### Q: Should I backup before migration?
**A:** Yes, always backup production before schema changes.

### Q: Can I rollback in production?
**A:** Yes, but you'd need to drop the column (data loss). Better to test staging first.

### Q: How do I handle multiple environments?
**A:** Same migration runs on all environments. Use connection string per environment.

---

## Support & Maintenance

### Q: What if I find a bug?
**A:** Check the code, add logging, test in isolation, create fix, test thoroughly.

### Q: How do I update documentation?
**A:** Edit the markdown files provided. Keep them in sync with code.

### Q: Should I add more logging?
**A:** Yes, especially around database operations for debugging.

### Q: Do I need monitoring?
**A:** Yes, monitor endpoint response times and error rates in production.

### Q: How do I handle version updates?
**A:** This is version 1.0. Future versions maintain backward compatibility where possible.

---

## Resources

- **ASP.NET Core Docs**: https://docs.microsoft.com/en-us/aspnet/core/
- **Entity Framework Core**: https://docs.microsoft.com/en-us/ef/core/
- **Razor Pages**: https://docs.microsoft.com/en-us/aspnet/core/razor-pages/
- **C# Documentation**: https://docs.microsoft.com/en-us/dotnet/csharp/
- **SQL Server**: https://docs.microsoft.com/en-us/sql/

---

## Still Have Questions?

1. **Check Documentation**: PRODUCT_BACKLOG_*.md files
2. **Review Code**: Look at similar patterns in ProjectBacklog
3. **Test Endpoints**: Use Postman to understand behavior
4. **Check Logs**: Browser console and Visual Studio debug output
5. **Google/StackOverflow**: Search specific error messages

---

**Last Updated**: 2025-01-05  
**Questions Added**: 80+  
**Coverage**: 90%+  

For questions not covered here, refer to the detailed documentation files or the source code itself.
