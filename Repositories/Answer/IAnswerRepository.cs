using BTL_WebNC.Models.Answer;

namespace BTL_WebNC.Repositories
{
    public interface IAnswerRepository
    {
        Task<IEnumerable<AnswerModel>> GetAllAsync();
        Task<AnswerModel> GetByIdAsync(int id);
        Task<IEnumerable<AnswerModel>> GetByQuestionIdAsync(int questionId);
        Task<IEnumerable<AnswerModel>> GetByAccountIdAsync(int accountId);
        Task<AnswerModel> CreateAsync(AnswerModel answer);
        Task<AnswerModel> UpdateAsync(AnswerModel answer);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
