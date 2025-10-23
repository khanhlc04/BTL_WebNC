using BTL_WebNC.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BTL_WebNC.Controllers
{
    public class UserSubjectController : Controller
    {
        private readonly ISubjectRepository _subjectRepo;

        public UserSubjectController(ISubjectRepository subjectRepo)
        {
            _subjectRepo = subjectRepo;
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            string currentSortOrder = string.IsNullOrEmpty(sortOrder) ? "id_asc" : sortOrder;

            ViewData["CurrentSort"] = currentSortOrder;
            ViewData["CurrentFilter"] = searchString;

            var subjects = await _subjectRepo.GetAllAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                subjects = subjects.Where(s => s.Name.ToLower().Contains(searchString.ToLower())).ToList();
            }

            switch (currentSortOrder)
            {
                case "name_asc":
                    subjects = subjects.OrderBy(s => s.Name).ToList();
                    break;
                case "name_desc":
                    subjects = subjects.OrderByDescending(s => s.Name).ToList();
                    break;
                case "id_desc":
                    subjects = subjects.OrderByDescending(s => s.Id).ToList();
                    break;
                default:
                    subjects = subjects.OrderBy(s => s.Id).ToList();
                    break;
            }

            return View("~/Views/User/SubjectList.cshtml", subjects);
        }
    }
}