using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.DTOs.Progress;
using PMTool.Application.Interfaces;

namespace PMTool.Web.Pages.Progress;

[Authorize]
public class AnalysisModel : PageModel
{
    private readonly IProgressService _progressService;

    public ProductAnalysisDto Data { get; set; } = new();

    // Pre-serialised JSON consumed by Chart.js (rendered via @Html.Raw to avoid RZ1006/RZ1027)
    public string BurndownLabelsJson { get; set; } = "[]";
    public string BurndownIdealJson { get; set; } = "[]";
    public string BurndownActualJson { get; set; } = "[]";
    public string BurndownForecastJson { get; set; } = "[]";

    public AnalysisModel(IProgressService progressService)
    {
        _progressService = progressService;
    }

    public async Task<IActionResult> OnGetAsync(Guid productId)
    {
        if (productId == Guid.Empty)
            return RedirectToPage("/Progress/Progress");

        Data = await _progressService.GetProductAnalysisAsync(productId);

        if (Data.ActiveSprint?.BurndownPoints.Any() == true)
        {
            var pts = Data.ActiveSprint.BurndownPoints;
            var opts = new JsonSerializerOptions { WriteIndented = false };

            BurndownLabelsJson = JsonSerializer.Serialize(pts.Select(p => p.Date.ToString("MMM d")), opts);
            BurndownIdealJson = JsonSerializer.Serialize(pts.Select(p => (object?)p.Ideal), opts);
            BurndownActualJson = JsonSerializer.Serialize(pts.Select(p => (object?)p.Actual), opts);
            BurndownForecastJson = JsonSerializer.Serialize(pts.Select(p => (object?)p.Forecast), opts);
        }

        return Page();
    }

    // ── Helpers used in the view ──────────────────────────────────────

    public static string ProgressRingHtml(int pct, int size, int stroke, string colorHex)
    {
        double r = (size - stroke) / 2.0;
        double circ = 2 * Math.PI * r;
        double dash = circ * pct / 100.0;
        int cx = size / 2;
        int cy = size / 2;
        return
            $"<svg width=\"{size}\" height=\"{size}\" viewBox=\"0 0 {size} {size}\" style=\"transform:rotate(-90deg)\">" +
            $"<circle cx=\"{cx}\" cy=\"{cy}\" r=\"{r:F1}\" fill=\"none\" stroke=\"#e9ecef\" stroke-width=\"{stroke}\"/>" +
            $"<circle cx=\"{cx}\" cy=\"{cy}\" r=\"{r:F1}\" fill=\"none\" stroke=\"{colorHex}\" stroke-width=\"{stroke}\" " +
            $"stroke-dasharray=\"{dash:F2} {circ:F2}\" stroke-linecap=\"round\"/>" +
            "</svg>";
    }

    public static string ProductStatusLabel(int status) => status switch
    {
        1 => "Planned",
        2 => "In Development",
        3 => "In Testing",
        4 => "Released",
        5 => "Deprecated",
        _ => "Unknown"
    };

    public static string ProductStatusBadgeClass(int status) => status switch
    {
        1 => "bg-info text-dark",
        2 => "bg-warning text-dark",
        3 => "bg-primary",
        4 => "bg-success",
        5 => "bg-secondary",
        _ => "bg-secondary"
    };

    public static string HealthBadgeClass(HealthStatus health) => health switch
    {
        HealthStatus.OnTrack => "bg-success",
        HealthStatus.AtRisk  => "bg-warning text-dark",
        HealthStatus.Blocked => "bg-danger",
        _                    => "bg-secondary"
    };

    public static string SeverityBarClass(RiskSeverity severity) => severity switch
    {
        RiskSeverity.Critical => "bg-danger",
        RiskSeverity.Warning  => "bg-warning",
        RiskSeverity.Ok       => "bg-success",
        _                     => "bg-secondary"
    };

    public static string RelativeDueText(DateTime? dueDate)
    {
        if (!dueDate.HasValue) return string.Empty;
        var diff = (dueDate.Value.Date - DateTime.UtcNow.Date).Days;
        return diff switch
        {
            < 0 => $"Overdue by {-diff} day{(-diff != 1 ? "s" : "")}",
            0   => "Due today",
            1   => "Due tomorrow",
            _   => $"Due in {diff} days"
        };
    }
}
