# OPMS - Developer Setup Guide

Welcome! This guide will help you set up the OPMS project on your machine.

## Prerequisites

Before you begin, ensure you have the following installed:

### Required Software
- **Visual Studio 2022 or later** (Community, Professional, or Enterprise)
  - Download: https://visualstudio.microsoft.com/downloads/
  - Required Workloads:
    - ASP.NET and web development
    - .NET 10 Desktop Development

- **.NET 10 SDK**
  - Download: https://dotnet.microsoft.com/download/dotnet/10.0
  - Verify installation: Open PowerShell and run `dotnet --version`

- **SQL Server**
  - **Option 1 (Recommended):** SQL Server LocalDB (comes with Visual Studio)
  - **Option 2:** SQL Server Express or Full Edition
  - Download SQL Server: https://www.microsoft.com/sql-server/sql-server-downloads

- **Git**
  - Download: https://git-scm.com/download/win
  - Verify installation: `git --version`

---

## Step 1: Clone the Repository

1. Open PowerShell or Command Prompt
2. Navigate to your desired directory:
```powershell
cd C:\Users\YourUsername\source\repos
```

3. Clone the repository:
```powershell
git clone https://github.com/Rdeshan/PM_Tool.git
```

4. Navigate to the project folder:
```powershell
cd PM_Tool
```

5. Checkout the feature branch (if needed):
```powershell
git checkout feature/Sub-Project
```

---

## Step 2: Open Project in Visual Studio

1. Open **Visual Studio 2022 or later**
2. Click **File** → **Open** → **Project/Solution**
3. Navigate to `C:\Users\YourUsername\source\repos\PM_Tool`
4. Select the solution file (`.sln`)
5. Click **Open**

Visual Studio will load the solution. Wait for the projects to fully load.

---

## Step 3: Restore NuGet Packages

1. In Visual Studio, click **Tools** → **NuGet Package Manager** → **Package Manager Console**
2. In the Package Manager Console, run:
```powershell
Update-Package -Reinstall
```

Or simply rebuild the solution:
   - **Build** → **Rebuild Solution** (Ctrl + Alt + F5)

---

## Step 4: Configure the Database

### Option A: Use LocalDB (Recommended)

LocalDB is automatically installed with Visual Studio and requires no additional setup.

The connection string in `appsettings.json` is already configured:
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=OUSL_PMDB;Trusted_Connection=True;MultipleActiveResultSets=true"
```

### Option B: Use SQL Server Express or Full Edition

If you're using a different SQL Server instance, update the connection string in `PMTool.Web\appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=OUSL_PMDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

Replace `YOUR_SERVER_NAME` with your SQL Server instance name (e.g., `DESKTOP-ABC123\SQLEXPRESS`).

---

## Step 5: Apply Database Migrations

1. In Visual Studio, open **Package Manager Console**:
   - **Tools** → **NuGet Package Manager** → **Package Manager Console**

2. Ensure the default project is set to `PMTool.Infrastructure`

3. Run the migration command:
```powershell
Update-Database
```

This will:
   - Create the database `OUSL_PMDB`
   - Apply all pending migrations
   - Create all required tables and schemas

If migrations are successful, you should see:
```
Build started...
Build succeeded.
Applying migration '...'
Done.
```

---

## Step 6: Seed Test Data

The application automatically seeds test users and roles when it first runs. These will be created during the first launch.

### Default Test Accounts

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@pmtool.com | Admin@123 |
| Project Manager | pm@pmtool.com | PM@123 |
| Developer | dev@pmtool.com | Dev@123 |
| QA | qa@pmtool.com | QA@123 |
| Business Analyst | ba@pmtool.com | BA@123 |
| Viewer | viewer@pmtool.com | Viewer@123 |
| Guest | guest@pmtool.com | Guest@123 |

**⚠️ Security Note:** These accounts are for development only. Never use these credentials in production.

---

## Step 7: Configure Email Service (Optional)

Email functionality is currently commented out. To enable it:

1. Open `PMTool.Web\appsettings.json`

2. Uncomment the Email section and update with your SMTP settings:

```json
"Email": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": 587,
  "SenderEmail": "your-email@gmail.com",
  "SenderPassword": "your-app-password",
  "SenderName": "PMTool",
  "UseSSL": true,
  "IsEnabled": true
}
```

**For Gmail users:**
- Use: `smtp.gmail.com`
- Port: `587`
- Enable 2-Step Verification in your Google Account
- Generate an App Password: https://myaccount.google.com/apppasswords
- Use the App Password instead of your regular password

---

## Step 8: Run the Application

1. Set `PMTool.Web` as the startup project:
   - Right-click on `PMTool.Web` in Solution Explorer
   - Select **Set as Startup Project**

2. Press **F5** or click **Debug** → **Start Debugging**

3. The application will:
   - Compile
   - Apply any pending migrations
   - Seed default roles and test users
   - Launch in your default browser at `http://localhost:5113`

4. You should see the Login page

