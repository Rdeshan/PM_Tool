using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.DTOs.Project;
using PMTool.Application.Interfaces;

namespace PMTool.Web.Pages.Tasks;

[Authorize]
public class IndexModel : PageModel
{

}