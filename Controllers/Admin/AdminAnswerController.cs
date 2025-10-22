using BTL_WebNC.Models.Answer;
using BTL_WebNC.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BTL_WebNC.Controllers.Admin
{
    public class AdminAnswerController : Controller
    {
        private readonly IAnswerRepository _answerRepo;

        public AdminAnswerController(IAnswerRepository answerRepo)
        {
            _answerRepo = answerRepo;
        }

        public async Task<IActionResult> Index()
        {
            var answers = await _answerRepo.GetAllAsync();
            return View("~/Views/Admin/Answer/Index.cshtml", answers);
        }

        public IActionResult CreateAnswer() => View("~/Views/Admin/Answer/CreateAnswer.cshtml");

        [HttpPost]
        public async Task<IActionResult> CreateAnswer(string Content, int UserId, int QuestionId)
        {
            var model = new AnswerModel
            {
                Content = Content,
                AccountId = UserId,
                QuestionId = QuestionId,
            };
            await _answerRepo.CreateAsync(model);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> EditAnswer(int id)
        {
            var answer = await _answerRepo.GetByIdAsync(id);
            return View("~/Views/Admin/Answer/EditAnswer.cshtml", answer);
        }

        [HttpPost]
        public async Task<IActionResult> EditAnswer(int Id, string Content, int QuestionId)
        {
            var model = await _answerRepo.GetByIdAsync(Id);
            if (model != null)
            {
                model.Content = Content;
                model.QuestionId = QuestionId;
                await _answerRepo.UpdateAsync(model);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteAnswer(int id)
        {
            var answer = await _answerRepo.GetByIdAsync(id);
            return View("~/Views/Admin/Answer/DeleteAnswer.cshtml", answer);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAnswerConfirmed(int id)
        {
            await _answerRepo.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
