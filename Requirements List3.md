**Software Component**

**Project Management Tool**

**Feature Requirements List**

*Version 2.2 | February 2026*

|  |  |
| --- | --- |
| **Document Title** | Software Component PM Tool — Feature Requirements List |
| **Version** | 2.2 |
| **Status** | Draft — For Review |
| **Date** | February 2026 |
| **Primary Audience** | Software Developers, Project Managers |
| **Changes in v2.2** | Added: Delay Tracking (Section 14) — per-ticket and project-wide delay visibility, delay reports, and escalation rules. Added: Email Notification Settings (Section 15) — configurable recipient rules per event type, role-based routing, and per-project notification settings. |

# 1. System Hierarchy Overview

*The system is organised in a four-level hierarchy. All features described in this document operate within this structure.*

| **Level** | **Name** | **Description & Example** |
| --- | --- | --- |
| **Level 1** | **Project** | Top-level container representing a client engagement or system. e.g. 'University Management System' |
| **Level 2** | **Product** | A release version within a project. e.g. 'UMS v1.0', 'UMS v2.0' |
| **Level 3** | **Sub-Project** | A functional module within a product. e.g. 'Student Registration Portal', 'Admin Portal' |
| **Level 4** | **Ticket** | An individual unit of work within a sub-project. e.g. a bug, task, feature, or improvement. |

# 2. User Management & Access Control

*Internal team members only. Customers do not require a system login — their interactions are handled via email (see Section 8).*

## 2.1 Authentication

* **Email & password login —** team members log in with a verified email address and password.

![](data:image/png;base64...)

* **OAuth single sign-on —** login via Google or Microsoft accounts supported as an alternative.
* **Multi-factor authentication (MFA) —** users can enable MFA via authenticator app; administrators can enforce it organisation-wide.

![](data:image/png;base64...)

* **Session timeout —** sessions expire automatically after a configurable period of inactivity (default: 30 minutes).

![](data:image/png;base64...)

* **Password reset —** self-service password reset via a time-limited email link.

![](data:image/png;base64...)

* **Account lockout —** accounts are temporarily locked after 10 consecutive failed login attempts within 15 minutes.

![](data:image/png;base64...)

## 2.2 Roles & Permissions

*Permissions are role-based. Each user can hold different roles on different projects simultaneously.*

* **Administrator —** full system access; manages users, projects, and organisation-wide settings.
* **Project Manager —** creates and manages projects, products, sub-projects, sprints, milestones, and team assignments.
* **Developer —** creates and updates tickets, logs time, and posts comments within assigned sub-projects.
* **QA Engineer —** creates and manages bug tickets, links test cases, and updates ticket status within assigned sub-projects.
* **Business Analyst (BA) —** creates and manages BRDs, user stories, and backlog items; can link requirements to tickets.
* **Viewer / Stakeholder —** read-only access to dashboards, reports, and ticket lists; cannot create or edit any content.
* **Guest —** limited read-only access to a specific project via a shared invite link; no system account required.

## 2.3 User Administration

* **Invite by email —** administrators invite users by email; invited users receive an account setup link.
* **Deactivate accounts —** administrators can deactivate users; their historical data is preserved.
* **Role reassignment —** a user's role on any project can be changed at any time by an administrator or project manager.
* **Profile settings —** users can update their display name, avatar, and notification preferences.
* **Team grouping —** users are grouped into named teams (e.g. Development, QA, BA, DevOps, Design) which can be assigned to sub-projects collectively and used to filter ticket views.

# 3. Projects

*The top-level container for all work, typically representing a client engagement or a system being built.*

## 3.1 Project Creation & Settings

* **Create project —** any Project Manager or Administrator can create a project with a name, description, client name, start date, and expected end date.

![](data:image/png;base64...)

* **Project code —** each project is assigned a short unique code (e.g. 'UMS') used as the prefix for all ticket IDs. e.g. UMS-001.

![](data:image/png;base64...)

* **Project status —** projects can be marked as Active, On Hold, or Completed.

![](data:image/png;base64...)

* **Project avatar & colour —** each project can have a custom icon and colour for quick identification in navigation.

![](data:image/png;base64...)

* **Project archival —** completed projects can be archived; archived projects are read-only but remain fully searchable.

![](data:image/png;base64...)

* **Team assignment —** teams and individual users are assigned to a project, controlling who can access it.

![](data:image/png;base64...)

* **Project-level backlog —** each project has a top-level backlog for BRDs, user stories, and high-level requirements (see Section 6).

# 4. Products (Release Versions)

*A Product represents a specific release version within a project. A single project can contain multiple products for different delivery milestones or version cycles.*

## 4.1 Product Creation & Management

* **Create product —** project managers create a product within a project with a version name, description, planned release date, and release type (Major, Minor, Patch, Hotfix).

![](data:image/png;base64...)

* **Product status —** products can be set to Planned, In Development, In Testing, Released, or Deprecated.
* **Multiple products —** a project can contain any number of products, each managed independently.
* **Product backlog —** each product has its own backlog for BRDs, user stories, use cases, and scope documents (see Section 6).
* **Release notes —** project managers can draft and publish release notes against a product, linked to all completed tickets in that release.
* **Product timeline —** a product-level view shows all its sub-projects and tickets plotted on a timeline.

