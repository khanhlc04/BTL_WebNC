using System.Collections.Generic;
using System.Threading.Tasks;
using BTL_WebNC.Models.Subject;

namespace BTL_WebNC.Repositories
{
    public interface ISubjectRepository
    {
        Task<IEnumerable<SubjectModel>> GetAllAsync();
        Task<SubjectModel> GetByIdAsync(int id);
        Task<SubjectModel> CreateAsync(SubjectModel subject);
        Task<SubjectModel> UpdateAsync(SubjectModel subject);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
