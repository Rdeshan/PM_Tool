# Project Management Dashboard Changes

## Overview
Refactored project management functionality to restrict create, edit, and delete operations to only Admin and Project Manager dashboards.

## Changes Made

### New Dashboard Pages Created

#### 1. **Admin Dashboard** (`PMTool.Web\Pages\Admin\Dashboard.cshtml` & `.cshtml.cs`)
- Restricted to users with "Administrator" role
- Features:
  - Create New Project button (leading to /Projects/Create)
  - Recent Projects grid showing last 6 projects
  - Edit and Delete buttons on each project card
  - Quick navigation to Projects Index
  - User Management, Roles & Permissions, and System Settings cards (for future implementation)

#### 2. **Project Manager Dashboard** (`PMTool.Web\Pages\PM\Dashboard.cshtml` & `.cshtml.cs`)
- Restricted to users with "Project Manager" role
- Features:
  - Create New Project button (leading to /Projects/Create)
  - Recent Projects grid showing last 6 projects
  - Edit and Delete buttons on each project card
  - Quick navigation to Projects Index
  - Project Statistics (Active project count, Team member count)

### Modified Pages

#### 3. **Main Dashboard** (`PMTool.Web\Pages\Dashboard.cshtml`)
- Added role-based button visibility
- Admin users see: "Go to Admin Dashboard" button
- PM users see: "Go to Project Manager Dashboard" button
- Added direct link to Projects Index from main dashboard

#### 4. **Projects Index** (`PMTool.Web\Pages\Projects\Index.cshtml`)
- Removed "Create New Project" button from header
- Removed "Edit" button from project cards (shown only in admin/PM dashboards)
- Added navigation buttons to respective dashboards for admin/PM users
- Users can still view projects and their details
- Archive functionality remains available for admin/PM users

#### 5. **Projects Details** (`PMTool.Web\Pages\Projects\Details.cshtml`)
- Already updated to hide edit/delete buttons for non-admin/non-PM users (from previous changes)
- Now serves as a read-only view for regular users
- Only back button visible for non-authorized users

## Access Control

### Regular Users
- Can view all projects in Projects/Index
- Can view project details
- Cannot create, edit, or delete projects
- Do not have access to Admin or PM dashboards

### Project Manager Users
- Can access PM Dashboard at `/PM/Dashboard`
- Can create new projects
- Can edit projects
- Can delete projects
- Can view all projects with full controls from dashboard

### Administrator Users
- Can access Admin Dashboard at `/Admin/Dashboard`
- Can create new projects
- Can edit projects
- Can delete projects
- Can view all projects with full controls from dashboard

## Navigation Flow

### For Admins:
1. Login → Main Dashboard → Admin Dashboard → Project Management operations

### For Project Managers:
1. Login → Main Dashboard → PM Dashboard → Project Management operations

### For Regular Users:
1. Login → Main Dashboard → Projects/Index (view only) → Project Details (view only)

## Technical Details

- Used Razor Page authorization with `[Authorize(Roles = "Administrator")]` and `[Authorize(Roles = "Project Manager")]`
- Implemented delete handlers on both dashboards with proper error handling
- Maintains consistent UI/UX across all dashboard pages
- All edit/delete operations properly validated on both client and server side