5. Use any of the test accounts above to log in

---

## Step 9: Quick Login (Development Only)

When running in Development mode, the login page displays **Quick Login** buttons for each role.

This is a convenient feature for testing different user roles without manually entering credentials each time.

**Location:** `PMTool.Web\Pages\Auth\Login.cshtml`

Each button pre-fills the email and password and submits to the `OnPostQuickLoginAsync` handler in `PMTool.Web\Pages\Auth\Login.cshtml.cs`

---

## Project Structure

```
PM_Tool/
├── PMTool.Domain/              # Core domain models and entities
│   ├── Entities/               # Database entities
│   │   ├── User.cs
│   │   ├── Project.cs
│   │   ├── Product.cs
│   │   ├── Team.cs
│   │   └── ...
│   ├── Enums/                  # Enumerations
│   │   ├── ProjectStatus.cs
│   │   ├── ProductEnums.cs
│   │   ├── SubProjectStatus.cs
│   │   ├── BacklogItemStatus.cs
│   │   └── RolePermission.cs
│   └── ValueObjects/           # Value objects
│
├── PMTool.Application/         # Business logic layer
│   ├── DTOs/                   # Data Transfer Objects
│   │   ├── User/
│   │   ├── Product/
│   │   ├── Project/
│   │   └── ...
│   ├── Interfaces/             # Service contracts
│   ├── Services/               # Application services
│   │   ├── Auth/
│   │   ├── Admin/
│   │   ├── Product/
│   │   ├── Project/
│   │   └── ...
│   ├── Validators/             # FluentValidation validators
│   │   ├── Auth/
│   │   ├── Product/
│   │   ├── Project/
│   │   ├── SubProject/
│   │   └── User/
│   └── Requests/               # Request models
│
├── PMTool.Infrastructure/      # Data access layer
│   ├── Data/
│   │   └── AppDbContext.cs     # DbContext
│   ├── Migrations/             # EF Core migrations
│   ├── Repositories/           # Repository implementations
│   │   ├── Interfaces/
│   │   ├── UserRepository.cs
│   │   ├── ProjectRepository.cs
│   │   ├── ProductRepository.cs
│   │   └── ...
│   └── Services/               # Infrastructure services
│       ├── TokenService.cs
│       ├── EmailService.cs
│       └── ...
│
├── PMTool.Web/                 # Presentation layer (Razor Pages)
│   ├── Pages/                  # Razor Pages
│   │   ├── Auth/
│   │   │   ├── Login.cshtml
│   │   │   ├── Login.cshtml.cs
│   │   │   ├── Register.cshtml
│   │   │   └── ...
│   │   ├── Admin/
│   │   │   ├── Dashboard.cshtml
│   │   │   └── Dashboard.cshtml.cs
│   │   ├── Products/
│   │   │   ├── Details.cshtml
│   │   │   └── Details.cshtml.cs
│   │   ├── Projects/
│   │   ├── PM/
│   │   ├── Backlog/
│   │   ├── Account/
│   │   ├── Shared/
│   │   └── _Layout.cshtml
│   ├── appsettings.json        # Configuration
│   ├── appsettings.Development.json
│   ├── Program.cs              # Startup configuration
│   └── Properties/
│       └── launchSettings.json
│
├── SETUP.md                    # This file
├── README.md                   # Project overview
└── PM_Tool.sln                 # Solution file
```

---

## Troubleshooting

### Issue: "Database connection failed"

**Solution:**
- Verify SQL Server LocalDB is installed:
```powershell
sqllocaldb info mssqllocaldb
```
- Check connection string in `appsettings.json`
- Ensure the database path is correct
- Try restarting Visual Studio

### Issue: "Migrations failed"

**Solution:**
```powershell
# Remove pending migrations
Remove-Migration

# Re-apply migrations
Update-Database
```

### Issue: "Port 5113 already in use"

**Solution:**
1. Edit `PMTool.Web\Properties\launchSettings.json`
2. Change the port number in the `applicationUrl` property
3. Also update `AppUrl` in `appsettings.json`

### Issue: "Quick Login buttons not showing"

**Solution:**
- Quick login is only available in Development mode
- Verify you're running in Development mode
- Check `Program.cs` for environment configuration

### Issue: ".NET 10 SDK not found"

**Solution:**
1. Download and install .NET 10 SDK from https://dotnet.microsoft.com/download/dotnet/10.0
2. Restart Visual Studio
3. Verify: `dotnet --version`

### Issue: "NuGet packages restore failed"

**Solution:**
1. Clear NuGet cache:
```powershell
nuget locals all -clear
```
2. Rebuild the solution
3. If still failing, check your internet connection and NuGet package sources

---

## Common Commands

### Build the solution
```powershell
dotnet build
```

### Run the application
```powershell
dotnet run --project PMTool.Web
```

### Apply migrations
```powershell
dotnet ef database update --project PMTool.Infrastructure
```

