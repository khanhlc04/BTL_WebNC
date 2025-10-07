using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BTLChatDemo.Models.Account;
using BTLChatDemo.Models.TeacherSubject;

namespace BTLChatDemo.Models.Teacher
{
    [Table("Teacher")]
    public class TeacherModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public int AccountId { get; set; }

        [ForeignKey("AccountId")]
        public virtual AccountModel Account { get; set; }

        public virtual ICollection<TeacherSubjectModel> TeacherSubjects { get; set; }

        public bool Deleted { get; set; } = false;
    }
}
