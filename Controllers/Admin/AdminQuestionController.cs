using BTL_WebNC.Models.Question;
using BTL_WebNC.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BTL_WebNC.Controllers.Admin
{
    public class AdminQuestionController : Controller
    {
        private readonly IQuestionRepository _questionRepo;

        private readonly IAccountRepository _accountRepo;

        public AdminQuestionController(
            IQuestionRepository questionRepo,
            IAccountRepository accountRepo
        )
        {
            _questionRepo = questionRepo;
            _accountRepo = accountRepo;
        }

        public async Task<IActionResult> Index()
        {
            var accounts = await _accountRepo.GetAllAsync();

            ViewBag.Accounts = accounts;

            var questions = await _questionRepo.GetAllAsync();
            return View("~/Views/Admin/Question/Index.cshtml", questions);
        }

        [HttpGet]
        public async Task<IActionResult> GetQuestion(int id)
        {
            var question = await _questionRepo.GetByIdAsync(id);

            if (question == null)
                return NotFound(new { success = false, message = "Không tìm thấy câu hỏi" });
            return Ok(new { success = true, data = question });
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuestion(
            [FromForm] string content,
            [FromForm] int accountId
        )
        {
            try
            {
                var question = new QuestionModel { Content = content, AccountId = accountId };
                await _questionRepo.CreateAsync(question);
                return Ok(new { success = true, message = "Tạo câu hỏi thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuestion(
            [FromForm] int id,
            [FromForm] string content,
            [FromForm] int accountId
        )
        {
            try
            {
                var existingQuestion = await _questionRepo.GetByIdAsync(id);
                if (existingQuestion == null)
                {
                    return NotFound(new { success = false, message = "Câu hỏi không tồn tại" });
                }

                existingQuestion.Content = content;
                existingQuestion.AccountId = accountId;

                await _questionRepo.UpdateAsync(existingQuestion);
                return Ok(new { success = true, message = "Cập nhật câu hỏi thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteQuestion([FromBody] int id)
        {
            try
            {
                var existingQuestion = await _questionRepo.GetByIdAsync(id);
                if (existingQuestion == null)
                {
                    return NotFound(new { success = false, message = "Câu hỏi không tồn tại" });
                }

                Console.WriteLine("Deleting question with ID: " + id);
                Console.WriteLine("Question content: " + existingQuestion.Content);

                await _questionRepo.DeleteAsync(id);
                return Ok(new { success = true, message = "Xóa câu hỏi thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // public IActionResult CreateQuestion()
        // {
        //     return View("~/Views/Admin/Question/CreateQuestion.cshtml");
        // }

        // [HttpPost]
        // public async Task<IActionResult> CreateQuestion(string Content, int UserId)
        // {
        //     var question = new QuestionModel { Content = Content, AccountId = UserId };
        //     await _questionRepo.CreateAsync(question);
        //     return RedirectToAction("Index");
        // }

        // public async Task<IActionResult> EditQuestion(int id)
        // {
        //     var question = await _questionRepo.GetByIdAsync(id);
        //     return View("~/Views/Admin/Question/EditQuestion.cshtml", question);
        // }

        // [HttpPost]
        // public async Task<IActionResult> EditQuestion(QuestionModel model)
        // {
        //     await _questionRepo.UpdateAsync(model);
        //     return RedirectToAction("Index");
        // }

        // public async Task<IActionResult> DeleteQuestion(int id)
        // {
        //     var question = await _questionRepo.GetByIdAsync(id);
        //     return View("~/Views/Admin/Question/DeleteQuestion.cshtml", question);
        // }

        // [HttpPost]
        // public async Task<IActionResult> DeleteQuestionConfirmed(int id)
        // {
        //     await _questionRepo.DeleteAsync(id);
        //     return RedirectToAction("Index");
        // }
    }
}
