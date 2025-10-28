using System.Collections.Generic;
using System.Linq;
using BTL_WebNC.Models.Teacher;
using BTL_WebNC.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BTL_WebNC.Controllers
{
    public class SubjectController : Controller
    {
        private readonly ISubjectRepository _subjectRepo;
        private readonly IDocumentRepository _documentRepo;
        private readonly ITeacherRepository _teacherRepo;
        private readonly ITeacherSubjectRepository _teacherSubjectRepo;

        public SubjectController(
            ISubjectRepository subjectRepo,
            IDocumentRepository documentRepo,
            ITeacherRepository teacherRepo,
            ITeacherSubjectRepository teacherSubjectRepo
        )
        {
            _subjectRepo = subjectRepo;
            _documentRepo = documentRepo;
            _teacherRepo = teacherRepo;
            _teacherSubjectRepo = teacherSubjectRepo;
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            string currentSortOrder = string.IsNullOrEmpty(sortOrder) ? "id_asc" : sortOrder;

            ViewData["CurrentSort"] = currentSortOrder;
            ViewData["CurrentFilter"] = searchString;

            var subjects = await _subjectRepo.GetAllAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                subjects = subjects
                    .Where(s => s.Name.ToLower().Contains(searchString.ToLower()))
                    .ToList();
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

            ViewBag.Subjects = subjects;
            return View("~/Views/Subject/SubjectList.cshtml");
        }

        public async Task<IActionResult> Details(int id)
        {
            var subject = await _subjectRepo.GetByIdAsync(id);
            if (subject == null)
                return NotFound();

            // get documents for this subject (repository provides GetAllAsync)
            var allDocs = await _documentRepo.GetAllAsync();
            var docs = allDocs
                .Where(d => d.SubjectId == id && !d.Deleted)
                .OrderBy(d => d.Title)
                .Take(4)
                .ToList();

            // get teachers that are linked to this subject via TeacherSubject join table
            var tsLinks = await _teacherSubjectRepo.GetBySubjectIdAsync(id);
            var teacherIds = tsLinks.Select(ts => ts.TeacherId).Distinct().ToList();
            var teachers = new List<TeacherModel>();
            foreach (var tid in teacherIds.Take(4))
            {
                var t = await _teacherRepo.GetByIdAsync(tid);
                if (t != null)
                    teachers.Add(t);
            }

            ViewBag.Subject = subject;
            ViewBag.Documents = docs;
            ViewBag.Teachers = teachers;

            return View("~/Views/Subject/SubjectDetails.cshtml");
        }
    }
}
