using BTLChatDemo.Models.Subject;
using BTLChatDemo.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BTL_WebNC.Controllers.Admin
{
    [Route("Admin/[controller]")]
    public class SubjectController : Controller
    {
        private readonly ISubjectRepository _subjectRepo;

        public SubjectController(ISubjectRepository subjectRepo)
        {
            _subjectRepo = subjectRepo;
        }

        public async Task<IActionResult> Index()
        {
            var subjects = await _subjectRepo.GetAllAsync();
            return View("~/Views/Admin/Subject/Index.cshtml", subjects);
        }

        public IActionResult CreateSubject() => View("~/Views/Admin/Subject/CreateSubject.cshtml");

        [HttpPost]
        public async Task<IActionResult> CreateSubject(string Name, string Description)
        {
            var model = new SubjectModel { Name = Name, Description = Description };
            await _subjectRepo.CreateAsync(model);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> EditSubject(int id)
        {
            var subject = await _subjectRepo.GetByIdAsync(id);
            return View("~/Views/Admin/Subject/EditSubject.cshtml", subject);
        }

        [HttpPost]
        public async Task<IActionResult> EditSubject(int Id, string Name, string Description)
        {
            var model = await _subjectRepo.GetByIdAsync(Id);
            if (model != null)
            {
                model.Name = Name;
                model.Description = Description;
                await _subjectRepo.UpdateAsync(model);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteSubject(int id)
        {
            var subject = await _subjectRepo.GetByIdAsync(id);
            return View("~/Views/Admin/Subject/DeleteSubject.cshtml", subject);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSubjectConfirmed(int id)
        {
            await _subjectRepo.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
