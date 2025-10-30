using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BTL_WebNC.Data;
using BTL_WebNC.Models.Subject;
using Microsoft.EntityFrameworkCore;

namespace BTL_WebNC.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly AppDbContext _context;

        public SubjectRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SubjectModel>> GetAllAsync()
        {
            return await _context
                .Subjects.Where(s => !s.Deleted)
                .OrderByDescending(d => d.Id)
                .ToListAsync();
        }

        public async Task<SubjectModel> GetByIdAsync(int id)
        {
            return await _context.Subjects.FirstOrDefaultAsync(s => s.Id == id && !s.Deleted);
        }

        public async Task<SubjectModel> CreateAsync(SubjectModel subject)
        {
            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();
            return subject;
        }

        public async Task<SubjectModel> UpdateAsync(SubjectModel subject)
        {
            _context.Entry(subject).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return subject;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var subject = await GetByIdAsync(id);
            if (subject == null)
                return false;
            subject.Deleted = true;
            var result = await UpdateAsync(subject);
            Console.WriteLine("ketr" + result);
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Subjects.AnyAsync(s => s.Id == id && !s.Deleted);
        }
    }
}
