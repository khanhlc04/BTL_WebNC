using BTLChatDemo.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace BTL_WebNC.Controllers.User
{
    public class UserTeacherController : Controller
    {
        private readonly ITeacherRepository _teacherRepo;
        private readonly ISubjectRepository _subjectRepo;

        public UserTeacherController(ITeacherRepository teacherRepo, ISubjectRepository subjectRepo)
        {
            _teacherRepo = teacherRepo;
            _subjectRepo = subjectRepo;
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString, int? subjectId)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            bool isStudent = !string.IsNullOrEmpty(userEmail) && userEmail.EndsWith("@students.hou.edu.vn");

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
                teachers = teachers.Where(t => t.TeacherSubjects.Any(ts => ts.SubjectId == subjectId.Value)).ToList();
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                teachers = teachers.Where(t => t.FullName.ToLower().Contains(searchString.ToLower())).ToList();
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
            
            return View("~/Views/User/TeacherList.cshtml", teachers);
        }
    }
}