# 5. Sub-Projects (Modules)

*Sub-projects represent functional modules within a product. e.g. under 'UMS v1.0': Student Registration Portal, Admin Portal, Finance Module.*

## 5.1 Sub-Project Creation & Management

* **Create sub-project —** project managers create a sub-project within a product with a name, description, assigned team(s), and module owner.
* **Sub-project status —** sub-projects can be set to Not Started, In Progress, In Review, or Completed.
* **Team assignment —** one or more teams (Development, QA, BA, etc.) are assigned to a sub-project.
* **Sub-project progress —** real-time progress indicator based on the ratio of completed to total tickets.
* **Sub-project timeline —** timeline view of all tickets within the sub-project plotted against start and due dates.
* **Dependencies between sub-projects —** a sub-project can be marked as dependent on another, surfacing sequencing constraints in planning views.

# 6. Backlog & Requirements Management

*Each project and product has a dedicated backlog for capturing and organising requirements documentation.*

## 6.1 Backlog Structure

* **Project-level backlog —** shared space at the project level for requirements spanning all products.

![](data:image/png;base64...)

* **Product-level backlog —** dedicated backlog per product for release-specific requirements and documents.
* **Backlog item fields —** title, description, type (BRD, User Story, Use Case, Epic, Change Request), priority, status (Draft, Approved, In Progress, Done), and owner.
* **Priority ordering —** backlog items can be reordered by drag-and-drop to reflect current priority.

## 6.2 Requirements Documentation

* **BRD attachment —** Business Requirements Documents can be uploaded as files or written in the built-in rich-text editor and linked to a backlog item.
* **User story creation —** user stories written in standard format with acceptance criteria listed beneath each story.
* **Use case attachment —** use case documents can be uploaded or linked to backlog items and referenced when creating tickets.
* **Version history —** all edits to backlog documents are versioned; previous versions can be viewed and restored.
* **Approval workflow —** BRDs and user stories can be submitted for approval; approvers receive a notification and can approve, reject, or request changes.
* **Traceability linking —** backlog items can be linked to tickets to enable RTM generation (see Section 11).

# 7. Tickets

*Tickets are the fundamental unit of work in the system. Every piece of work — bug, task, feature, improvement — is a ticket within a sub-project.*

## 7.1 Ticket Categories

* **Task —** a general unit of work. e.g. 'Set up the database schema'.
* **Bug —** a defect or unintended behaviour found during development or testing, or reported by a customer via email.
* **Feature —** a new piece of functionality being added.
* **Improvement —** an enhancement to an existing feature or process.
* **Change Request —** a formally requested scope change, typically from a client.
* **User Story —** a ticket linked directly to a user story in the backlog.
* **Test Case —** a specific test scenario to be executed by the QA team.

## 7.2 Ticket Status

*Ticket statuses reflect the full lifecycle of a work item from initial creation through to closure. Statuses are configurable per project; the following are the system defaults.*

| **Status** | **Description** | **Typical Owner** |
| --- | --- | --- |
| **Open** | Ticket has been created but work has not yet been assigned or scheduled. | *PM / BA* |
| **Not Started** | Ticket is assigned and scheduled but work has not yet begun. | *Developer / QA* |
| **Implementing** | Active development is currently underway on this ticket. | *Developer* |
| **Paused** | Work has been temporarily stopped, e.g. due to a blocker or dependency. A pause reason must be entered. | *Developer / PM* |
| **In Review** | Implementation is complete and the ticket is awaiting code or peer review. | *Developer / PM* |
| **QA** | Ticket has passed review and is currently being tested by the QA team. | *QA Engineer* |
| **UAT** | Ticket is in User Acceptance Testing with the client or business stakeholder. | *BA / PM* |
| **Completed** | All work, review, and testing is done. Ticket is ready to be included in a release. | *PM* |
| **Closed** | Ticket has been formally closed and included in a release, or marked as resolved with no further action needed. | *PM* |
| **Cancelled** | Ticket has been cancelled and will not be actioned. A cancellation reason must be entered. | *PM* |
| **Reopened** | A previously completed or closed ticket has been reopened, e.g. due to a regression or incomplete fix. | *QA / PM* |

* **Custom statuses —** project managers can add, rename, reorder, or remove statuses to match the team's actual workflow.
* **Workflow transition rules —** rules can be set to control which transitions are permitted and by whom (e.g. only a QA Engineer can move a ticket from In Review to QA).
* **Status history —** every status change is recorded in an immutable audit trail with the user and timestamp.
* **Reopen with reason —** reopening a closed or completed ticket requires the user to enter a reason, which is logged in the activity trail.
* **Pause with reason —** pausing a ticket requires the user to select or enter a pause reason (e.g. Blocked by Dependency, Awaiting Client Input, On Hold by PM).
* **Bulk status update —** multiple tickets can have their status changed simultaneously from list or board view.

## 7.3 Ticket Fields

*The following fields are available when creating or editing a ticket.*

