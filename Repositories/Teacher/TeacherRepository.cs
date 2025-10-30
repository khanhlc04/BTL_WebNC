using BTL_WebNC.Data;
using BTL_WebNC.Models.Subject;
using BTL_WebNC.Models.Teacher;
using BTL_WebNC.Models.TeacherSubject;
using Microsoft.EntityFrameworkCore;

namespace BTL_WebNC.Repositories
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
                .Include(t => t.TeacherSubjects.Where(ts => !ts.Deleted))
                .ThenInclude(ts => ts.Subject)
                .Where(t => !t.Deleted)
                .OrderByDescending(t => t.Id)
                .ToListAsync();
        }

        public async Task<TeacherModel> GetByIdAsync(int id)
        {
            return await _context
                .Teachers.Where(t => t.Id == id && !t.Deleted)
                .Select(t => new TeacherModel
                {
                    Id = t.Id,
                    FullName = t.FullName,
                    Email = t.Email,
                    Account = t.Account,
                    ThumbnailPath = t.ThumbnailPath,

                    TeacherSubjects = t
                        .TeacherSubjects.Where(ts => !ts.Deleted)
                        .Select(ts => new TeacherSubjectModel
                        {
                            Id = ts.Id,
                            SubjectId = ts.SubjectId,
                            Subject = new SubjectModel
                            {
                                Id = ts.Subject.Id,
                                Name = ts.Subject.Name,
                            },
                        })
                        .ToList(),
                })
                .FirstOrDefaultAsync();
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
            _context.Entry(teacher).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return teacher;
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
