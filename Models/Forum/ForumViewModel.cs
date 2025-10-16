namespace BTL_WebNC.Models.Forum
{
    public class ForumViewModel
    {
        public string? SearchQuery { get; set; }
        public IEnumerable<QuestionSummaryViewModel> Questions { get; set; } = Enumerable.Empty<QuestionSummaryViewModel>();
        public bool ShowAskButton { get; set; } // Cờ ẩn/hiện nút "Đặt câu hỏi"
    }
}