| **Field** | **Description** | **Required / Optional** |
| --- | --- | --- |
| **Ticket ID** | Auto-generated unique ID using project code + sequence number. e.g. UMS-042 | *Auto-generated* |
| **Title** | A concise summary of the ticket. | Required |
| **Description** | Rich-text field: text, headings, images, code blocks, tables, and lists. | Required |
| **Category** | Task, Bug, Feature, Improvement, Change Request, User Story, or Test Case. | Required |
| **Sub-Project** | The module this ticket belongs to. | Required |
| **Priority** | Critical, High, Medium, or Low. | Required |
| **Status** | Current lifecycle stage (see Section 7.2 for full status list). | Required |
| **Assignee(s)** | One or more team members responsible for the ticket. | Required |
| **Team** | Development, QA, BA, DevOps, Design, or custom team. | Required |
| **Reporter** | The user who created the ticket. Auto-populated but editable. | *Auto-populated* |
| **Start Date** | The date work on this ticket is expected to begin. | Optional |
| **Expected Due Date** | The planned completion date. | Optional |
| **Actual End Date** | The date the ticket was actually completed. Auto-set when status changes to Completed. | Optional |
| **Story Points / Effort** | Effort estimate in story points or hours. | Optional |
| **Sprint** | The sprint this ticket is assigned to. | Optional |
| **Milestone** | The milestone this ticket contributes to. | Optional |
| **BRD Number** | Reference to the BRD this ticket fulfils. | Optional |
| **Use Case Number** | Reference to the use case this ticket implements. | Optional |
| **Test Case Number** | Reference to the associated test case(s). | Optional |
| **Labels / Tags** | Free-form tags for cross-cutting categorisation. | Optional |
| **Linked Tickets** | Relationship to other tickets: blocks, is blocked by, relates to, duplicates. | Optional |
| **Attachments** | Files, documents, images, and external links (see Section 7.5). | Optional |
| **Time Logged** | Cumulative time logged against this ticket by all assignees. | *Auto-calculated* |
| **Severity** | For Bug tickets: Critical, Major, Minor, or Trivial. | Required for Bugs |
| **Steps to Reproduce** | For Bug tickets: step-by-step instructions to reproduce the issue. | Required for Bugs |
| **Expected vs Actual** | For Bug tickets: what was expected vs. what actually happened. | Required for Bugs |
| **Environment** | For Bug tickets: Dev, QA, UAT, Staging, or Production. | Required for Bugs |
| **Pause / Cancel Reason** | Required when setting status to Paused or Cancelled. | *Conditional* |

## 7.4 Ticket Views

*The system provides multiple ways for individuals and teams to view and manage their tickets.*

**Personal Views (Per User)**

* **My Tickets —** a personal dashboard showing every ticket currently assigned to the logged-in user, across all projects and sub-projects. This is the default landing view after login.
* **Filter options —** filterable by project, sub-project, status, priority, category, and due date.
* **Sort options —** sortable by priority, due date, last updated, and story points.
* **Quick filters —** one-click presets: 'My Open Tickets', 'Due This Week', 'Overdue', 'Paused', 'Recently Updated'.
* **Status grouping —** tickets can be grouped by status so the user sees all their 'Implementing', 'QA', etc. tickets at a glance.
* **My Activity —** a chronological log of all actions the logged-in user has taken or received across all tickets: assignments, comments, status changes, and mentions.
* **Watching —** a view of all tickets the user has subscribed to as a watcher, regardless of assignment.

**Team Views (Per Team)**

* **Team Board —** a Kanban board showing all tickets assigned to members of a specific team (e.g. the QA board shows all tickets currently assigned to any QA Engineer), across all sub-projects they are assigned to.
* **QA Board —** shows all tickets in QA status or assigned to the QA team, making it easy for QA leads to manage the testing queue.
* **Development Board —** shows all tickets assigned to the Development team, grouped by sub-project or assignee.
* **BA Board —** shows all backlog items, BRDs, and tickets assigned to Business Analysts.
* **Any custom team —** team boards are automatically available for every named team in the system.
* **Team List View —** a flat list view equivalent of the team board, sortable and filterable by all ticket fields.
* **Team Workload Summary —** a chart showing how many open tickets and story points are assigned to each member of the team, helping team leads identify over- or under-allocation.
* **Team filter on all views —** any project-level or sub-project-level view can be filtered by team, allowing project managers to see only the tickets relevant to a specific team.

**Project & Sub-Project Views**

* **Sub-project board —** Kanban board showing all tickets within a single sub-project, organised by status columns.
* **Sub-project list —** flat sortable list of all tickets in a sub-project with inline editing.
* **Sub-project table —** spreadsheet-style grid view with all ticket fields as columns; columns can be shown, hidden, and reordered.
* **Sub-project timeline —** Gantt-style view of all tickets in the sub-project plotted by start and due date.
* **Project overview —** top-level summary of all products and sub-projects within a project, showing status and progress at a glance.

## 7.5 Ticket Attachments

*Every ticket supports multiple types of attachments. All attachments are version-controlled — if a file is re-uploaded, the previous version is retained and accessible.*

