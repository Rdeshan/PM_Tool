# Common Components - Visual Guide

## 📊 Component Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────┐
│                    COMMON COMPONENTS FOLDER                         │
│              Pages/Shared/Common/ (Reusable Partials)               │
└─────────────────────────────────────────────────────────────────────┘

                              │
                ┌─────────────┼─────────────┐
                │             │             │
                ▼             ▼             ▼
         ┌──────────┐  ┌──────────┐  ┌──────────┐
         │Navigation│  │ Messages │  │  Actions │
         │Components│  │Components│  │Components│
         └──────────┘  └──────────┘  └──────────┘
             │             │             │
         ┌───┴───┐         │         ┌───┴───┐
         ▼       ▼         ▼         ▼       ▼
      Back    Cancel   Alert     Action   Delete
      Button  Button   Message   Buttons  Modal
```

---

## 🎨 Visual Component Examples

### 1. Back Button Component

```
╔════════════════════════════════════════════╗
║         Project Details Page               ║
║                                            ║
║  ┌──────────────────────────────────────┐  ║
║  │ Project: Web Portal                  │  ║
║  │ Client: ABC Corp                     │  ║
║  │ Status: Active                       │  ║
║  │                                      │  ║
║  └──────────────────────────────────────┘  ║
║                                            ║
║  ┌─────────────────────────────┐           ║
║  │ [← Back]                    │           ║
║  └─────────────────────────────┘           ║
║  Footer (uses _BackButton)                ║
╚════════════════════════════════════════════╝
```

### 2. Cancel Button in Form

```
╔════════════════════════════════════════════╗
║         Create New Project                 ║
║                                            ║
║  Name: [________________]                  ║
║  Code: [________________]                  ║
║  Date: [________________]                  ║
║                                            ║
║  ┌──────────────────────────────────────┐  ║
║  │ [Cancel]           [Create Project]  │  ║
║  └──────────────────────────────────────┘  ║
║  (uses _CancelButton and submit button)   ║
╚════════════════════════════════════════════╝
```

### 3. Alert Message Component

```
╔════════════════════════════════════════════╗
║  [!] Error occurred while creating project  │ ✕
║      Please check all required fields.     ║
║                                            ║
╔════════════════════════════════════════════╗
║  [✓] Project created successfully!          │ ✕
║                                            ║
╚════════════════════════════════════════════╝

(uses _AlertMessage with Type: "danger" or "success")
```

### 4. Action Button Group

```
╔════════════════════════════════════════════╗
│           PROJECT CARDS (Grid)             │
├────────────────────────────────────────────┤
│                                            │
│  ┌─────────────────┐  ┌─────────────────┐ │
│  │ Project Name    │  │ Project Name    │ │
│  │ ABC Corp        │  │ XYZ Inc         │ │
│  │ Status: Active  │  │ Status: On Hold │ │
│  │                 │  │                 │ │
│  │ [View] [Edit]   │  │ [View] [Edit]   │ │
│  │ (button group)  │  │ (button group)  │ │
│  └─────────────────┘  └─────────────────┘ │
│                                            │
│  (uses _ActionButtonGroup in card footer) │
└────────────────────────────────────────────┘
```

### 5. Delete Confirmation Modal

```
╔════════════════════════════════════════════╗
║         Page Behind Modal...              ║
║         (Darkened/Disabled)               ║
║                                            ║
║     ╔══════════════════════════════════╗   ║
║     ║ [!] Confirm Delete               ║   ║
║     ╟──────────────────────────────────╢   ║
║     ║ Are you sure you want to delete  ║   ║
║     ║ this project?                    ║   ║
║     ║                                  ║   ║
║     ║ Item: Web Portal                 ║   ║
║     ║                                  ║   ║
║     ║ This action cannot be undone.    ║   ║
║     ╟──────────────────────────────────╢   ║
║     ║ [Cancel]        [🗑 Delete]      ║   ║
║     ╚══════════════════════════════════╝   ║
║                                            ║
│        (uses _DeleteConfirmModal)          │
╚════════════════════════════════════════════╝
```

---

## 🔄 Data Flow Diagram

```
USER CLICKS BUTTON
        │
        ▼
