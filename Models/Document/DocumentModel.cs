using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BTLChatDemo.Models.Account;

namespace BTLChatDemo.Models.Document
{
    [Table("Document")]
    public class DocumentModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        [StringLength(500)]
        public string ThumbnailUrl { get; set; }

        [Required]
        public string FileUrl { get; set; }

        public bool Deleted { get; set; } = false;
    }
}
