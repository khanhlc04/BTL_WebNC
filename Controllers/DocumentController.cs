using System.IO;
using BTL_WebNC.Models.Document;
using BTL_WebNC.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace BTL_WebNC.Controllers.Document
{
    public class DocumentController : Controller
    {
        private readonly IDocumentRepository _documentRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DocumentController(
            IDocumentRepository documentRepo,
            IWebHostEnvironment webHostEnvironment
        )
        {
            _documentRepo = documentRepo;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(string searchTerm, List<int>? subjectIds)
        {
            var documents = await _documentRepo.GetAllAsync();

            // Lọc theo search term
            if (!string.IsNullOrEmpty(searchTerm))
            {
                documents = documents.Where(d =>
                    d.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                    || (
                        d.FileName != null
                        && d.FileName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                    )
                );
            }

            // Lọc theo môn học
            if (subjectIds != null && subjectIds.Any())
            {
                documents = documents.Where(d => subjectIds.Contains(d.SubjectId));
            }

            ViewBag.Count = documents.Count();
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SelectedSubjects = subjectIds ?? new List<int>();

            var subjects = await _documentRepo.GetSubjectsAsync();
            ViewBag.Subjects = subjects;

            return View("~/Views/Document/Document.cshtml", documents);
        }

        // Download file tài liệu
        public async Task<IActionResult> Download(int id)
        {
            var document = await _documentRepo.GetByIdAsync(id);

            if (document == null || string.IsNullOrEmpty(document.FilePath))
                return NotFound("Không tìm thấy tài liệu");

            // Đường dẫn file đầy đủ
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, document.FilePath);

            // Kiểm tra file có tồn tại không
            if (!System.IO.File.Exists(filePath))
                return NotFound("File không tồn tại");

            try
            {
                // Đọc file vào memory
                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                // Lấy tên file an toàn
                var fileName = !string.IsNullOrEmpty(document.FileName)
                    ? document.FileName
                    : GetSafeFileName(document.Title, Path.GetExtension(filePath));

                // Lấy content type
                var contentType = GetContentType(filePath);

                // Trả về file để download
                return File(memory, contentType, fileName);
            }
            catch (Exception ex)
            {
                // Log error nếu cần
                return StatusCode(500, "Lỗi khi tải file: " + ex.Message);
            }
        }

        // Xem file online (cho PDF, image)
        public async Task<IActionResult> View(int id)
        {
            var document = await _documentRepo.GetByIdAsync(id);

            if (document == null || string.IsNullOrEmpty(document.FilePath))
                return NotFound("Không tìm thấy tài liệu");

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, document.FilePath);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File không tồn tại");

            var contentType = GetContentType(filePath);

            // Chỉ cho phép xem trực tiếp PDF và hình ảnh
            if (contentType.StartsWith("application/pdf") || contentType.StartsWith("image/"))
            {
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                return File(fileBytes, contentType);
            }

            // Các file khác thì download
            return RedirectToAction("Download", new { id });
        }

        // API để lấy thông tin file (dùng cho AJAX nếu cần)
        [HttpGet]
        public async Task<IActionResult> GetFileInfo(int id)
        {
            var document = await _documentRepo.GetByIdAsync(id);

            if (document == null)
                return NotFound(new { success = false, message = "Không tìm thấy tài liệu" });

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, document.FilePath);

            if (!System.IO.File.Exists(filePath))
                return NotFound(new { success = false, message = "File không tồn tại" });

            var fileInfo = new FileInfo(filePath);
            var sizeInMB = (fileInfo.Length / (1024.0 * 1024.0)).ToString("0.##");

            return Json(
                new
                {
                    success = true,
                    data = new
                    {
                        id = document.Id,
                        title = document.Title,
                        fileName = document.FileName,
                        fileSize = sizeInMB + " MB",
                        extension = fileInfo.Extension,
                        subjectId = document.SubjectId,
                        thumbnailPath = document.ThumbnailPath,
                    },
                }
            );
        }

        private string GetSafeFileName(string title, string extension)
        {
            // Loại bỏ ký tự không hợp lệ
            var invalidChars = Path.GetInvalidFileNameChars();
            var safeTitle = string.Join(
                "_",
                title.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries)
            );

            // Giới hạn độ dài tên file
            if (safeTitle.Length > 100)
                safeTitle = safeTitle.Substring(0, 100);

            return safeTitle + extension;
        }

        private string GetContentType(string path)
        {
            var types = new Dictionary<string, string>
            {
                // Documents
                { ".pdf", "application/pdf" },
                { ".doc", "application/vnd.ms-word" },
                {
                    ".docx",
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                },
                { ".xls", "application/vnd.ms-excel" },
                { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                { ".ppt", "application/vnd.ms-powerpoint" },
                {
                    ".pptx",
                    "application/vnd.openxmlformats-officedocument.presentationml.presentation"
                },
                // Archives
                { ".zip", "application/zip" },
                { ".rar", "application/x-rar-compressed" },
                { ".7z", "application/x-7z-compressed" },
                // Text
                { ".txt", "text/plain" },
                { ".csv", "text/csv" },
                // Images
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".png", "image/png" },
                { ".gif", "image/gif" },
                { ".bmp", "image/bmp" },
                { ".svg", "image/svg+xml" },
            };

            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types.ContainsKey(ext) ? types[ext] : "application/octet-stream";
        }
    }
}