| **Attachment Type** | **Description** | **Supported Formats / Limits** |
| --- | --- | --- |
| **File Upload** | Any file can be uploaded directly to a ticket. Files are stored securely and downloadable by all users with access to the ticket. | *Any file type. Max 25 MB per file. Unlimited files per ticket.* |
| **Document** | Word, PDF, Excel, and PowerPoint documents uploaded as attachments. Documents are previewed inline where possible (PDF, images). | *PDF, DOCX, XLSX, PPTX, TXT. Max 25 MB per file.* |
| **Screenshot / Image** | Images attached to a ticket are displayed as inline thumbnails that can be enlarged. Screenshots can also be pasted directly from the clipboard into the description or comment fields. | *PNG, JPG, GIF, WEBP. Max 10 MB per image. Inline paste supported.* |
| **External Link** | A URL can be attached to a ticket with an optional display label. The system shows a preview card (title, favicon) where available. Links can point to any external resource: Confluence pages, Google Docs, Figma designs, SharePoint files, etc. | *Any valid URL. Unlimited links per ticket.* |
| **Inline in Description** | Images and links can be embedded directly within the ticket description or comment body using the rich-text editor, in addition to being listed as formal attachments. | *Same limits as above.* |

* **Attachment list —** all attachments on a ticket are listed in a dedicated Attachments section on the ticket detail view, showing file name, type, uploader, upload date, and file size.
* **Attachment on comments —** files, images, and links can also be attached to individual comments, not just the ticket itself.
* **Version history —** if a file with the same name is re-uploaded to a ticket, the previous version is preserved and labelled; users can download any version.
* **Attachment search —** the global search includes attachment file names, so team members can locate a ticket by searching for a document name.

## 7.6 Ticket Search & Filtering

* **Global search —** searches all ticket titles, descriptions, comments, and attachment file names across all accessible projects; results within 1 second.
* **Filter by —** sub-project, category, priority, status, assignee, team, sprint, milestone, BRD number, use case number, test case number, label, date range.
* **Saved filters —** any filter combination can be saved as a named view and shared with team members.
* **Quick filters —** one-click presets: 'My Open Tickets', 'Due This Week', 'Overdue', 'High Priority', 'Unassigned', 'Paused'.

## 7.7 Ticket Comments & Collaboration

* **Threaded comments —** all team members can post comments; comments support markdown, code blocks, inline images, and @mentions.
* **@mention notifications —** mentioning a user sends them an immediate in-app and email notification.
* **Emoji reactions —** reactions can be added to comments for lightweight acknowledgement.
* **Activity log —** every change (field edits, status changes, comments, attachments) is recorded chronologically and permanently.
* **Watchers —** any user can subscribe to a ticket to receive notifications without being an assignee.
* **Internal notes —** a separate internal comment thread visible only to team members, not visible to customers or guest viewers.

## 7.8 Ticket Automation

* **Auto-block on overdue dependency —** if a dependency ticket becomes overdue, the dependent ticket is flagged as blocked and both assignees are notified.
* **Due date reminders —** automated notifications sent to the assignee 48 hours before a ticket's due date if not yet completed.
* **Auto-set Actual End Date —** when a ticket is moved to Completed status, the Actual End Date is automatically set to today's date (editable if needed).
* **Recurring tickets —** tickets can be set to recur on a daily, weekly, or monthly schedule, creating a new ticket instance on each cycle.

## 7.9 Bulk Operations

* **Multi-select —** multiple tickets selected simultaneously using checkboxes in list or table view.
* **Bulk actions available —** change status, change priority, reassign, add or remove label, move to sprint, return to backlog, archive, delete.

# 8. Customer Bug Reporting (Email-to-Ticket)

*Customers do not require a system login. Bugs are reported via a structured email template which automatically creates a ticket in the system.*

## 8.1 Email Template

* **Template distribution —** a standardised bug report template is provided to customers with clearly labelled fields to fill in before sending.
* **Template fields —** Subject/Title, Description, Steps to reproduce, Expected vs. actual behaviour, Environment (browser, device, app version), Severity, and any screenshot attachments.
* **Dedicated inbox —** each project has a unique intake email address (e.g. bugs.ums@company.com) to which customers send completed templates.

![](data:image/png;base64...)

* **Auto-reply —** the customer immediately receives a confirmation email with the generated ticket ID and a summary of the details received.

## 8.2 Automatic Ticket Creation

* **Email parsing —** the system parses the incoming email and maps template fields to the corresponding ticket fields automatically.
* **Default assignment —** email-created tickets are assigned to a configurable default assignee or team (e.g. QA team lead) for triage.
* **Auto-categorisation —** tickets created via email are automatically categorised as Bug with a status of Open.
* **Attachment handling —** file attachments in the email (e.g. screenshots) are automatically attached to the created ticket.
* **Duplicate detection —** incoming tickets that closely match an existing open bug are flagged for triage review before creating a duplicate.
* **Invalid format handling —** emails that do not follow the template are flagged for manual review; the sender receives a reply with instructions to resubmit.

## 8.3 Customer Communication

* **Status update emails —** customers receive automated email updates when their ticket moves to key statuses (In Progress, Resolved).
* **Resolution email —** on resolution, the customer receives a confirmation with the fix summary and the version in which it will be available.
* **No login required —** at no point is the customer required to create an account or log into the system.

