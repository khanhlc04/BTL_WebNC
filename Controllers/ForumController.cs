using BTL_WebNC.Models.Answer;
using BTL_WebNC.Models.Question;
using BTL_WebNC.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BTL_WebNC.Controllers
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

        [HttpGet]
        public async Task<IActionResult> SearchQuestions(string query)
        {
            var searchResults = await _questionRepo.SearchAsync(query);
            var list = searchResults
                .Select(q => new
                {
                    questionId = q.Id,
                    content = q.Content,
                    authorName = q.Account?.Email,
                    createdAt = q.CreatedAt,
                    replyCount = q.Answers?.Count(a => !a.Deleted) ?? 0,
                    lastReplyAuthor = q
                        .Answers?.Where(a => !a.Deleted)
                        .OrderByDescending(a => a.CreatedAt)
                        .FirstOrDefault()
                        ?.Account?.Email,
                    lastReplyDate = q
                        .Answers?.Where(a => !a.Deleted)
                        .OrderByDescending(a => a.CreatedAt)
                        .FirstOrDefault()
                        ?.CreatedAt,
                })
                .ToList();

            // Return the array directly (frontend expects an array)
            return Json(list);
        }

        public async Task<IActionResult> Details(int id)
        {
            var question = await _questionRepo.GetByIdAsync(id);
            if (question == null)
                return NotFound();

            return View(question);
        }

        [HttpPost]
        public async Task<IActionResult> AddReplyAjax(int questionId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return BadRequest("Nội dung không được để trống");

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Bạn cần đăng nhập để trả lời");

            if (!int.TryParse(userIdClaim.Value, out int userId))
                return BadRequest("ID người dùng không hợp lệ");

            var question = await _questionRepo.GetByIdAsync(questionId);
            if (question == null)
                return NotFound("Không tìm thấy câu hỏi");

            var answer = new AnswerModel
            {
                QuestionId = questionId,
                AccountId = userId,
                Content = content,
                CreatedAt = DateTime.Now,
            };

            var createdAnswer = await _answerRepo.CreateAsync(answer);
            if (createdAnswer == null)
                return StatusCode(500, "Không thể tạo câu trả lời");

            // Get full answer details
            var fullAnswer = await _answerRepo.GetByIdAsync(createdAnswer.Id);
            return PartialView("_AnswerPartial", fullAnswer);
        }

        [HttpGet]
        public async Task<IActionResult> GetSortedAnswers(int questionId, string sortOrder)
        {
            var answers = await _answerRepo.GetByQuestionIdAsync(questionId);
            if (answers == null)
                return NotFound();

            var orderedAnswers =
                sortOrder?.ToLower() == "asc"
                    ? answers.OrderBy(a => a.CreatedAt)
                    : answers.OrderByDescending(a => a.CreatedAt);

            return PartialView("_AnswersList", orderedAnswers);
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

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);

            var question = new QuestionModel
            {
                Content = Content,
                AccountId = userId,
                CreatedAt = DateTime.Now,
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

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                TempData["Error"] = "Bạn cần đăng nhập để trả lời!";
                return RedirectToAction("Details", new { id = QuestionId });
            }

            var userId = int.Parse(userIdClaim.Value);

            var answer = new AnswerModel
            {
                QuestionId = QuestionId,
                Content = Content,
                AccountId = userId,
                CreatedAt = DateTime.Now,
            };

            await _answerRepo.CreateAsync(answer);

            return RedirectToAction("Details", new { id = QuestionId });
        }
    }
}
