using BTLChatDemo.Models.Account;
using BTLChatDemo.Models.Teacher;
using BTLChatDemo.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BTL_WebNC.Controllers.Admin
{
    [Area("Admin")]
    public class TeacherController : Controller
    {
        private readonly ITeacherRepository _teacherRepo;
        private readonly IAccountRepository _accountRepo;

        public TeacherController(ITeacherRepository teacherRepo, IAccountRepository accountRepo)
        {
            _teacherRepo = teacherRepo;
            _accountRepo = accountRepo;
        }

        public async Task<IActionResult> Index()
        {
            var teachers = await _teacherRepo.GetAllAsync();
            return View(teachers);
        }

        public IActionResult CreateTeacher() => View();

        [HttpPost]
        public async Task<IActionResult> CreateTeacher(
            string FullName,
            string Email,
            string Password
        )
        {
            var account = new AccountModel { Email = Email, Password = Password };
            var createdAccount = await _accountRepo.CreateAsync(account);

            var teacher = new TeacherModel
            {
                FullName = FullName,
                Email = Email,
                AccountId = createdAccount.Id,
            };
            await _teacherRepo.CreateAsync(teacher);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> EditTeacher(int id)
        {
            var teacher = await _teacherRepo.GetByIdAsync(id);
            return View(teacher);
        }

        [HttpPost]
        public async Task<IActionResult> EditTeacher(TeacherModel model)
        {
            await _teacherRepo.UpdateAsync(model);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var teacher = await _teacherRepo.GetByIdAsync(id);
            return View(teacher);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTeacherConfirmed(int id)
        {
            await _teacherRepo.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
