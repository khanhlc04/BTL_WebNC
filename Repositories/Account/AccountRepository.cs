using BTL_WebNC.Data;
using BTL_WebNC.Models.Account;
using Microsoft.EntityFrameworkCore;

namespace BTL_WebNC.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext _context;

        public AccountRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AccountModel>> GetAllAsync()
        {
            return await _context
                .Accounts.Where(a => !a.Deleted)
                .OrderByDescending(a => a.Id)
                .ToListAsync();
        }

        public async Task<AccountModel> GetByIdAsync(int id)
        {
            return await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id && !a.Deleted);
        }

        public async Task<AccountModel> GetByEmailAsync(string email)
        {
            return await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email && !a.Deleted);
        }

        public async Task<AccountModel> CreateAsync(AccountModel account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<AccountModel> UpdateAsync(AccountModel account)
        {
            _context.Entry(account).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var account = await GetByIdAsync(id);
            if (account == null)
                return false;

            account.Deleted = true;
            await UpdateAsync(account);
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Accounts.AnyAsync(a => a.Id == id && !a.Deleted);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Accounts.AnyAsync(a => a.Email == email && !a.Deleted);
        }
    }
}
