namespace BTL_WebNC.Models.Forum
{
    public class QuestionSummaryViewModel
    {
        public int Id { get; set; }
        public string? Title { get; set; } // Lấy từ Question.Content
        public string? AuthorFullName { get; set; } // Tên người đặt câu hỏi (lấy từ Teacher/Student qua Account)
        public DateTime CreatedAt { get; set; }
        public int ReplyCount { get; set; } // Số lượng câu trả lời
    }
}
