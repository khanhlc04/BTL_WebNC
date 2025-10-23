using BTL_WebNC.Data;
using BTL_WebNC.Models.Chat;
using Microsoft.EntityFrameworkCore;

namespace BTL_WebNC.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly AppDbContext _context;

        public ChatRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ChatModel>> GetAllAsync()
        {
            return await _context
                .Chats.Include(c => c.Sender)
                .Include(c => c.Receiver)
                .Where(c => !c.Deleted)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<ChatModel> GetByIdAsync(int id)
        {
            return await _context
                .Chats.Include(c => c.Sender)
                .Include(c => c.Receiver)
                .FirstOrDefaultAsync(c => c.Id == id && !c.Deleted);
        }

        public async Task<IEnumerable<ChatModel>> GetConversationAsync(int senderId, int receiverId)
        {
            return await _context
                .Chats.Include(c => c.Sender)
                .Include(c => c.Receiver)
                .Where(c =>
                    (
                        (c.SenderId == senderId && c.ReceiverId == receiverId)
                        || (c.SenderId == receiverId && c.ReceiverId == senderId)
                    ) && !c.Deleted
                )
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ChatModel>> GetBySenderIdAsync(int senderId)
        {
            return await _context
                .Chats.Include(c => c.Sender)
                .Include(c => c.Receiver)
                .Where(c => c.SenderId == senderId && !c.Deleted)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ChatModel>> GetByReceiverIdAsync(int receiverId)
        {
            return await _context
                .Chats.Include(c => c.Sender)
                .Include(c => c.Receiver)
                .Where(c => c.ReceiverId == receiverId && !c.Deleted)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<ChatModel> CreateAsync(ChatModel chat)
        {
            chat.CreatedAt = DateTime.Now;
            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();
            return chat;
        }

        public async Task<ChatModel> UpdateAsync(ChatModel chat)
        {
            _context.Entry(chat).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return chat;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var chat = await GetByIdAsync(id);
            if (chat == null)
                return false;

            chat.Deleted = true;
            await UpdateAsync(chat);
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Chats.AnyAsync(c => c.Id == id && !c.Deleted);
        }
    }
}
