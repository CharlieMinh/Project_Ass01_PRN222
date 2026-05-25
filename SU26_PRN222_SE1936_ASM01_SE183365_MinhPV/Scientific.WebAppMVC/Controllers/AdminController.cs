using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scientific.WebAppMVC.Authorization;

namespace Scientific.WebAppMVC.Controllers
{
    [Authorize(Policy = AppPolicies.AdminOnly)]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
