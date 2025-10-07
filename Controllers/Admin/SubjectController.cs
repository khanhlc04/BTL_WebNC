using BTLChatDemo.Models.Subject;
using BTLChatDemo.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BTL_WebNC.Controllers.Admin
{
    [Area("Admin")]
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
            return View(subjects);
        }

        public IActionResult CreateSubject() => View();

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
            return View(subject);
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
            return View(subject);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSubjectConfirmed(int id)
        {
            await _subjectRepo.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
