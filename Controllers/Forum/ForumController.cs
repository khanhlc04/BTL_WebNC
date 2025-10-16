using Microsoft.AspNetCore.Mvc;
using BTL_WebNC.Repositories;
using BTL_WebNC.Models.Forum;
using BTL_WebNC.Models.Question;
using Microsoft.AspNetCore.Authorization;


namespace BTL_WebNC.Controllers.Forum
{
    public class ForumController : Controller
    {
        private readonly IQuestionRepository _questionRepo;
        //private readonly IUserService _userService; // Dịch vụ tra cứu vai trò và Id người dùng

        public ForumController(IQuestionRepository questionRepo)
        {
            _questionRepo = questionRepo;
        }

        /// <summary>
        /// Hiển thị danh sách các câu hỏi.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(string searchQuery)
        {
            // 1. Lấy danh sách câu hỏi theo tìm kiếm
            var summaries = await _questionRepo.SearchAsync(searchQuery);

            // 2. Xác định vai trò để ẩn/hiện nút "Đặt câu hỏi"
            //var currentUserId = _userService.GetCurrentAccountId();
            //var isTeacher = await _userService.IsTeacherAsync(currentUserId);

            //var viewModel = new ForumViewModel
            //{
            //    Questions = summaries,
            //    SearchQuery = searchQuery,
            //    // Yêu cầu: Nút chỉ hiện cho HỌC SINH (role khác Giáo viên)
            //    ShowAskButton = !isTeacher
            //};

            return View();
        }

        /// <summary>
        /// Hiển thị form để Học sinh đặt câu hỏi mới.
        /// </summary>
        //[HttpGet]
        //[Authorize] // Yêu cầu đăng nhập
        /*
        public async Task<IActionResult> Ask()
        {
            var currentUserId = _userService.GetCurrentAccountId();
            if (await _userService.IsTeacherAsync(currentUserId))
            {
                // Nếu là Giáo viên, chuyển hướng hoặc thông báo lỗi
                return RedirectToAction(nameof(Index));
            }

            return View(new AskQuestionViewModel());
        }

        /// <summary>
        /// Xử lý việc gửi câu hỏi mới.
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Ask(AskQuestionViewModel model)
        {
            var currentUserId = _userService.GetCurrentAccountId();
            if (await _userService.IsTeacherAsync(currentUserId))
            {
                // Ngăn Giáo viên tạo câu hỏi
                ModelState.AddModelError("", "Giáo viên không được phép đăng câu hỏi.");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                // Tạo QuestionModel và lưu vào CSDL
                var question = new QuestionModel
                {
                    Content = model.Content,
                    AccountId = currentUserId,
                    CreatedAt = DateTime.Now
                };
                await _questionRepo.CreateAsync(question);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }*/
    }
}
