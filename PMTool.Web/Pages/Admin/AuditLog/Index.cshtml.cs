using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Infrastructure.DTOs.Audit;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Web.Pages.Admin.AuditLog;

[Authorize(Roles = "Administrator")]
public class IndexModel : PageModel
{
    private readonly IAuditRepository _auditRepository;

    public IEnumerable<AuditLogDto> Logs { get; set; } = new List<AuditLogDto>();
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }

    [BindProperty(SupportsGet = true)] public Guid? FilterUserId { get; set; }
    [BindProperty(SupportsGet = true)] public string? FilterEntityType { get; set; }
    [BindProperty(SupportsGet = true)] public DateTime? FilterDateFrom { get; set; }
    [BindProperty(SupportsGet = true)] public DateTime? FilterDateTo { get; set; }
    [BindProperty(SupportsGet = true)] public int CurrentPage { get; set; } = 1;

    private const int PageSize = 25;

    public IndexModel(IAuditRepository auditRepository)
    {
        _auditRepository = auditRepository;
    }

    public async Task OnGetAsync()
    {
        var request = new AuditQueryRequest
        {
            UserId = FilterUserId,
            EntityType = FilterEntityType,
            DateFrom = FilterDateFrom,
            DateTo = FilterDateTo.HasValue ? FilterDateTo.Value.AddDays(1).AddTicks(-1) : null,
            Page = CurrentPage < 1 ? 1 : CurrentPage,
            PageSize = PageSize
        };

        var (items, total) = await _auditRepository.QueryAsync(request);
        Logs = items;
        TotalCount = total;
        TotalPages = (int)Math.Ceiling(total / (double)PageSize);
    }
}
