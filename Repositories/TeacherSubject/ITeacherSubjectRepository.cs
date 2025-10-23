using System.Collections.Generic;
using System.Threading.Tasks;
using BTL_WebNC.Models.TeacherSubject;

namespace BTL_WebNC.Repositories
{
    public interface ITeacherSubjectRepository
    {
        Task<IEnumerable<TeacherSubjectModel>> GetAllAsync();
        Task<TeacherSubjectModel> GetByIdAsync(int id);
        Task<TeacherSubjectModel> CreateAsync(TeacherSubjectModel teacherSubject);
        Task<TeacherSubjectModel> UpdateAsync(TeacherSubjectModel teacherSubject);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<TeacherSubjectModel>> GetByTeacherIdAsync(int teacherId);
        Task<IEnumerable<TeacherSubjectModel>> GetBySubjectIdAsync(int subjectId);
    }
}