# 9. Sprint Planning

*Available for projects using Scrum or hybrid methodology. Kanban projects use continuous flow without fixed sprints.*

## 9.1 Sprint Management

* **Create sprint —** project managers create a sprint with a name, goal, start date, end date, and story point capacity.
* **One active sprint —** only one sprint active per sub-project or product at a time; future sprints can be prepared in advance.
* **Populate sprint —** tickets moved from the backlog into the sprint via drag-and-drop with real-time capacity indicators.
* **Capacity warning —** warning displayed when planned story points exceed the sprint's configured capacity.
* **Per-member capacity —** individual availability adjustable per sprint for leave or split allocation.
* **Sprint goal display —** sprint goal shown prominently on the board throughout the sprint.
* **Mid-sprint changes —** tickets can be added or removed mid-sprint; all scope changes are logged.
* **Sprint closure —** on closure, project manager chooses disposition for incomplete tickets: carry over, return to backlog, or close.
* **Sprint summary —** auto-generated on closure: planned vs. completed points, completion rate, and carried-over items.
* **Sprint history —** all past sprints and their summaries permanently accessible.

# 10. Visualisations & Charts

![](data:image/png;base64...)![](data:image/png;base64...)

## 10.1 Gantt Chart

* **Ticket-level Gantt —** all tickets within a sub-project on a horizontal timeline by start and due date.
* **Sub-project Gantt —** all sub-projects within a product shown on a timeline with planned duration and overlap.
* **Product Gantt —** top-level view showing all products within a project across time.
* **Dependency lines —** dependency relationships shown as connecting lines on the chart.
* **Critical path —** the critical path is highlighted to show which tasks determine the minimum project duration.
* **Drag to reschedule —** ticket dates adjusted by dragging bars; dependent tickets shift accordingly.
* **Today marker —** vertical line indicating the current date.

![](data:image/png;base64...)

* **Gantt export —** exportable as PDF or PNG.

## 10.2 Kanban Board

![](data:image/png;base64...)

* **Board per sub-project and per team —** each sub-project and each team has its own Kanban board with columns mapped to ticket statuses.
* **Drag-and-drop —** tickets moved between status columns by dragging; status updated instantly.
* **WIP limits —** work-in-progress limits set per column; columns exceeding the limit are highlighted.
* **Swimlanes —** board groupable by assignee, team, priority, or epic.
* **Card customisation —** information shown on each ticket card (assignee, priority, due date, story points) is configurable.

## 10.3 Burndown Chart

* **Sprint burndown —** ideal vs. actual remaining story points per day within the active sprint; updates in real time.
* **Scope change markers —** markers where tickets were added or removed mid-sprint.
* **Archived burndowns —** completed sprint burndown charts permanently accessible from sprint history.

## 10.4 Velocity Chart

![](data:image/png;base64...)

* **Story points per sprint —** planned vs. completed story points for each of the last 10 sprints.
* **Rolling average —** rolling velocity average, configurable over the last 3, 5, or 10 sprints.

![](data:image/png;base64...)

* **Delivery forecast —** projected release date for remaining backlog scope based on current velocity.

## 10.5 Summary Charts

* **Ticket status distribution —** donut or bar chart showing tickets by status within a sub-project or product.
* **Ticket category breakdown —** proportion of tickets by category (Bug, Feature, Task, etc.).
* **Team workload chart —** story points or ticket count per team member in the active sprint or date range.
* **Milestone progress bar —** completion bar per milestone based on completed vs. total linked story points.
* **Bug trend chart —** bugs opened vs. closed over a configurable time period.

# 11. Outputs & Reports

## 11.1 Requirements Traceability Matrix (RTM)

* **Auto-generated RTM —** cross-references BRDs, user stories, use cases, and tickets automatically.
* **RTM columns —** Requirement ID, Requirement Description (from BRD), Use Case Number, User Story, Linked Ticket(s), Ticket Status, Test Case Number, Test Status.
* **Coverage gaps —** highlights requirements with no linked ticket and tickets with no linked requirement.
* **Filter by sub-project —** filterable to a specific sub-project or product.
* **Export RTM —** PDF or Microsoft Excel.

## 11.2 Dependency Matrix

* **Auto-generated matrix —** grid showing all ticket-to-ticket and sub-project-to-sub-project dependencies.
* **Blocked items —** currently blocked items highlighted in the matrix.
* **Filter options —** filterable by sub-project, team, or sprint.
* **Export —** PDF or Excel.

## 11.3 Costing & Budget Report

* **Developer rate configuration —** each user account can have an hourly or daily rate set by the administrator.
* **Time-based cost calculation —** cost per ticket calculated by multiplying time logged against assignee rates.
* **Cost by sub-project and team —** aggregated development cost per sub-project, product, project, and team.

![](data:image/png;base64...)

* **Budget vs. actual —** project managers set a budget per product or sub-project; report compares this against actual cost.
* **Effort summary —** total hours logged per ticket, sub-project, and team member.
* **Cost forecast —** projected total cost at completion based on remaining tickets and current logging rates.
* **Export —** PDF or Excel including summary table and per-ticket breakdown.

