using BTLChatDemo.Data;
using BTLChatDemo.Models.Answer;
using Microsoft.EntityFrameworkCore;

namespace BTLChatDemo.Repositories
{
    public class AnswerRepository : IAnswerRepository
    {
        private readonly AppDbContext _context;

        public AnswerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AnswerModel>> GetAllAsync()
        {
            return await _context
                .Answers.Include(a => a.Account)
                .Include(a => a.Question)
                .Where(a => !a.Deleted)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<AnswerModel> GetByIdAsync(int id)
        {
            return await _context
                .Answers.Include(a => a.Account)
                .Include(a => a.Question)
                .FirstOrDefaultAsync(a => a.Id == id && !a.Deleted);
        }

        public async Task<IEnumerable<AnswerModel>> GetByQuestionIdAsync(int questionId)
        {
            return await _context
                .Answers.Include(a => a.Account)
                .Include(a => a.Question)
                .Where(a => a.QuestionId == questionId && !a.Deleted)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<AnswerModel>> GetByAccountIdAsync(int accountId)
        {
            return await _context
                .Answers.Include(a => a.Account)
                .Include(a => a.Question)
                .Where(a => a.AccountId == accountId && !a.Deleted)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<AnswerModel> CreateAsync(AnswerModel answer)
        {
            answer.CreatedAt = DateTime.Now;
            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();
            return answer;
        }

        public async Task<AnswerModel> UpdateAsync(AnswerModel answer)
        {
            _context.Entry(answer).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return answer;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var answer = await GetByIdAsync(id);
            if (answer == null)
                return false;

            answer.Deleted = true;
            await UpdateAsync(answer);
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Answers.AnyAsync(a => a.Id == id && !a.Deleted);
        }
    }
}
