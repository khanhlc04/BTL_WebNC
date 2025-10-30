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

        public async Task<IActionResult> Index(string searchString)
        {
            var subjects = await _subjectRepo.GetAllAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                subjects = subjects
                    .Where(s => s.Name.ToLower().Contains(searchString.ToLower()))
                    .ToList();
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
                .Take(3)
                .ToList();

            // get teachers that are linked to this subject via TeacherSubject join table
            var tsLinks = await _teacherSubjectRepo.GetBySubjectIdAsync(id);
            var teacherIds = tsLinks.Select(ts => ts.TeacherId).Distinct().ToList();
            var teachers = new List<TeacherModel>();
            foreach (var tid in teacherIds.Take(3))
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
