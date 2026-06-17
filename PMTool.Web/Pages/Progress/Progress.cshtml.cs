using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.DTOs.Progress;
using PMTool.Application.Interfaces;

namespace PMTool.Web.Pages.Progress;

[Authorize]
public class ProgressModel : PageModel
{
    private readonly IProgressService _progressService;

    public DashboardProgressDto Data { get; set; } = new();

    public ProgressModel(IProgressService progressService)
    {
        _progressService = progressService;
    }

    public async Task OnGetAsync()
    {
        Data = await _progressService.GetDashboardProgressAsync();
    }

    public static string StatusBadgeClass(int status) => status switch
    {
        1 => "bg-success",
        2 => "bg-warning text-dark",
        3 => "bg-info text-dark",
        _ => "bg-secondary"
    };

    public static string StatusLabel(int status) => status switch
    {
        1 => "Active",
        2 => "On Hold",
        3 => "Completed",
        _ => "Unknown"
    };

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
}
