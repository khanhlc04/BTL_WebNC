using BTLChatDemo.Models.Student;

namespace BTLChatDemo.Repositories
{
    public interface IStudentRepository
    {
        Task<IEnumerable<StudentModel>> GetAllAsync();
        Task<StudentModel> GetByIdAsync(int id);
        Task<StudentModel> GetByAccountIdAsync(int accountId);
        Task<StudentModel> GetByStudentCodeAsync(string studentCode);
        Task<StudentModel> CreateAsync(StudentModel student);
        Task<StudentModel> UpdateAsync(StudentModel student);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> StudentCodeExistsAsync(string studentCode);
    }
}