### Create a new migration
```powershell
dotnet ef migrations add MigrationName --project PMTool.Infrastructure --startup-project PMTool.Web
```

### Remove the last migration (if not applied)
```powershell
dotnet ef migrations remove --project PMTool.Infrastructure
```

### Run tests (if available)
```powershell
dotnet test
```

### Clean build
```powershell
dotnet clean
dotnet build
```

---

## Git Workflow

### Clone and setup
```powershell
git clone https://github.com/Rdeshan/PM_Tool.git
cd PM_Tool
git checkout feature/Sub-Project
```

### Create a new feature branch
```powershell
git checkout -b feature/your-feature-name
```

### Commit changes
```powershell
git add .
git commit -m "Your descriptive commit message"
```

### Push changes
```powershell
git push origin feature/your-feature-name
```

### Pull latest changes
```powershell
git pull origin feature/Sub-Project
```

### View branch status
```powershell
git status
git log --oneline -5
```

---

## Development Tips

1. **Use Quick Login** - In Development mode, click Quick Login buttons instead of typing credentials
2. **Check migrations before pulling** - Before pulling changes, apply migrations with `Update-Database`
3. **Clear browser cache** - If UI changes aren't showing, clear cache (Ctrl + Shift + Delete)
4. **Review appsettings** - Environment-specific settings may need adjustment
5. **Enable detailed logging** - Set LogLevel to "Debug" in `appsettings.json` for troubleshooting
6. **Use Solution Explorer** - Familiarize yourself with the project structure
7. **Set startup project** - Always ensure `PMTool.Web` is the startup project before running

---

## Architecture Overview

### Authentication & Authorization
- Cookie-based authentication
- Role-based access control (RBAC)
- Two-factor authentication (2FA) support
- Permissions system with granular controls

### Core Features
- **Project Management** - Create and manage projects with status tracking
- **Product Management** - Define products, features, and releases
- **Sub-Projects** - Break down work into manageable sub-projects with dependencies
- **Team Management** - Organize users into teams for collaboration
- **Backlog Management** - Track work items and sprint planning
- **User Administration** - Manage users, roles, and permissions
- **Role-Based Access Control** - Fine-grained access control

### Technology Stack
- **.NET 10** - Runtime framework
- **ASP.NET Core Razor Pages** - Web framework
- **Entity Framework Core** - ORM for database access
- **SQL Server** - Database (LocalDB for development)
- **FluentValidation** - Input validation
- **Bootstrap 5** - Responsive UI framework
- **Cookies** - Session management

### Key Design Patterns
- **Repository Pattern** - Data access abstraction
- **Service Layer Pattern** - Business logic isolation
- **DTO Pattern** - Data transfer objects for API/View models
- **Dependency Injection** - Loose coupling and testability

---

## Environment Configuration

### Development Environment
- **Location:** `appsettings.Development.json`
- **Features:** Quick Login enabled, detailed logging, hot reload
- **Database:** LocalDB (local development database)
- **SSL:** Optional

### Production Environment
- **Location:** `appsettings.json`
- **Features:** Quick Login disabled, minimal logging
- **Database:** SQL Server (separate instance)
- **SSL:** Required (HTTPS)

---

## Database Schema Highlights

### Core Tables
- `Users` - User accounts and profiles
- `Roles` - User roles (Admin, PM, Developer, QA, BA, Viewer, Guest)
- `Permissions` - Granular permissions for roles
- `Projects` - Project entities with status tracking
- `Products` - Product definitions within projects
- `SubProjects` - Sub-project breakdown
- `Teams` - Team groupings
- `TeamMembers` - User-team associations
- `BacklogItems` - Work items for sprint/backlog planning

---

## Support & Resources

- **Project Repository:** https://github.com/Rdeshan/PM_Tool
- **GitHub Issues:** Report bugs or request features
- **.NET 10 Documentation:** https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10
- **ASP.NET Core:** https://learn.microsoft.com/en-us/aspnet/core
- **Entity Framework Core:** https://learn.microsoft.com/en-us/ef/core
- **Razor Pages:** https://learn.microsoft.com/en-us/aspnet/core/razor-pages
- **Bootstrap 5:** https://getbootstrap.com/docs/5.0

---

## Next Steps After Setup

1. ✅ Explore the project structure
2. ✅ Read the inline code documentation and comments
3. ✅ Test different user roles using Quick Login buttons
4. ✅ Review the database schema in SQL Server Management Studio
5. ✅ Examine the validators to understand input validation patterns
6. ✅ Create a feature branch for your contributions
7. ✅ Follow the coding standards used throughout the project
8. ✅ Review existing pull requests to understand the workflow

---

## Reporting Issues

If you encounter issues during setup:

1. Check this SETUP.md guide first
2. Review the Troubleshooting section
3. Check GitHub Issues for similar problems
4. Document the steps to reproduce
5. Include your environment details (OS, Visual Studio version, .NET version)

---

**Happy coding! 🚀**

*Last Updated: 2024*
