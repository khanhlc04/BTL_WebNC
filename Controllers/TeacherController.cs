using System.Security.Claims;
using BTL_WebNC.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BTL_WebNC.Controllers.User
{
    public class TeacherController : Controller
    {
        private readonly ITeacherRepository _teacherRepo;
        private readonly ISubjectRepository _subjectRepo;
        private readonly ITeacherSubjectRepository _teacherSubjectRepo;

        public TeacherController(
            ITeacherRepository teacherRepo,
            ISubjectRepository subjectRepo,
            ITeacherSubjectRepository teacherSubjectRepo
        )
        {
            _teacherRepo = teacherRepo;
            _subjectRepo = subjectRepo;
            _teacherSubjectRepo = teacherSubjectRepo;
        }

        public async Task<IActionResult> Index(
            string sortOrder,
            string searchString,
            int? subjectId
        )
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            bool isStudent =
                !string.IsNullOrEmpty(userEmail) && userEmail.EndsWith("@students.hou.edu.vn");

            ViewData["IsStudent"] = isStudent;

            string currentSortOrder = string.IsNullOrEmpty(sortOrder) ? "id_asc" : sortOrder;
            ViewData["CurrentSort"] = currentSortOrder;
            ViewData["CurrentFilter"] = searchString;

            var subjects = await _subjectRepo.GetAllAsync();
            ViewBag.Subjects = new SelectList(subjects, "Id", "Name", subjectId);
            ViewData["CurrentSubjectId"] = subjectId;

            var teachers = await _teacherRepo.GetAllAsync();

            if (subjectId.HasValue && subjectId > 0)
            {
                teachers = teachers
                    .Where(t => t.TeacherSubjects.Any(ts => ts.SubjectId == subjectId.Value))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                teachers = teachers
                    .Where(t => t.FullName.ToLower().Contains(searchString.ToLower()))
                    .ToList();
            }

            switch (currentSortOrder)
            {
                case "name_asc":
                    teachers = teachers.OrderBy(t => t.FullName).ToList();
                    break;
                case "name_desc":
                    teachers = teachers.OrderByDescending(t => t.FullName).ToList();
                    break;
                case "id_desc":
                    teachers = teachers.OrderByDescending(t => t.Id).ToList();
                    break;
                default:
                    teachers = teachers.OrderBy(t => t.Id).ToList();
                    break;
            }

            return View("~/Views/Teacher/TeacherList.cshtml", teachers);
        }

        public async Task<IActionResult> Details(int id)
        {
            var teacher = await _teacherRepo.GetByIdAsync(id);
            if (teacher == null)
                return NotFound();

            // get subject links for this teacher
            var tsLinks = await _teacherSubjectRepo.GetByTeacherIdAsync(id);
            var subjectIds = tsLinks.Select(ts => ts.SubjectId).Distinct().ToList();

            var subjects = new List<BTL_WebNC.Models.Subject.SubjectModel>();
            foreach (var sid in subjectIds)
            {
                var s = await _subjectRepo.GetByIdAsync(sid);
                if (s != null)
                    subjects.Add(s);
            }

            ViewBag.Teacher = teacher;
            ViewBag.Subjects = subjects;
            return View("~/Views/Teacher/TeacherDetails.cshtml");
        }
    }
}
