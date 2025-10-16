using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BTL_WebNC.Models.Account;
using BTL_WebNC.Models.TeacherSubject;

namespace BTL_WebNC.Models.Teacher
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
