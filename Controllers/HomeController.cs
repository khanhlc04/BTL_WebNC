using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace BTLChatDemo.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Login", "Account");
        }
    }
}
