using BTL_WebNC.Models.Question;

namespace BTL_WebNC.Repositories
{
    public interface IQuestionRepository
    {
        Task<IEnumerable<QuestionModel>> GetAllAsync();
        Task<QuestionModel> GetByIdAsync(int id);
        Task<IEnumerable<QuestionModel>> GetByAccountIdAsync(int accountId);
        Task<QuestionModel> CreateAsync(QuestionModel question);
        Task<QuestionModel> UpdateAsync(QuestionModel question);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<QuestionModel>> SearchAsync(string searchTerm);
    }
}
