using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BTLChatDemo.Models.Account;

namespace BTLChatDemo.Models.RoomChat
{
    [Table("RoomChat")]
    public class RoomChatModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual AccountModel Student { get; set; }

        [Required]
        public int TeacherId { get; set; }

        [ForeignKey("TeacherId")]
        public virtual AccountModel Teacher { get; set; }

        public bool Deleted { get; set; } = false;
    }
}
