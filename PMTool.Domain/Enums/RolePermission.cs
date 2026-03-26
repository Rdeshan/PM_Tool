namespace PMTool.Domain.Enums;

public enum RoleType
{
    Administrator = 1,
    ProjectManager = 2,
    Developer = 3,
    QAEngineer = 4,
    BusinessAnalyst = 5,
    Viewer = 6,
    Guest = 7
}

public enum PermissionType
{
    // User Management
    ManageUsers = 1,
    ManageRoles = 2,
    ViewUsers = 3,

    // Project Management
    CreateProject = 10,
    EditProject = 11,
    DeleteProject = 12,
    ViewProject = 13,
    ManageProjectMembers = 14,

    // Product Management
    CreateProduct = 20,
    EditProduct = 21,
    DeleteProduct = 22,
    ViewProduct = 23,

    // Sub-Project Management
    CreateSubProject = 30,
    EditSubProject = 31,
    DeleteSubProject = 32,
    ViewSubProject = 33,

    // Sprint Management
    CreateSprint = 40,
    EditSprint = 41,
    DeleteSprint = 42,
    ViewSprint = 43,

    // Milestone Management
    CreateMilestone = 50,
    EditMilestone = 51,
    DeleteMilestone = 52,
    ViewMilestone = 53,

    // Ticket Management
    CreateTicket = 60,
    EditTicket = 61,
    DeleteTicket = 62,
    ViewTicket = 63,
    UpdateTicketStatus = 64,
    AssignTicket = 65,

    // Bug Management
    CreateBugTicket = 70,
    EditBugTicket = 71,
    DeleteBugTicket = 72,
    LinkTestCases = 73,

    // User Story Management
    CreateUserStory = 80,
    EditUserStory = 81,
    DeleteUserStory = 82,
    ViewUserStory = 83,

    // BRD Management
    CreateBRD = 90,
    EditBRD = 91,
    DeleteBRD = 92,
    ViewBRD = 93,

    // Time Tracking
    LogTime = 100,
    ViewTimeLog = 101,

    // Comments
    PostComment = 110,
    EditComment = 111,
    DeleteComment = 112,

    // Reporting & Analytics
    ViewReports = 120,
    ViewDashboards = 121,
    ExportData = 122,

    // Organization Management
    ManageOrganization = 130,
    ManageOrganizationSettings = 131,

    // System Administration
    ViewSystemLogs = 140,
    ManageSystemSettings = 141
}