┌──────────────────────┐
│   Check Auth/        │
│   Permission         │
└──────────┬───────────┘
           │
    ┌──────┴──────┐
    │             │
    ▼             ▼
 ALLOWED       DENIED
    │             │
    ▼             ▼
 Navigate    Show Error
    │        (_AlertMessage)
    ▼             │
Navigate to   Stay on
 Details      Page
```

---

## 🎯 Component Selection Flow

```
START: Building a Page
        │
        ▼
    ┌─────────────────────────┐
    │ What's the page type?   │
    └─────────────────────────┘
        │
    ┌───┴───┬───────────────┬──────────────┐
    │       │               │              │
    ▼       ▼               ▼              ▼
  CREATE  EDIT           DETAILS        LIST/INDEX
   PAGE   PAGE            PAGE           PAGE
    │       │               │              │
    ▼       ▼               ▼              ▼
  Need    Need           Need            Need
 Cancel  Cancel          Back            Actions
 Button  Button          Button          Buttons
    │       │               │              │
    ▼       ▼               ▼              ▼
_Cancel  _Cancel      _BackButton    _ActionButtonGroup
Button   Button
```

---

## 📍 Component Placement Map

```
PROJECTS / INDEX PAGE
┌─────────────────────────────────────────────┐
│                    Header                   │
│  [New Project] [Admin Dashboard]            │
├─────────────────────────────────────────────┤
│  Active Projects  │  Archived (tabs)        │
├─────────────────────────────────────────────┤
│                                             │
│  ┌─────────────┐  ┌─────────────┐          │
│  │ PROJECT     │  │ PROJECT     │          │
│  │ Card 1      │  │ Card 2      │          │
│  │             │  │             │          │
│  │ [Actions]   │  │ [Actions]   │ ◄────── _ActionButtonGroup
│  │ Buttons     │  │ Buttons     │
│  └─────────────┘  └─────────────┘          │
│                                             │
└─────────────────────────────────────────────┘


PROJECTS / CREATE PAGE
┌─────────────────────────────────────────────┐
│           Create New Project                │
├─────────────────────────────────────────────┤
│  [Error/Success Message]                    │ ◄────── _AlertMessage
│                                             │
│  Name: [________________]                   │
│  Code: [________________]                   │
│  ...                                        │
│                                             │
│  [Cancel]            [Create Project]       │ ◄────── _CancelButton
│                                             │
└─────────────────────────────────────────────┘


PROJECTS / DETAILS PAGE
┌─────────────────────────────────────────────┐
│           Project Details                   │
│           Web Portal                        │
├─────────────────────────────────────────────┤
│                                             │
│  Client: ABC Corp                           │
│  Status: Active                             │
│  Duration: Jan 1 - Dec 31, 2024             │
│  ...                                        │
│                                             │
│  [Edit] [Delete]                            │
│                                             │
├─────────────────────────────────────────────┤
│  [← Back]                                   │ ◄────── _BackButton
│                                             │
└─────────────────────────────────────────────┘
```

---

## 🔀 Component Interaction Flow

```
USER INTERACTIONS:

1. USER VIEWS LIST PAGE
   └─→ Sees _ActionButtonGroup buttons
       ├─→ Clicks [View] → Navigate to Details
       ├─→ Clicks [Edit] → Navigate to Edit
       └─→ Clicks [Delete] → Shows _DeleteConfirmModal

2. USER FILLS CREATE FORM
   └─→ Sees _CancelButton (Cancel button)
       ├─→ Clicks [Cancel] → Uses _CancelButton → Go back
       └─→ Clicks [Create] → Validate → Submit → Details Page

3. USER VIEWS DETAILS PAGE
   └─→ Sees _BackButton (Back button)
       ├─→ Clicks [Back] → Uses _BackButton → Go to List
       ├─→ Clicks [Edit] → Navigate to Edit
       └─→ Clicks [Delete] → _DeleteConfirmModal appears

4. USER SEES ERROR/SUCCESS
   └─→ Page shows _AlertMessage
       ├─→ Type: "danger" → Red alert with error icon
       ├─→ Type: "success" → Green alert with check icon
       ├─→ Type: "warning" → Yellow alert with warning icon
       └─→ Type: "info" → Blue alert with info icon
