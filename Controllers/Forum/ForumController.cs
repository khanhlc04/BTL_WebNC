using BTL_WebNC.Repositories;
using Microsoft.AspNetCore.Mvc;
using BTL_WebNC.Models.Question;
using BTL_WebNC.Models.Answer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BTL_WebNC.Controllers.Forum
{
    public class ForumController : Controller
    {
        private readonly IQuestionRepository _questionRepo;
        private readonly IAnswerRepository _answerRepo;

        public ForumController(IQuestionRepository questionRepo, IAnswerRepository answerRepo)
        {
            _questionRepo = questionRepo;
            _answerRepo = answerRepo;
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
        public async Task<IActionResult> Create(string Content)
        {
            if (string.IsNullOrWhiteSpace(Content))
            {
                ModelState.AddModelError("", "Nội dung câu hỏi không được để trống");
                return View();
            }

            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

            var question = new QuestionModel
            {
                Content = Content,
                AccountId = userId,
                CreatedAt = DateTime.Now
            };

            await _questionRepo.CreateAsync(question);
            return RedirectToAction("Index");
        }

        // Thêm câu trả lời
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReply(int QuestionId, string Content)
        {
            if (string.IsNullOrWhiteSpace(Content))
            {
                TempData["Error"] = "Nội dung trả lời không được để trống!";
                return RedirectToAction("Details", new { id = QuestionId });
            }

            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

            var answer = new AnswerModel
            {
                QuestionId = QuestionId,
                Content = Content,
                AccountId = userId,
                CreatedAt = DateTime.Now
            };

            await _answerRepo.CreateAsync(answer);

            return RedirectToAction("Details", new { id = QuestionId });
        }
    }
}
