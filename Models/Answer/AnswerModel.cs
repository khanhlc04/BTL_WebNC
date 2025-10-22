using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BTL_WebNC.Models.Account;
using BTL_WebNC.Models.Question;

namespace BTL_WebNC.Models.Answer
{
    [Table("Answer")]
    public class AnswerModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(1000)]
        public string Content { get; set; }

        [Required]
        public int QuestionId { get; set; }

        [ForeignKey("QuestionId")]
        public virtual QuestionModel Question { get; set; }

        [Required]
        public int AccountId { get; set; }

        [ForeignKey("AccountId")]
        public virtual AccountModel Account { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool Deleted { get; set; } = false;
    }
}
