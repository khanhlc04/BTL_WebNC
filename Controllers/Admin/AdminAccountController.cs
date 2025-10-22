using BTLChatDemo.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BTL_WebNC.Controllers.Admin
{
    public class AdminAccountController : Controller
    {
        private readonly IAccountRepository _accountRepo;

        public AdminAccountController(IAccountRepository accountRepo)
        {
            _accountRepo = accountRepo;
        }

        public async Task<IActionResult> Index()
        {
            var accounts = await _accountRepo.GetAllAsync();
            return View("~/Views/Admin/Account/Index.cshtml", accounts);
        }

        public async Task<IActionResult> DeleteAccount(int id)
        {
            var account = await _accountRepo.GetByIdAsync(id);
            return View("~/Views/Admin/Account/DeleteAccount.cshtml", account);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccountConfirmed(int id)
        {
            await _accountRepo.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
