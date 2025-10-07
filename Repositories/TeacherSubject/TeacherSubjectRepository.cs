using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BTLChatDemo.Data;
using BTLChatDemo.Models.TeacherSubject;
using Microsoft.EntityFrameworkCore;

namespace BTLChatDemo.Repositories
{
    public class TeacherSubjectRepository : ITeacherSubjectRepository
    {
        private readonly AppDbContext _context;

        public TeacherSubjectRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TeacherSubjectModel>> GetAllAsync()
        {
            return await _context.TeacherSubjects.Where(ts => !ts.Deleted).ToListAsync();
        }

        public async Task<TeacherSubjectModel> GetByIdAsync(int id)
        {
            return await _context.TeacherSubjects.FirstOrDefaultAsync(ts =>
                ts.Id == id && !ts.Deleted
            );
        }

        public async Task<TeacherSubjectModel> CreateAsync(TeacherSubjectModel teacherSubject)
        {
            _context.TeacherSubjects.Add(teacherSubject);
            await _context.SaveChangesAsync();
            return teacherSubject;
        }

        public async Task<TeacherSubjectModel> UpdateAsync(TeacherSubjectModel teacherSubject)
        {
            _context.Entry(teacherSubject).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return teacherSubject;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var teacherSubject = await GetByIdAsync(id);
            if (teacherSubject == null)
                return false;
            teacherSubject.Deleted = true;
            await UpdateAsync(teacherSubject);
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.TeacherSubjects.AnyAsync(ts => ts.Id == id && !ts.Deleted);
        }

        public async Task<IEnumerable<TeacherSubjectModel>> GetByTeacherIdAsync(int teacherId)
        {
            return await _context
                .TeacherSubjects.Where(ts => ts.TeacherId == teacherId && !ts.Deleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<TeacherSubjectModel>> GetBySubjectIdAsync(int subjectId)
        {
            return await _context
                .TeacherSubjects.Where(ts => ts.SubjectId == subjectId && !ts.Deleted)
                .ToListAsync();
        }
    }
}
