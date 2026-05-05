using System;
using System.Collections.Generic;
using System.Text;

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
}
