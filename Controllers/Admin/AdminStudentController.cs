using BTL_WebNC.Models.Account;
using BTL_WebNC.Models.Student;
using BTL_WebNC.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BTL_WebNC.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminStudentController : Controller
    {
        private readonly IStudentRepository _studentRepo;
        private readonly IAccountRepository _accountRepo;

        public AdminStudentController(
            IStudentRepository studentRepo,
            IAccountRepository accountRepo
        )
        {
            _studentRepo = studentRepo;
            _accountRepo = accountRepo;
        }

        public async Task<IActionResult> Index()
        {
            var students = await _studentRepo.GetAllAsync();
            return View("~/Views/Admin/Student/Index.cshtml", students);
        }

        [HttpGet]
        public async Task<IActionResult> GetStudent(int id)
        {
            var student = await _studentRepo.GetByIdAsync(id);
            if (student == null)
                return NotFound(new { success = false, message = "Không tìm thấy sinh viên" });

            return Json(new { success = true, data = student });
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudent(
            [FromForm] string fullName,
            [FromForm] string email,
            [FromForm] string studentCode
        )
        {
            try
            {
                var existingAccount = await _accountRepo.GetByEmailAsync(email);
                if (existingAccount != null)
                {
                    return Json(new { success = false, message = "Email đã được sử dụng" });
                }

                var account = new AccountModel
                {
                    Email = email,
                    Password = "HOU123",
                    Role = "Student",
                    Deleted = false,
                };
                var createdAccount = await _accountRepo.CreateAsync(account);

                var student = new StudentModel
                {
                    FullName = fullName,
                    Email = email,
                    StudentCode = studentCode,
                    AccountId = createdAccount.Id,
                    Deleted = false,
                };
                var createdStudent = await _studentRepo.CreateAsync(student);

                return Json(new { success = true, message = "Tạo sinh viên thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStudent(
            [FromForm] int id,
            [FromForm] string fullName,
            [FromForm] string email,
            [FromForm] string studentCode
        )
        {
            try
            {
                var student = await _studentRepo.GetByIdAsync(id);
                if (student == null)
                {
                    return Json(new { success = false, message = "Sinh viên không tồn tại" });
                }

                // Cập nhật email account
                var account = await _accountRepo.GetByEmailAsync(email);

                if (account != null)
                {
                    account.Email = email;
                    await _accountRepo.UpdateAsync(account);
                }

                student.FullName = fullName;
                student.Email = email;
                student.StudentCode = studentCode;

                var updatedStudent = await _studentRepo.UpdateAsync(student);
                return Json(new { success = true, data = updatedStudent });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStudent([FromBody] int id)
        {
            try
            {
                var success = await _studentRepo.DeleteAsync(id);
                if (!success)
                {
                    return Json(new { success = false, message = "Sinh viên không tồn tại" });
                }

                return Json(new { success = true, message = "Xóa sinh viên thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
