using Microsoft.AspNetCore.Mvc;

namespace BTLChatDemo.Controllers.Admin
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
