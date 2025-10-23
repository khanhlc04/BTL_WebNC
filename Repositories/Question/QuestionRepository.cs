using BTL_WebNC.Data;
using BTL_WebNC.Models.Question;
using Microsoft.EntityFrameworkCore;

namespace BTL_WebNC.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly AppDbContext _context;

        public QuestionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<QuestionModel>> GetAllAsync()
        {
            return await _context
                .Questions.Include(q => q.Account)
                .Where(q => !q.Deleted)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<QuestionModel> GetByIdAsync(int id)
        {
            return await _context
                .Questions.Include(q => q.Account)
                .FirstOrDefaultAsync(q => q.Id == id && !q.Deleted);
        }

        public async Task<IEnumerable<QuestionModel>> GetByAccountIdAsync(int accountId)
        {
            return await _context
                .Questions.Include(q => q.Account)
                .Where(q => q.AccountId == accountId && !q.Deleted)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<QuestionModel> CreateAsync(QuestionModel question)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a =>
                a.Id == question.AccountId && !a.Deleted
            );

            question.CreatedAt = DateTime.Now;
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task<QuestionModel> UpdateAsync(QuestionModel question)
        {
            _context.Entry(question).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var question = await GetByIdAsync(id);
            if (question == null)
                return false;

            question.Deleted = true;
            await UpdateAsync(question);
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Questions.AnyAsync(q => q.Id == id && !q.Deleted);
        }

        public async Task<IEnumerable<QuestionModel>> SearchAsync(string searchTerm)
        {
            return await _context
                .Questions.Include(q => q.Account)
                .Where(q => q.Content.Contains(searchTerm) && !q.Deleted)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }
    }
}
