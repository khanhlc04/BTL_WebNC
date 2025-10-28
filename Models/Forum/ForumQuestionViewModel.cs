namespace BTL_WebNC.Models.Forum
{
    public class ForumQuestionViewModel
    {
        public int QuestionId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Reply info
        public int ReplyCount { get; set; }
        public string? LastReplyAuthor { get; set; }
        public DateTime? LastReplyDate { get; set; }
    }
}
