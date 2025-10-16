using System.ComponentModel.DataAnnotations;

namespace BTL_WebNC.Models.Forum
{
    public class AskQuestionViewModel
    {
        [Required(ErrorMessage = "Nội dung câu hỏi không được để trống.")]
        [MinLength(10, ErrorMessage = "Câu hỏi phải có ít nhất 10 ký tự.")]
        public string Content { get; set; }
    }
}
