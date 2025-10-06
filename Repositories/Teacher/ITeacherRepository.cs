using BTLChatDemo.Models.Teacher;

namespace BTLChatDemo.Repositories
{
    public interface ITeacherRepository
    {
        Task<IEnumerable<TeacherModel>> GetAllAsync();
        Task<TeacherModel> GetByIdAsync(int id);
        Task<TeacherModel> GetByAccountIdAsync(int accountId);
        Task<TeacherModel> CreateAsync(TeacherModel teacher);
        Task<TeacherModel> UpdateAsync(TeacherModel teacher);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
