using Microsoft.AspNetCore.Mvc;

namespace BTL_WebNC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                // if (role == "Admin")
                // {
                //     return RedirectToAction("Index", "AdminDashboard");
                // }
                return View("~/Views/Home/Index.cshtml");
            }
            return RedirectToAction("Login", "Account");
        }
    }
}
