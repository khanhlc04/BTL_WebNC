using BTL_WebNC.Data;
using BTL_WebNC.Models.Student;
using Microsoft.EntityFrameworkCore;

namespace BTL_WebNC.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;

        public StudentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StudentModel>> GetAllAsync()
        {
            return await _context
                .Students.Include(s => s.Account)
                .Where(s => !s.Deleted)
                .OrderByDescending(s => s.Id)
                .ToListAsync();
        }

        public async Task<StudentModel> GetByIdAsync(int id)
        {
            return await _context
                .Students.Include(s => s.Account)
                .FirstOrDefaultAsync(s => s.Id == id && !s.Deleted);
        }

        public async Task<StudentModel> GetByAccountIdAsync(int accountId)
        {
            return await _context
                .Students.Include(s => s.Account)
                .FirstOrDefaultAsync(s => s.AccountId == accountId && !s.Deleted);
        }

        public async Task<StudentModel> GetByStudentCodeAsync(string studentCode)
        {
            return await _context
                .Students.Include(s => s.Account)
                .FirstOrDefaultAsync(s => s.StudentCode == studentCode && !s.Deleted);
        }

        public async Task<StudentModel> CreateAsync(StudentModel student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<StudentModel> UpdateAsync(StudentModel student)
        {
            var existing = await _context.Students.FindAsync(student.Id);
            if (existing == null)
                return null;

            existing.FullName = student.FullName;
            existing.Email = student.Email;
            existing.StudentCode = student.StudentCode;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var student = await GetByIdAsync(id);
            if (student == null)
                return false;

            student.Deleted = true;
            await UpdateAsync(student);
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Students.AnyAsync(s => s.Id == id && !s.Deleted);
        }

        public async Task<bool> StudentCodeExistsAsync(string studentCode)
        {
            return await _context.Students.AnyAsync(s =>
                s.StudentCode == studentCode && !s.Deleted
            );
        }
    }
}
