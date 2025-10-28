using BTL_WebNC.Models.Subject;
using BTL_WebNC.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BTL_WebNC.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminSubjectController : Controller
    {
        private readonly ISubjectRepository _subjectRepo;

        public AdminSubjectController(ISubjectRepository subjectRepo)
        {
            _subjectRepo = subjectRepo;
        }

        public async Task<IActionResult> Index()
        {
            var subjects = await _subjectRepo.GetAllAsync();
            return View("~/Views/Admin/Subject/Index.cshtml", subjects);
        }

        [HttpGet]
        public async Task<IActionResult> GetSubject(int id)
        {
            var subject = await _subjectRepo.GetByIdAsync(id);
            if (subject == null)
                return NotFound(new { success = false, message = "Không tìm thấy môn học" });

            return Json(new { success = true, data = subject });
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubject(
            [FromForm] string name,
            [FromForm] string description
        )
        {
            try
            {
                var subject = new SubjectModel { Name = name, Description = description };
                await _subjectRepo.CreateAsync(subject);
                return Json(new { success = true, message = "Tạo môn học thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSubject(
            [FromForm] int id,
            [FromForm] string name,
            [FromForm] string description
        )
        {
            try
            {
                var subject = new SubjectModel
                {
                    Id = id,
                    Name = name,
                    Description = description,
                };
                await _subjectRepo.UpdateAsync(subject);
                return Json(new { success = true, message = "Cập nhật môn học thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSubject([FromForm] int id)
        {
            try
            {
                await _subjectRepo.DeleteAsync(id);
                return Json(new { success = true, message = "Xóa môn học thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
