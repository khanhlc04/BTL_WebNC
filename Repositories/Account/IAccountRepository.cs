using BTL_WebNC.Models.Account;

namespace BTL_WebNC.Repositories
{
    public interface IAccountRepository
    {
        Task<IEnumerable<AccountModel>> GetAllAsync();
        Task<AccountModel> GetByIdAsync(int id);
        Task<AccountModel> GetByEmailAsync(string email);
        Task<AccountModel> CreateAsync(AccountModel account);
        Task<AccountModel> UpdateAsync(AccountModel account);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> EmailExistsAsync(string email);
    }
}
