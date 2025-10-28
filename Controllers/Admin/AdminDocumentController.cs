using BTL_WebNC.Helpers;
using BTL_WebNC.Models.Document;
using BTL_WebNC.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BTL_WebNC.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminDocumentController : Controller
    {
        private readonly IDocumentRepository _documentRepo;
        private readonly IFileHelper _fileHelper;

        private readonly ISubjectRepository _subjectRepo;

        public AdminDocumentController(
            IDocumentRepository documentRepo,
            ISubjectRepository subjectRepo,
            IFileHelper fileHelper
        )
        {
            _documentRepo = documentRepo;
            _fileHelper = fileHelper;
            _subjectRepo = subjectRepo;
        }

        public async Task<IActionResult> Index()
        {
            var subjects = await _subjectRepo.GetAllAsync();

            ViewBag.Subjects = subjects;

            var documents = await _documentRepo.GetAllAsync();
            return View("~/Views/Admin/Document/Index.cshtml", documents);
        }

        // API endpoint để lấy document theo ID
        [HttpGet]
        public async Task<IActionResult> GetDocument(int id)
        {
            var document = await _documentRepo.GetByIdAsync(id);
            if (document == null)
                return NotFound(new { success = false, message = "Không tìm thấy tài liệu" });

            return Json(new { success = true, data = document });
        }

        // API endpoint để tạo document với file upload
        [HttpPost]
        public async Task<IActionResult> CreateDocument(
            [FromForm] string title,
            [FromForm] IFormFile thumbnailFile,
            [FromForm] IFormFile documentFile,
            [FromForm] int subjectId
        )
        {
            try
            {
                if (documentFile == null || documentFile.Length == 0)
                    return Json(new { success = false, message = "Vui lòng chọn file tài liệu" });

                // Upload file tài liệu
                string documentPath = await _fileHelper.SaveFileAsync(documentFile, "documents");

                // Upload thumbnail (nếu có)
                string thumbnailPath = null;
                if (thumbnailFile != null && thumbnailFile.Length > 0)
                {
                    thumbnailPath = await _fileHelper.SaveFileAsync(thumbnailFile, "thumbnails");
                }

                var model = new DocumentModel
                {
                    Title = title,
                    ThumbnailPath = thumbnailPath,
                    FilePath = documentPath,
                    FileName = documentFile.FileName,
                    SubjectId = subjectId,
                };

                await _documentRepo.CreateAsync(model);
                return Json(new { success = true, message = "Thêm tài liệu thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // API endpoint để cập nhật document
        [HttpPost]
        public async Task<IActionResult> UpdateDocument(
            [FromForm] int id,
            [FromForm] string title,
            [FromForm] IFormFile thumbnailFile,
            [FromForm] IFormFile documentFile,
            [FromForm] int subjectId
        )
        {
            try
            {
                var existingDoc = await _documentRepo.GetByIdAsync(id);
                if (existingDoc == null)
                    return NotFound(new { success = false, message = "Không tìm thấy tài liệu" });

                // Cập nhật file tài liệu nếu có file mới
                if (documentFile != null && documentFile.Length > 0)
                {
                    // Xóa file cũ
                    _fileHelper.DeleteFile(existingDoc.FilePath);

                    // Upload file mới
                    existingDoc.FilePath = await _fileHelper.SaveFileAsync(
                        documentFile,
                        "documents"
                    );
                    existingDoc.FileName = documentFile.FileName;
                }

                // Cập nhật thumbnail nếu có file mới
                if (thumbnailFile != null && thumbnailFile.Length > 0)
                {
                    // Xóa thumbnail cũ
                    if (!string.IsNullOrEmpty(existingDoc.ThumbnailPath))
                        _fileHelper.DeleteFile(existingDoc.ThumbnailPath);

                    // Upload thumbnail mới
                    existingDoc.ThumbnailPath = await _fileHelper.SaveFileAsync(
                        thumbnailFile,
                        "thumbnails"
                    );
                }

                existingDoc.Title = title;
                existingDoc.SubjectId = subjectId;

                await _documentRepo.UpdateAsync(existingDoc);
                return Json(new { success = true, message = "Cập nhật tài liệu thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // API endpoint để xóa document
        [HttpPost]
        public async Task<IActionResult> DeleteDocument([FromBody] int id)
        {
            try
            {
                var document = await _documentRepo.GetByIdAsync(id);
                if (document == null)
                    return NotFound(new { success = false, message = "Không tìm thấy tài liệu" });

                // Xóa các file
                _fileHelper.DeleteFile(document.FilePath);
                if (!string.IsNullOrEmpty(document.ThumbnailPath))
                    _fileHelper.DeleteFile(document.ThumbnailPath);

                await _documentRepo.DeleteAsync(id);
                return Json(new { success = true, message = "Xóa tài liệu thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // Download file
        public async Task<IActionResult> DownloadFile(int id)
        {
            var document = await _documentRepo.GetByIdAsync(id);
            if (document == null)
                return NotFound();

            var filePath = _fileHelper.GetAbsolutePath(document.FilePath);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, _fileHelper.GetContentType(filePath), document.FileName);
        }
    }
}
