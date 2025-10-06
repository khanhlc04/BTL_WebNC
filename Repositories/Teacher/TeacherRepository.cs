using BTLChatDemo.Data;
using BTLChatDemo.Models.Teacher;
using Microsoft.EntityFrameworkCore;

namespace BTLChatDemo.Repositories
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly AppDbContext _context;

        public TeacherRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TeacherModel>> GetAllAsync()
        {
            return await _context
                .Teachers.Include(t => t.Account)
                .Where(t => !t.Deleted)
                .ToListAsync();
        }

        public async Task<TeacherModel> GetByIdAsync(int id)
        {
            return await _context
                .Teachers.Include(t => t.Account)
                .FirstOrDefaultAsync(t => t.Id == id && !t.Deleted);
        }

        public async Task<TeacherModel> GetByAccountIdAsync(int accountId)
        {
            return await _context
                .Teachers.Include(t => t.Account)
                .FirstOrDefaultAsync(t => t.AccountId == accountId && !t.Deleted);
        }

        public async Task<TeacherModel> CreateAsync(TeacherModel teacher)
        {
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();
            return teacher;
        }

        public async Task<TeacherModel> UpdateAsync(TeacherModel teacher)
        {
            var existing = await _context.Teachers.FindAsync(teacher.Id);
            if (existing == null)
                return null;

            existing.FullName = teacher.FullName;
            existing.Email = teacher.Email;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var teacher = await GetByIdAsync(id);
            if (teacher == null)
                return false;

            teacher.Deleted = true;
            await UpdateAsync(teacher);
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Teachers.AnyAsync(t => t.Id == id && !t.Deleted);
        }
    }
}
