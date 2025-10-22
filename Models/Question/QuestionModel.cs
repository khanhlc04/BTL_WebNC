using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BTL_WebNC.Models.Account;

namespace BTL_WebNC.Models.Question
{
    [Table("Question")]
    public class QuestionModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(2000)]
        public string Content { get; set; }

        [Required]
        public int AccountId { get; set; }

        [ForeignKey("AccountId")]
        public virtual AccountModel Account { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool Deleted { get; set; } = false;
    }
}