## 11.4 Additional Reports

* **Sprint report —** planned vs. completed story points, completion rate, and carried-over items for any sprint.
* **Bug report —** open vs. closed bugs over time, average time to resolution, and breakdown by severity and sub-project.
* **Workload report —** story points and hours assigned per team member across a selected date range.
* **Ticket age report —** open tickets ordered by age, highlighting long-standing unresolved items.

![](data:image/png;base64...)

* **Change request log —** formal log of all Change Request tickets: requester, date, status, and estimated impact.
* **Scheduled delivery —** any report scheduled for automated weekly or monthly email delivery.
* **Export formats —** PDF, CSV, or Microsoft Excel (.xlsx).

# 12. Notifications

## 12.1 In-App Notifications

* **Notification bell —** unread notification count in the navigation bar; clicking opens the notification panel.
* **Triggering events —** ticket assigned, comment posted, @mention, status changed, due date approaching (48 hours), ticket blocked, ticket paused, sprint started or closed, milestone at risk, approval request received.
* **Mark as read —** notifications individually or bulk-marked as read.

![](data:image/png;base64...)

## 12.2 Email Notifications

* **Email equivalents —** all in-app notification events also trigger emails by default.
* **User preferences —** users configure which events send emails and which appear in-app only; configurable per project.
* **Daily digest —** users can opt into a daily email digest summarising all activity on assigned and watched tickets.
* **Customer notifications —** automated emails to customers when their email-reported bug ticket is updated (see Section 8).

# 13. System & Technical Requirements

## 13.1 Performance

* **Page load —** boards, ticket lists, and dashboards load within 2 seconds for up to 1,000 tickets.
* **Search —** global search returns results within 1 second across up to 10,000 tickets.
* **Report generation —** RTM, dependency matrix, and costing reports generated within 5 seconds.
* **Concurrent users —** supports at least 200 concurrent users without performance degradation.

## 13.2 Security

* **Encryption in transit —** all client-server communication encrypted via TLS 1.2 or higher.
* **Encryption at rest —** all stored data encrypted using AES-256.
* **Role-based access —** all data access governed by user role; no data accessible without an authenticated session.
* **Audit log —** all user actions logged with identity, timestamp, and affected record; retained for 12 months.
* **OWASP compliance —** hardened against OWASP Top 10 vulnerability categories.

## 13.3 Reliability & Availability

* **Uptime —** 99.9% uptime target outside scheduled maintenance windows.
* **Backups —** automated database backups every 6 hours, retained for 30 days.
* **Recovery —** Recovery Time Objective of 4 hours; Recovery Point Objective of 6 hours.

## 13.4 Usability & Accessibility

* **WCAG 2.1 Level AA —** the application meets web accessibility guidelines.
* **Keyboard navigation —** all core features accessible without a mouse.
* **Responsive design —** fully functional on screens 768px wide and above.
* **Browser support —** latest two major versions of Chrome, Firefox, Edge, and Safari.

## 13.5 API & Integration

* **RESTful API —** all core features accessible via a documented RESTful API (OpenAPI 3.0 specification).
* **API versioning —** API versions maintained for a minimum of 12 months after a superseding version is released.
* **Webhook support —** system can POST event notifications to user-configured webhook URLs for lightweight third-party integration.

# 14. Delay Tracking

*The system must provide proactive visibility into delays at every level of the hierarchy — from an individual ticket through to the full project. Delays should surface automatically without requiring manual investigation.*

## 14.1 What Constitutes a Delay

| **Delay Type** | **Definition** |
| --- | --- |
| **Overdue Ticket** | A ticket whose Expected Due Date has passed and whose status is not Completed, Closed, or Cancelled. |
| **Not Started — Late** | A ticket whose Start Date has passed but whose status is still Not Started or Open. |
| **Paused — Extended** | A ticket that has been in Paused status for longer than a configurable threshold (default: 3 business days). |
| **Sprint Overrun** | A sprint where the projected completion (based on remaining points and current velocity) exceeds the sprint end date. |
| **Milestone At Risk** | A milestone where the projected completion date based on current velocity exceeds its target date. |
| **Sub-Project Delayed** | A sub-project where one or more critical-path tickets are overdue, causing the overall sub-project completion date to slip. |
| **Blocked — Unresolved** | A ticket that has been in Blocked status (Paused due to dependency) for longer than a configurable threshold (default: 2 business days) without the blocking ticket being resolved. |

## 14.2 Delay Indicators on Tickets

* **Overdue badge —** tickets that have passed their Expected Due Date are automatically flagged with a visible 'Overdue' badge on all views (Kanban card, list row, table row, and ticket detail).
* **Days overdue counter —** the ticket detail view and list views display how many calendar days the ticket is overdue, updating daily.
* **Start date missed indicator —** tickets whose Start Date has passed without the status leaving Not Started are flagged with a 'Not Started — Late' label.
* **Delay reason field —** when a ticket is identified as delayed, the assignee or project manager can log a delay reason explaining the cause (e.g. 'Awaiting client sign-off', 'Blocked by UMS-034', 'Scope change under review').
* **Delay history —** every delay event (when it started, how long it lasted, and the reason logged) is permanently recorded in the ticket's activity log.
* **Revised due date —** project managers can set a Revised Due Date on an overdue ticket; the system then tracks against the revised date while retaining the original for reporting.

