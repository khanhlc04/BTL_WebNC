using BTL_WebNC.Helpers;
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

        private readonly IFileHelper _fileHelper;

        public AdminTeacherController(
            ITeacherRepository teacherRepo,
            IAccountRepository accountRepo,
            ISubjectRepository subjectRepo,
            ITeacherSubjectRepository teacherSubjectRepo,
            IFileHelper fileHelper
        )
        {
            _teacherRepo = teacherRepo;
            _accountRepo = accountRepo;
            _subjectRepo = subjectRepo;
            _fileHelper = fileHelper;
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
            [FromForm] List<int> subjects,
            [FromForm] IFormFile thumbnailFile
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
                    Role = "Teacher",
                    Deleted = false,
                };

                var createdAccount = await _accountRepo.CreateAsync(account);

                string thumbnailPath = null;
                if (thumbnailFile != null)
                {
                    thumbnailPath = await _fileHelper.SaveFileAsync(thumbnailFile, "thumbnails");
                }

                var teacher = new TeacherModel
                {
                    FullName = fullName,
                    Email = email,
                    AccountId = createdAccount.Id,
                    ThumbnailPath = thumbnailPath,
                };

                Console.WriteLine("Creating teacher: " + teacher.AccountId);

                await _teacherRepo.CreateAsync(teacher);

                if (subjects != null)
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

                // Prepare DTO to return to client: teacher info + assigned subjects
                var allSubjects = await _subjectRepo.GetAllAsync();
                var assignedSubjects = new List<object>();
                if (subjects != null && subjects.Count > 0)
                {
                    assignedSubjects = allSubjects
                        .Where(s => subjects.Contains(s.Id))
                        .Select(s => new
                        {
                            id = s.Id,
                            name = s.Name,
                            thumbnailPath = s.ThumbnailPath,
                        })
                        .Cast<object>()
                        .ToList();
                }

                var teacherDto = new
                {
                    id = teacher.Id,
                    fullName = teacher.FullName,
                    email = teacher.Email,
                    thumbnailPath = teacher.ThumbnailPath,
                    accountId = teacher.AccountId,
                };

                return Json(
                    new
                    {
                        success = true,
                        message = "Tạo giảng viên thành công",
                        data = new { teacher = teacherDto, subjects = assignedSubjects },
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTeacher(
            [FromForm] int id,
            [FromForm] string fullName,
            [FromForm] string email,
            [FromForm] List<int> subjects,
            [FromForm] IFormFile thumbnailFile
        )
        {
            try
            {
                var existingTeacher = await _teacherRepo.GetByIdAsync(id);
                if (existingTeacher == null)
                    return NotFound(new { success = false, message = "Không tìm thấy giảng viên" });

                // Giữ giá trị hiện tại
                string thumbnailPath = existingTeacher.ThumbnailPath;

                // Nếu có file mới, lưu trước; nếu lưu thành công thì xóa file cũ
                if (thumbnailFile != null && thumbnailFile.Length > 0)
                {
                    var newPath = await _fileHelper.SaveFileAsync(thumbnailFile, "thumbnails");
                    if (!string.IsNullOrEmpty(newPath))
                    {
                        // xóa file cũ nếu khác
                        if (
                            !string.IsNullOrEmpty(existingTeacher.ThumbnailPath)
                            && !string.Equals(
                                existingTeacher.ThumbnailPath,
                                newPath,
                                StringComparison.OrdinalIgnoreCase
                            )
                        )
                        {
                            _fileHelper.DeleteFile(existingTeacher.ThumbnailPath);
                        }

                        thumbnailPath = newPath;
                    }
                }

                // Cập nhật entity đã load
                existingTeacher.FullName = fullName;
                existingTeacher.Email = email;
                existingTeacher.ThumbnailPath = thumbnailPath;

                var createdTeacher = await _teacherRepo.UpdateAsync(existingTeacher);

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
                            TeacherId = createdTeacher.Id,
                            SubjectId = subjectId,
                        };
                        await _teacherSubjectRepo.CreateAsync(teacherSubject);
                    }
                }

                // Prepare DTO to return to client: teacher info + assigned subjects (same shape as CreateTeacher)
                var allSubjects = await _subjectRepo.GetAllAsync();
                var assignedSubjects = new List<object>();
                if (subjects != null && subjects.Count > 0)
                {
                    assignedSubjects = allSubjects
                        .Where(s => subjects.Contains(s.Id))
                        .Select(s => new
                        {
                            id = s.Id,
                            name = s.Name,
                            thumbnailPath = s.ThumbnailPath,
                        })
                        .Cast<object>()
                        .ToList();
                }

                var teacherDto = new
                {
                    id = createdTeacher.Id,
                    fullName = createdTeacher.FullName,
                    email = createdTeacher.Email,
                    thumbnailPath = createdTeacher.ThumbnailPath,
                    accountId = createdTeacher.AccountId,
                };

                return Json(
                    new
                    {
                        success = true,
                        message = "Cập nhật giảng viên thành công",
                        data = new { teacher = teacherDto, subjects = assignedSubjects },
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTeacher([FromBody] int id)
        {
            try
            {
                var result = await _teacherRepo.DeleteAsync(id);
                return Json(new { success = true, message = "Xóa giảng viên thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
