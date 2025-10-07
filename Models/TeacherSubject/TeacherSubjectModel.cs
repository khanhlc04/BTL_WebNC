using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BTLChatDemo.Models.Account;
using BTLChatDemo.Models.Subject;
using BTLChatDemo.Models.Teacher;

namespace BTLChatDemo.Models.TeacherSubject
{
    [Table("TeacherSubject")]
    public class TeacherSubjectModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int TeacherId { get; set; }

        [ForeignKey("TeacherId")]
        public virtual TeacherModel Teacher { get; set; }

        [Required]
        public int SubjectId { get; set; }

        [ForeignKey("SubjectId")]
        public virtual SubjectModel Subject { get; set; }

        public bool Deleted { get; set; } = false;
    }
}