## 14.3 Delay Views & Dashboards

* **Delays dashboard —** a dedicated project-level view listing all currently delayed tickets, sub-projects, and milestones in one place, filterable by delay type, team, sub-project, and assignee.
* **Overdue tickets list —** a filtered view within every project, sub-project, team board, and My Tickets view showing only overdue items, sorted by days overdue (most overdue first).
* **At-risk highlights on Gantt —** overdue tickets and milestones are highlighted in red on the Gantt chart; tickets approaching their due date within 2 days are shown in amber.
* **Sprint delay indicator —** the active sprint board shows a clear banner when the sprint is projected to overrun its end date, with the projected overrun displayed in days.
* **Milestone delay indicator —** milestones flagged as At Risk or Overdue are shown with a distinct colour (amber for At Risk, red for Overdue) on both the roadmap and the milestone list.
* **Sub-project delay summary —** the sub-project overview card on the product page shows a delay indicator if any critical-path tickets within it are overdue.

## 14.4 Delay Reports

* **Delay report —** a dedicated report listing all tickets that were delayed at any point within a selected date range, showing: ticket ID, title, sub-project, assignee, original due date, revised due date (if set), days delayed, and delay reason.
* **Delay frequency by team —** a chart showing how many delay events occurred per team over a selected period, helping identify systemic issues.
* **Average delay duration —** the average number of days tickets are delayed per sub-project, team, and assignee; useful for improving estimation accuracy over time.
* **On-time delivery rate —** percentage of tickets completed on or before their Expected Due Date per sprint, sub-project, and team; displayed as a trend over time.
* **Export delay report —** exportable as PDF, CSV, or Excel.

## 14.5 Delay Escalation Rules

*Escalation rules define who is automatically notified when a delay condition is detected and for how long it has persisted. These are configured in Notification Settings (see Section 15).*

* **Immediate escalation —** as soon as a ticket becomes overdue (Expected Due Date passes with status not Completed), a notification is sent to the assignee and the configured escalation recipients.
* **Progressive escalation —** if an overdue ticket is not resolved within a configurable number of days (e.g. 3 days), a second-level escalation notification is sent to a higher-tier recipient (e.g. Project Manager or Director).
* **Repeated reminders —** recurring escalation emails can be configured to send every N days until the ticket is resolved or marked with a revised due date.
* **Escalation suppression —** if a project manager sets a Revised Due Date on an overdue ticket, escalation emails are paused until the revised date is also missed.
* **Sprint overrun alert —** when a sprint is projected to overrun, an immediate alert is sent to the project manager and configured recipients.
* **Milestone alert —** when a milestone is flagged At Risk or becomes overdue, an alert is sent immediately to the project manager and director-level recipients as configured.

# 15. Email Notification Settings

*The system provides a comprehensive notification settings panel at both the organisation and project level, allowing administrators and project managers to precisely control who receives emails for every type of event.*

## 15.1 Settings Location & Access

* **Organisation-level settings —** administrators can configure default email notification rules that apply across all projects unless overridden at the project level.
* **Project-level settings —** project managers can configure notification rules specific to their project, overriding the organisation defaults where needed.
* **User-level preferences —** individual users can further adjust which notifications they personally receive (within the bounds of what the project or organisation settings permit); they cannot opt out of escalation emails directed at their role.

## 15.2 Recipient Configuration

*For every notification event type, the project manager or administrator can define a recipient list by selecting from the following recipient types. Multiple recipient types can be combined for any single event.*

| **Recipient Type** | **Description** |
| --- | --- |
| **Assignee** | The user(s) directly assigned to the ticket or item that triggered the event. |
| **Reporter** | The user who originally created the ticket. |
| **Watchers** | All users who have subscribed to the ticket as watchers. |
| **Team Lead** | The designated lead of the team assigned to the ticket (e.g. QA Lead, Dev Lead). Team leads are configured in team settings. |
| **Project Manager** | The project manager assigned to the project containing the ticket. |
| **Director** | A named director-level user configured in the project settings as a senior escalation recipient. |
| **Sub-Project Owner** | The user designated as the owner of the sub-project the ticket belongs to. |
| **Specific User(s)** | One or more individually named users selected from the organisation's user list. |
| **Specific Role** | All users holding a specific role on the project (e.g. all QA Engineers, all BAs on this project). |
| **Specific Team** | All members of a named team assigned to the project (e.g. the full Development team or the full QA team). |
| **External Email** | One or more external email addresses not associated with a system account. Useful for notifying clients or external stakeholders without giving them system access. |

## 15.3 Notification Event Configuration

*Each event type can be independently configured with its own recipient list, delivery timing, and escalation rules. The table below lists all configurable notification events.*