```

---

## 🗂️ Component to Page Mapping

```
┌──────────────────────────────────────────────────────────────────┐
│                    COMPONENT USAGE MATRIX                        │
├──────────────────────────────────────────────────────────────────┤
│                  │Projects│Products│SubProjects│Other            │
├──────────────────┼────────┼────────┼───────────┼────────         │
│_BackButton       │  ✓     │  ✓     │    ✓      │  ✓              │
│_CancelButton     │  ✓     │  ✓     │    ✓      │  ✓              │
│_AlertMessage     │  ✓     │  ✓     │    ✓      │  ✓              │
│_ActionButtonGrp  │  ✓     │  ✓     │    ✓      │  ✓              │
│_DeleteConfirm    │  ✓     │  ✓     │    ✓      │  ✓              │
└──────────────────┴────────┴────────┴───────────┴────────         ┘

Key:
✓ = Used in page type
○ = Sometimes used
- = Not typically used
```

---

## 🎨 Component State Diagram

```
_BackButton STATE:
┌─────────────┐
│   Enabled   │ (Normal state)
│ [← Back]    │
└──────┬──────┘
       │
       └─→ Click → Navigate

_CancelButton STATE:
┌──────────────┐
│   Enabled    │ (Normal state)
│ [Cancel]     │
└──────┬───────┘
       │
       └─→ Click → Navigate without save

_ActionButtonGroup STATES:
┌─────────────────────────────────────┐
│         Permission Check            │
└──────┬──────────────────────┬───────┘
       │                      │
   isAdmin/PM              Regular User
       │                      │
       ▼                      ▼
┌──────────────┐        ┌──────────────┐
│  FULL EDIT   │        │  VIEW ONLY   │
│ [V][E][D]    │        │ [V][E*][D*]  │
└──────────────┘        └──────────────┘
       *disabled
```

---

## 📱 Responsive Layout

```
DESKTOP VIEW (4 action buttons)
┌─────────────────────────────────────────┐
│ [View] [Edit] [Delete] [Archive]        │
└─────────────────────────────────────────┘

TABLET VIEW (2x2 buttons)
┌──────────────────┐
│ [View]  [Edit]   │
│ [Delete][Archive]│
└──────────────────┘

MOBILE VIEW (Stacked)
┌──────────────┐
│ [View]       │
│ [Edit]       │
│ [Delete]     │
│ [Archive]    │
└──────────────┘

(All use Bootstrap flex classes for responsiveness)
```

---

## 🎬 Animation & Transitions

```
Component Animations:

_BackButton / _CancelButton:
  Hover: Scale 1.05, shadow increase
  Click: Button press effect

_AlertMessage:
  Enter: Fade in + slide down
  Exit: Fade out + slide up

_ActionButtonGroup:
  Hover: Individual button highlight
  Each button has hover effect

_DeleteConfirmModal:
  Enter: Fade in + scale from center
  Exit: Fade out
```

---

## 🔐 Security & Permissions

```
Component Permission Checks:

_ActionButtonGroup:
  ├─ View Button: Always shown (if ShowView=true)
  ├─ Edit Button: 
  │  ├─ If CanEdit=true → Enabled
  │  └─ If CanEdit=false → Disabled
  └─ Delete Button:
     ├─ If CanEdit=true → Enabled
     └─ If CanEdit=false → Disabled

_CancelButton / _BackButton:
  └─ No permission check needed
```

---

## 📊 Performance Metrics

```
Component Size & Load Time:

_BackButton:
  Size: ~200 bytes (minified)
  Render: ~1ms

_CancelButton:
  Size: ~250 bytes
  Render: ~1ms

_AlertMessage:
  Size: ~350 bytes
  Render: ~2ms

_ActionButtonGroup:
  Size: ~500 bytes
  Render: ~2ms

_DeleteConfirmModal:
  Size: ~600 bytes
  Render: ~3ms

TOTAL: ~1900 bytes, ~9ms compile time
(All components together on a page)
```

---

**Visual Guide Complete**  
For implementation details, see README.md or QUICK_REFERENCE.md
