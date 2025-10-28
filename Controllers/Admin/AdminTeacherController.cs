using BTL_WebNC.Models.Account;
using BTL_WebNC.Models.Teacher;
using BTL_WebNC.Models.TeacherSubject;
using BTL_WebNC.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BTL_WebNC.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminTeacherController : Controller
    {
        private readonly ITeacherRepository _teacherRepo;
        private readonly IAccountRepository _accountRepo;

        private readonly ISubjectRepository _subjectRepo;
        private readonly ITeacherSubjectRepository _teacherSubjectRepo;

        public AdminTeacherController(
            ITeacherRepository teacherRepo,
            IAccountRepository accountRepo,
            ISubjectRepository subjectRepo,
            ITeacherSubjectRepository teacherSubjectRepo
        )
        {
            _teacherRepo = teacherRepo;
            _accountRepo = accountRepo;
            _subjectRepo = subjectRepo;
            _teacherSubjectRepo = teacherSubjectRepo;
        }

        public async Task<IActionResult> Index()
        {
            var teachers = await _teacherRepo.GetAllAsync();

            var subjects = await _subjectRepo.GetAllAsync();
            ViewBag.Subjects = subjects;

            return View("~/Views/Admin/Teacher/Index.cshtml", teachers);
        }

        [HttpGet]
        public async Task<IActionResult> GetTeacher(int id)
        {
            var teacher = await _teacherRepo.GetByIdAsync(id);
            if (teacher == null)
                return NotFound(new { success = false, message = "Không tìm thấy giảng viên" });

            return Json(new { success = true, data = teacher });
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeacher(
            [FromForm] string fullName,
            [FromForm] string email,
            [FromForm] string password,
            [FromForm] List<int> subjects
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
                    Password = password,
                    Role = "User",
                    Deleted = false,
                };
                await _accountRepo.CreateAsync(account);

                var teacher = new TeacherModel
                {
                    FullName = fullName,
                    Email = email,
                    AccountId = account.Id,
                };
                await _teacherRepo.CreateAsync(teacher);

                Console.WriteLine(
                    $"Created teacher Id={teacher.Id}, subjects={(subjects != null ? string.Join(',', subjects) : "null")}"
                );

                if (subjects != null)
                {
                    foreach (var subjectId in subjects)
                    {
                        Console.WriteLine(
                            $"Creating TeacherSubject -> TeacherId={teacher.Id}, SubjectId={subjectId}"
                        );
                        var teacherSubject = new TeacherSubjectModel
                        {
                            TeacherId = teacher.Id,
                            SubjectId = subjectId,
                        };
                        await _teacherSubjectRepo.CreateAsync(teacherSubject);
                    }
                }

                return Json(new { success = true, message = "Tạo giảng viên thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new
                    {
                        success = false,
                        message = ex.Message,
                        detail = ex.InnerException?.Message,
                        trace = ex.ToString(),
                    }
                );
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTeacher(
            [FromForm] int id,
            [FromForm] string fullName,
            [FromForm] string email,
            [FromForm] List<int> subjects
        )
        {
            try
            {
                var teacher = new TeacherModel
                {
                    Id = id,
                    FullName = fullName,
                    Email = email,
                };
                await _teacherRepo.UpdateAsync(teacher);

                var account = await _accountRepo.GetByEmailAsync(email);

                if (account != null)
                {
                    account.Email = email;
                    await _accountRepo.UpdateAsync(account);
                }

                // Cập nhật môn học cho giảng viên
                var existingSubjects = await _teacherSubjectRepo.GetByTeacherIdAsync(id);
                foreach (var es in existingSubjects)
                {
                    await _teacherSubjectRepo.DeleteAsync(es.Id);
                }

                if (subjects != null && subjects.Count > 0)
                {
                    foreach (var subjectId in subjects)
                    {
                        var teacherSubject = new TeacherSubjectModel
                        {
                            TeacherId = teacher.Id,
                            SubjectId = subjectId,
                        };
                        await _teacherSubjectRepo.CreateAsync(teacherSubject);
                    }
                }

                return Json(new { success = true, message = "Cập nhật giảng viên thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTeacher([FromForm] int id)
        {
            try
            {
                await _teacherRepo.DeleteAsync(id);
                return Json(new { success = true, message = "Xóa giảng viên thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
