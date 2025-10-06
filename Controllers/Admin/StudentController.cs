using BTLChatDemo.Models.Account;
using BTLChatDemo.Models.Student;
using BTLChatDemo.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BTL_WebNC.Controllers.Admin
{
    [Area("Admin")]
    public class StudentController : Controller
    {
        private readonly IStudentRepository _studentRepo;
        private readonly IAccountRepository _accountRepo;

        public StudentController(IStudentRepository studentRepo, IAccountRepository accountRepo)
        {
            _studentRepo = studentRepo;
            _accountRepo = accountRepo;
        }

        public async Task<IActionResult> Index()
        {
            var students = await _studentRepo.GetAllAsync();
            return View(students);
        }

        public IActionResult CreateStudent() => View();

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> CreateStudent(
            string FullName,
            string Email,
            string Password,
            string StudentCode
        )
        {
            var account = new AccountModel { Email = Email, Password = Password };
            var createdAccount = await _accountRepo.CreateAsync(account);

            var student = new StudentModel
            {
                FullName = FullName,
                Email = Email,
                StudentCode = StudentCode,
                AccountId = createdAccount.Id,
            };
            await _studentRepo.CreateAsync(student);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> EditStudent(int id)
        {
            var student = await _studentRepo.GetByIdAsync(id);
            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> EditStudent(StudentModel model)
        {
            await _studentRepo.UpdateAsync(model);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _studentRepo.GetByIdAsync(id);
            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStudentConfirmed(int id)
        {
            await _studentRepo.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