| **Event** | **When It Triggers** | **Default Recipients** |
| --- | --- | --- |
| **Ticket Created** | A new ticket is created in the project. | *Assignee, Reporter* |
| **Ticket Assigned** | A ticket is assigned or reassigned to a user. | *Assignee* |
| **Ticket Status Changed** | A ticket's status is updated to any new value. | *Assignee, Watchers* |
| **Ticket Commented On** | A comment is posted on a ticket. | *Assignee, Watchers* |
| **Ticket Mentioned (@)** | A user is @mentioned in a comment or description. | *Mentioned User* |
| **Ticket Due in 48 Hours** | A ticket's due date is within the next 48 hours and is not yet Completed. | *Assignee* |
| **Ticket Due in 24 Hours** | A ticket's due date is within the next 24 hours and is not yet Completed. | *Assignee, Team Lead* |
| **Ticket Overdue** | A ticket's Expected Due Date has passed and status is not Completed. | *Assignee, Team Lead, Project Manager* |
| **Ticket Overdue — 3+ Days** | A ticket remains overdue for 3 or more business days with no revised date set. | *Assignee, Team Lead, Project Manager, Director* |
| **Ticket Paused** | A ticket's status is set to Paused. | *Assignee, Project Manager* |
| **Ticket Blocked** | A ticket is blocked by a dependency that becomes overdue. | *Assignee, Reporter, Project Manager* |
| **Ticket Reopened** | A completed or closed ticket is reopened. | *Assignee, Reporter, Team Lead* |
| **Attachment Added** | A file, document, image, or link is attached to a ticket. | *Assignee, Watchers* |
| **Approval Requested** | A BRD or user story is submitted for approval. | *Designated Approver* |
| **Approval Decision** | An approval is granted, rejected, or returned for changes. | *Reporter, Project Manager* |
| **Sprint Started** | A sprint is activated by the project manager. | *All Sprint Members* |
| **Sprint Closing Soon** | A sprint has 2 days remaining with incomplete tickets. | *Project Manager, Team Leads* |
| **Sprint Overrun Risk** | Sprint projected to overrun based on remaining points and velocity. | *Project Manager, Director* |
| **Sprint Closed** | A sprint is formally closed by the project manager. | *All Sprint Members, Project Manager* |
| **Milestone At Risk** | A milestone's projected completion exceeds its target date. | *Project Manager, Director* |
| **Milestone Overdue** | A milestone's target date has passed and it is not yet Completed. | *Project Manager, Director* |
| **Milestone 7-Day Alert** | A milestone is within 7 days of its deadline and less than 80% complete. | *Project Manager, Sub-Project Owner, Director* |
| **Milestone Completed** | All tickets linked to a milestone are marked Completed. | *Project Manager, Director* |
| **Customer Bug Received** | A customer email bug report is parsed and a ticket is auto-created. | *Assignee (default triage), Project Manager* |
| **Customer Bug Resolved** | A customer-reported bug ticket is moved to Completed. | *Reporter (customer email), Project Manager* |

## 15.4 Notification Delivery Options

* **Channel selection —** for each event type, the project manager can choose whether notifications are delivered via In-App only, Email only, or both In-App and Email.
* **Delivery timing —** for non-urgent events (e.g. comment posted), the project manager can configure whether emails are sent immediately or batched into a daily digest.
* **Digest option —** users can opt into a daily digest email that consolidates all non-urgent notifications from the previous 24 hours into a single email, reducing inbox noise.
* **Escalation delay —** for escalation events (e.g. Ticket Overdue — 3+ Days), the project manager sets how many business days must pass before the escalation email fires.
* **Repeat interval —** escalation emails can be set to repeat every N business days until the condition is resolved (e.g. resend to Director every 2 days while ticket remains overdue).
* **Quiet hours —** organisation-level setting to suppress email delivery between configured hours (e.g. no emails sent between 8:00 PM and 7:00 AM local time).
* **CC and BCC —** any notification rule can include additional CC or BCC recipients beyond the primary recipient list.

## 15.5 Email Content Configuration

* **Email subject template —** the subject line for each notification type is configurable with dynamic variables: {ticket\_id}, {ticket\_title}, {project\_name}, {assignee\_name}, {status}, {days\_overdue}, {due\_date}.
* **Email body —** notification emails include: a summary of the event, key ticket details (ID, title, sub-project, priority, status, assignee, due date), a direct link to the ticket in the system, and the delay reason or comment text where applicable.
* **Custom message —** project managers can add a custom introductory message to specific notification types (e.g. a note explaining the escalation process to Director-level recipients).
* **Branding —** organisation-level setting to add a company logo and colour to notification email headers.
* **Reply-to address —** the reply-to address for notification emails is configurable; replies can route to a project manager's inbox or a shared team mailbox.
* **Unsubscribe —** all non-mandatory notification emails include an unsubscribe link; users who unsubscribe from a specific event type are noted in their notification preferences. Escalation emails directed at a user's role cannot be unsubscribed from.

## 15.6 Notification Log

* **Sent email log —** a searchable log within the admin and project manager settings panel showing every notification email sent: recipient, event type, ticket or item referenced, timestamp, and delivery status (Sent, Delivered, Bounced, Opened).
* **Bounce handling —** if an email bounces (e.g. invalid address), the system flags the recipient's email address in their user profile and notifies the administrator.
* **Audit trail —** changes to notification settings (who changed what rule and when) are recorded in the system's audit log.

*— End of Document —*