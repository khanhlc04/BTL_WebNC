using BTLChatDemo.Models.Question;
using BTLChatDemo.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BTL_WebNC.Controllers.Admin
{
    [Route("Admin/[controller]")]
    public class QuestionController : Controller
    {
        private readonly IQuestionRepository _questionRepo;

        public QuestionController(IQuestionRepository questionRepo)
        {
            _questionRepo = questionRepo;
        }

        public async Task<IActionResult> Index()
        {
            var questions = await _questionRepo.GetAllAsync();
            return View("~/Views/Admin/Question/Index.cshtml", questions);
        }

        public IActionResult CreateQuestion()
        {
            return View("~/Views/Admin/Question/CreateQuestion.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuestion(string Content, int UserId)
        {
            var question = new QuestionModel { Content = Content, AccountId = UserId };
            await _questionRepo.CreateAsync(question);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> EditQuestion(int id)
        {
            var question = await _questionRepo.GetByIdAsync(id);
            return View("~/Views/Admin/Question/EditQuestion.cshtml", question);
        }

        [HttpPost]
        public async Task<IActionResult> EditQuestion(QuestionModel model)
        {
            await _questionRepo.UpdateAsync(model);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _questionRepo.GetByIdAsync(id);
            return View("~/Views/Admin/Question/DeleteQuestion.cshtml", question);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteQuestionConfirmed(int id)
        {
            await _questionRepo.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
