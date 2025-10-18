using BTL_WebNC.Repositories;
using Microsoft.AspNetCore.Mvc;
using BTL_WebNC.Models.Question;
using BTL_WebNC.Models.Answer;
using Microsoft.EntityFrameworkCore;

namespace BTL_WebNC.Controllers.Forum
{
    public class ForumController : Controller
    {
        private readonly IQuestionRepository _questionRepo;

        public ForumController(IQuestionRepository questionRepo)
        {
            _questionRepo = questionRepo;
        }

        public async Task<IActionResult> Index()
        {
            var forumQuestions = await _questionRepo.GetForumQuestionsAsync();
            return View("~/Views/Forum/Index.cshtml", forumQuestions);
        }
        public async Task<IActionResult> Details(int id)
        {
            var question = await _questionRepo.GetByIdAsync(id);
            if (question == null) return NotFound();

            return View(question);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(QuestionModel model)
        {
            if (ModelState.IsValid)
            {
                // giả sử lấy AccountId = 1 (cần thay bằng user đang login)
                model.AccountId = 1;
                await _questionRepo.AddAsync(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // Thêm câu trả lời
        [HttpPost]
        public async Task<IActionResult> AddReply(int QuestionId, string Content)
        {
            if (string.IsNullOrWhiteSpace(Content))
            {
                return RedirectToAction("Details", new { id = QuestionId });
            }

            var ans = new AnswerModel
            {
                QuestionId = QuestionId,
                Content = Content,
                AccountId = 1 // TODO: lấy account từ login
            };

            await _questionRepo.AddAnswerAsync(ans);

            return RedirectToAction("Details", new { id = QuestionId });
        }
    }
}
