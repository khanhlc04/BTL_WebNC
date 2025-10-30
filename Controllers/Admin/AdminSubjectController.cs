using BTL_WebNC.Helpers;
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
        private readonly IFileHelper _fileHelper;

        public AdminSubjectController(ISubjectRepository subjectRepo, IFileHelper fileHelper)
        {
            _subjectRepo = subjectRepo;
            _fileHelper = fileHelper;
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
            [FromForm] string description,
            [FromForm] IFormFile thumbnailFile
        )
        {
            try
            {
                string thumbnailPath = null;
                if (thumbnailFile != null && thumbnailFile.Length > 0)
                {
                    thumbnailPath = await _fileHelper.SaveFileAsync(thumbnailFile, "thumbnails");
                }
                var subject = new SubjectModel
                {
                    Name = name,
                    Description = description,
                    ThumbnailPath = thumbnailPath,
                };
                var result = await _subjectRepo.CreateAsync(subject);
                return Json(
                    new
                    {
                        success = true,
                        message = "Tạo môn học thành công",
                        data = result,
                    }
                );
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
            [FromForm] string description,
            [FromForm] IFormFile thumbnailFile
        )
        {
            try
            {
                var existingSubject = await _subjectRepo.GetByIdAsync(id);
                if (existingSubject == null)
                    return NotFound(new { success = false, message = "Không tìm thấy môn học" });

                if (thumbnailFile != null && thumbnailFile.Length > 0)
                {
                    existingSubject.ThumbnailPath = await _fileHelper.SaveFileAsync(
                        thumbnailFile,
                        "thumbnails"
                    );
                }

                existingSubject.Name = name;
                existingSubject.Description = description;

                var result = await _subjectRepo.UpdateAsync(existingSubject);
                return Json(
                    new
                    {
                        success = true,
                        message = "Cập nhật môn học thành công",
                        data = result,
                    }
                );
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
