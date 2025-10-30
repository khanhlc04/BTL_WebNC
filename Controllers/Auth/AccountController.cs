using System.Security.Claims;
using BTL_WebNC.Models.Account;
using BTL_WebNC.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace BTL_WebNC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (!(User?.Identity?.IsAuthenticated == true))
                return RedirectToAction("Login", "Account");

            return View("~/Views/Account/ChangePassword.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!(User?.Identity?.IsAuthenticated == true))
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                return View("~/Views/Account/ChangePassword.cshtml", model);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                ModelState.AddModelError("", "Không xác định được người dùng");
                return View("~/Views/Account/ChangePassword.cshtml", model);
            }

            if (!int.TryParse(userIdClaim.Value, out var userId))
            {
                ModelState.AddModelError("", "Không xác định được người dùng");
                return View("~/Views/Account/ChangePassword.cshtml", model);
            }

            var account = await _accountRepository.GetByIdAsync(userId);
            if (account == null)
            {
                ModelState.AddModelError("", "Tài khoản không tồn tại");
                return View("~/Views/Account/ChangePassword.cshtml", model);
            }

            // Verify current password (note: passwords are stored plain in this project)
            if (account.Password != model.CurrentPassword)
            {
                ModelState.AddModelError("CurrentPassword", "Mật khẩu hiện tại không đúng");
                return View("~/Views/Account/ChangePassword.cshtml", model);
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không khớp");
                return View("~/Views/Account/ChangePassword.cshtml", model);
            }

            // Update password
            account.Password = model.NewPassword;
            await _accountRepository.UpdateAsync(account);

            // After change, redirect to home
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                if (role == "Admin")
                {
                    return RedirectToAction("Index", "AdminDashboard");
                }
                return View("~/Views/Home/Index.cshtml");
            }
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AccountLogin model)
        {
            if (ModelState.IsValid)
            {
                var account = await _accountRepository.GetByEmailAsync(model.Email);

                if (
                    account != null
                    && account.Password == model.Password
                    && account.Deleted == false
                )
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                        new Claim(ClaimTypes.Email, account.Email),
                        new Claim(ClaimTypes.Role, account.Role),
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims,
                        CookieAuthenticationDefaults.AuthenticationScheme
                    );
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2),
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        claimsPrincipal,
                        authProperties
                    );

                    if (account.Role == "Admin")
                    {
                        return RedirectToAction("Index", "AdminDashboard");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");
            }

            return View(model);
        }

        // POST: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        public static int? GetCurrentUserId(HttpContext httpContext)
        {
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : null;
        }

        public static string GetCurrentUserRole(HttpContext httpContext)
        {
            return httpContext.User.FindFirst(ClaimTypes.Role)?.Value;
        }

        public static string GetCurrentUserEmail(HttpContext httpContext)
        {
            return httpContext.User.FindFirst(ClaimTypes.Email)?.Value;
        }
    }
}
