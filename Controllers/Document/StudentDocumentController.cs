using BTLChatDemo.Models.Document;
using BTLChatDemo.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;


namespace BTLChatDemo.Controllers.Document
{
    public class StudentDocumentController : Controller
    {

        private readonly IDocumentRepository _documentRepo;
        public StudentDocumentController(IDocumentRepository documentRepo)
        {
            _documentRepo = documentRepo;
        }
        //public async Task<IActionResult> Index()
        //{
        //    var documents = await _documentRepo.GetAllAsync() ?? new List<DocumentModel>(); ;
        //    ViewBag.Count = documents.Count();
        //    return View("~/Views/Document/Document.cshtml", documents);
        //}


        public async Task<IActionResult> Index(string searchTerm, List<int>? subjectIds)
        {
            var documents = await _documentRepo.GetAllAsync();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                documents = documents.Where(d => d.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

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
        public async Task<IActionResult> Download(int id)
        {
            var document = await _documentRepo.GetByIdAsync(id);

            if (document == null || string.IsNullOrEmpty(document.FileUrl))
                return NotFound();

            if (document.FileUrl.StartsWith("http"))
            {
                using (var httpClient = new HttpClient())
                {
                    var fileBytes = await httpClient.GetByteArrayAsync(document.FileUrl);
                    var safeTitle = string.Join("_", document.Title.Split(Path.GetInvalidFileNameChars()));
                    var fileName = $"{safeTitle}.pdf";

                    return File(fileBytes, "application/pdf", fileName);
                }
            }
            return NotFound();
        }
    }
}

