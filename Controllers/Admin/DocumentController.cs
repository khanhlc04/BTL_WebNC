using BTLChatDemo.Models.Document;
using BTLChatDemo.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BTL_WebNC.Controllers.Admin
{
    [Route("Admin/[controller]")]
    public class DocumentController : Controller
    {
        private readonly IDocumentRepository _documentRepo;

        public DocumentController(IDocumentRepository documentRepo)
        {
            _documentRepo = documentRepo;
        }

        public async Task<IActionResult> Index()
        {
            var documents = await _documentRepo.GetAllAsync();
            return View("~/Views/Admin/Document/Index.cshtml", documents);
        }

        public IActionResult CreateDocument() =>
            View("~/Views/Admin/Document/CreateDocument.cshtml");

        [HttpPost]
        public async Task<IActionResult> CreateDocument(
            string Title,
            string ThumbnailUrl,
            string FileUrl,
            int SubjectId
        )
        {
            var model = new DocumentModel
            {
                Title = Title,
                ThumbnailUrl = ThumbnailUrl,
                FileUrl = FileUrl,
                SubjectId = SubjectId,
            };
            await _documentRepo.CreateAsync(model);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> EditDocument(int id)
        {
            var document = await _documentRepo.GetByIdAsync(id);
            return View("~/Views/Admin/Document/EditDocument.cshtml", document);
        }

        [HttpPost]
        public async Task<IActionResult> EditDocument(
            int Id,
            string Title,
            string ThumbnailUrl,
            string FileUrl,
            int SubjectId
        )
        {
            var model = await _documentRepo.GetByIdAsync(Id);
            if (model != null)
            {
                model.Title = Title;
                model.ThumbnailUrl = ThumbnailUrl;
                model.FileUrl = FileUrl;
                model.SubjectId = SubjectId;
                await _documentRepo.UpdateAsync(model);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteDocument(int id)
        {
            var document = await _documentRepo.GetByIdAsync(id);
            return View("~/Views/Admin/Document/DeleteDocument.cshtml", document);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDocumentConfirmed(int id)
        {
            await _documentRepo.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
