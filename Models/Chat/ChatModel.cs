using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BTL_WebNC.Models.Account;

namespace BTL_WebNC.Models.Chat
{
    [Table("Chat")]
    public class ChatModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(1000)]
        public string Message { get; set; }

        [Required]
        public int SenderId { get; set; }

        [Required]
        public int ReceiverId { get; set; }

        [ForeignKey("SenderId")]
        public virtual AccountModel Sender { get; set; }

        [ForeignKey("ReceiverId")]
        public virtual AccountModel Receiver { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool Deleted { get; set; } = false;
    }
}
