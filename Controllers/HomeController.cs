using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace BTLChatDemo.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role == "Admin")
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            return View();
        }
    }
}
