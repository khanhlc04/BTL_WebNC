using BTLChatDemo.Models.Account;
using BTLChatDemo.Models.Teacher;
using BTLChatDemo.Models.TeacherSubject;
using BTLChatDemo.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BTL_WebNC.Controllers.Admin
{
    public class AdminTeacherController : Controller
    {
        private readonly ITeacherRepository _teacherRepo;
        private readonly IAccountRepository _accountRepo;

        private readonly ISubjectRepository _subjectRepo;
        private readonly ITeacherSubjectRepository _teacherSubjectRepo;

        public AdminTeacherController(
            ITeacherRepository teacherRepo,
            IAccountRepository accountRepo,
            ISubjectRepository subjectRepo,
            ITeacherSubjectRepository teacherSubjectRepo
        )
        {
            _teacherRepo = teacherRepo;
            _accountRepo = accountRepo;
            _subjectRepo = subjectRepo;
            _teacherSubjectRepo = teacherSubjectRepo;
        }

        public async Task<IActionResult> Index()
        {
            var teachers = await _teacherRepo.GetAllAsync();

            return View("~/Views/Admin/Teacher/Index.cshtml", teachers);
        }

        public async Task<IActionResult> CreateTeacher()
        {
            ViewBag.Subjects = await _subjectRepo.GetAllAsync();
            return View("~/Views/Admin/Teacher/CreateTeacher.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeacher(
            string FullName,
            string Email,
            string Password,
            List<int> SubjectIds
        )
        {
            var account = new AccountModel { Email = Email, Password = Password };
            var createdAccount = await _accountRepo.CreateAsync(account);

            var teacher = new TeacherModel
            {
                FullName = FullName,
                Email = Email,
                AccountId = createdAccount.Id,
            };
            var createdTeacher = await _teacherRepo.CreateAsync(teacher);

            foreach (var subjectId in SubjectIds)
            {
                await _teacherSubjectRepo.CreateAsync(
                    new TeacherSubjectModel { TeacherId = createdTeacher.Id, SubjectId = subjectId }
                );
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EditTeacher(int id)
        {
            var teacher = await _teacherRepo.GetByIdAsync(id);
            if (teacher == null)
                return NotFound();

            var subjects = await _subjectRepo.GetAllAsync();
            var selectedSubjectIds = (await _teacherSubjectRepo.GetByTeacherIdAsync(id))
                .Select(ts => ts.SubjectId)
                .ToList();

            ViewBag.Subjects = subjects;
            ViewBag.SelectedSubjectIds = selectedSubjectIds;

            return View("~/Views/Admin/Teacher/EditTeacher.cshtml", teacher);
        }

        [HttpPost]
        public async Task<IActionResult> EditTeacher(
            int id,
            string FullName,
            string Email,
            List<int> SubjectIds
        )
        {
            var teacher = await _teacherRepo.GetByIdAsync(id);
            if (teacher == null)
                return NotFound();

            teacher.FullName = FullName;
            teacher.Email = Email;
            await _teacherRepo.UpdateAsync(teacher);

            var existingSubjects = await _teacherSubjectRepo.GetByTeacherIdAsync(id);
            foreach (var ts in existingSubjects)
            {
                await _teacherSubjectRepo.DeleteAsync(ts.Id);
            }

            foreach (var subjectId in SubjectIds ?? new List<int>())
            {
                await _teacherSubjectRepo.CreateAsync(
                    new TeacherSubjectModel { TeacherId = id, SubjectId = subjectId }
                );
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var teacher = await _teacherRepo.GetByIdAsync(id);
            return View("~/Views/Admin/Teacher/DeleteTeacher.cshtml", teacher);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTeacherConfirmed(int id)
        {
            await _teacherRepo.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
