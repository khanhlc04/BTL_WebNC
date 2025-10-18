using BTL_WebNC.Models.Account;
using BTL_WebNC.Models.Answer;
using BTL_WebNC.Models.Chat;
using BTL_WebNC.Models.Document;
using BTL_WebNC.Models.Forum;
using BTL_WebNC.Models.Question;
using BTL_WebNC.Models.RoomChat;
using BTL_WebNC.Models.Student;
using BTL_WebNC.Models.Subject;
using BTL_WebNC.Models.Teacher;
using BTL_WebNC.Models.TeacherSubject;
using Microsoft.EntityFrameworkCore;

namespace BTL_WebNC.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // DbSets
        public DbSet<AccountModel> Accounts { get; set; }
        public DbSet<StudentModel> Students { get; set; }
        public DbSet<TeacherModel> Teachers { get; set; }
        public DbSet<QuestionModel> Questions { get; set; }
        public DbSet<AnswerModel> Answers { get; set; }
        public DbSet<ChatModel> Chats { get; set; }
        public DbSet<RoomChatModel> RoomChats { get; set; }
        public DbSet<DocumentModel> Documents { get; set; }
        public DbSet<SubjectModel> Subjects { get; set; }
        public DbSet<TeacherSubjectModel> TeacherSubjects { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships for Student
            modelBuilder
                .Entity<StudentModel>()
                .HasOne(s => s.Account)
                .WithMany()
                .HasForeignKey(s => s.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationships for Teacher
            modelBuilder
                .Entity<TeacherModel>()
                .HasOne(t => t.Account)
                .WithMany()
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationships for Question
            modelBuilder
                .Entity<QuestionModel>()
                .HasOne(q => q.Account)
                .WithMany()
                .HasForeignKey(q => q.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<QuestionModel>()
                .HasMany(q => q.Answers)
                .WithOne(a => a.Question)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationships for Answer
            modelBuilder
                .Entity<AnswerModel>()
                .HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<AnswerModel>()
                .HasOne(a => a.Account)
                .WithMany()
                .HasForeignKey(a => a.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure relationships for Chat
            modelBuilder
                .Entity<ChatModel>()
                .HasOne(c => c.Sender)
                .WithMany()
                .HasForeignKey(c => c.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<ChatModel>()
                .HasOne(c => c.Receiver)
                .WithMany()
                .HasForeignKey(c => c.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure relationships for RoomChat
            modelBuilder
                .Entity<RoomChatModel>()
                .HasOne(r => r.Student)
                .WithMany()
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<RoomChatModel>()
                .HasOne(r => r.Teacher)
                .WithMany()
                .HasForeignKey(r => r.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure TeacherSubject
            modelBuilder.Entity<TeacherSubjectModel>().HasKey(ts => ts.Id);

            modelBuilder
                .Entity<TeacherSubjectModel>()
                .HasOne(ts => ts.Teacher)
                .WithMany(t => t.TeacherSubjects)
                .HasForeignKey(ts => ts.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<TeacherSubjectModel>()
                .HasOne(ts => ts.Subject)
                .WithMany()
                .HasForeignKey(ts => ts.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure unique constraints
            modelBuilder.Entity<AccountModel>().HasIndex(a => a.Email).IsUnique();

            modelBuilder.Entity<StudentModel>().HasIndex(s => s.StudentCode).IsUnique();

            modelBuilder.Entity<StudentModel>().HasIndex(s => s.AccountId).IsUnique();

            modelBuilder.Entity<TeacherModel>().HasIndex(t => t.AccountId).IsUnique();

            // Configure composite unique key for RoomChat
            modelBuilder
                .Entity<RoomChatModel>()
                .HasIndex(r => new { r.StudentId, r.TeacherId })
                .IsUnique();

            modelBuilder.Entity<SubjectModel>().HasKey(s => s.Id);
        }
    }
}
