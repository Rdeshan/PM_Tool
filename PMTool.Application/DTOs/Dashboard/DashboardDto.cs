using System;
using System.Collections.Generic;

namespace PMTool.Application.DTOs.Dashboard
{
    public class DashboardDto
    {
        public int TotalProjects { get; set; }
        public int CompletedProjects { get; set; }
        public int RunningProjects { get; set; }
        public int PendingProjects { get; set; }
        public List<DashboardProjectDto> UpcomingProjects { get; set; } = new();
    }

    public class DashboardProjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public string? ColourCode { get; set; }
    }

    public class PersonalDashboardDto
    {
        public int OpenTickets { get; set; }
        public int OverdueTickets { get; set; }
        public int InProgressTickets { get; set; }
        public int DoneThisWeek { get; set; }
        public List<PersonalTicketDto> MyTickets { get; set; } = new();
        public List<PersonalActivityDto> RecentActivity { get; set; } = new();
        public PersonalSprintDto? ActiveSprint { get; set; }
        public List<PersonalNotificationDto> Notifications { get; set; } = new();
    }

    public class PersonalTicketDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Status { get; set; }
        public int Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public string? ProductName { get; set; }
        public Guid ProductId { get; set; }
        public Guid? SprintId { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class PersonalActivityDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Status { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? ProductName { get; set; }
        public Guid ProductId { get; set; }
    }

    public class PersonalSprintDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Goal { get; set; } = string.Empty;
        public int TotalItems { get; set; }
        public int DoneItems { get; set; }
        public int MyItems { get; set; }
    }

    public class PersonalNotificationDto
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public Guid? ItemId { get; set; }
    }

    public class CollabProjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string ColourCode { get; set; } = "#0D6EFD";
        public DateTime DueDate { get; set; }
        public int Status { get; set; }
        public List<CollabTeamDto> Teams { get; set; } = new();
    }

    public class CollabTeamDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string ColorCode { get; set; } = "#6c757d";
        public List<CollabMemberDto> Members { get; set; } = new();
    }

    public class CollabMemberDto
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; } = "";
        public string Email { get; set; } = "";
        public string RoleName { get; set; } = "";
        public string Initials { get; set; } = "";
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
    }
}
