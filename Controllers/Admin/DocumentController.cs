using BTLChatDemo.Models.Document;
using BTLChatDemo.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BTL_WebNC.Controllers.Admin
{
    [Area("Admin")]
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
            return View(documents);
        }

        public IActionResult CreateDocument() => View();

        [HttpPost]
        public async Task<IActionResult> CreateDocument(DocumentModel model)
        {
            await _documentRepo.CreateAsync(model);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> EditDocument(int id)
        {
            var document = await _documentRepo.GetByIdAsync(id);
            return View(document);
        }

        [HttpPost]
        public async Task<IActionResult> EditDocument(DocumentModel model)
        {
            await _documentRepo.UpdateAsync(model);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteDocument(int id)
        {
            var document = await _documentRepo.GetByIdAsync(id);
            return View(document);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDocumentConfirmed(int id)
        {
            await _documentRepo.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
