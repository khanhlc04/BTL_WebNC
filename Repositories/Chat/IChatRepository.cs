using BTL_WebNC.Models.Chat;

namespace BTL_WebNC.Repositories
{
    public interface IChatRepository
    {
        Task<IEnumerable<ChatModel>> GetAllAsync();
        Task<ChatModel> GetByIdAsync(int id);
        Task<IEnumerable<ChatModel>> GetConversationAsync(int senderId, int receiverId);
        Task<IEnumerable<ChatModel>> GetBySenderIdAsync(int senderId);
        Task<IEnumerable<ChatModel>> GetByReceiverIdAsync(int receiverId);
        Task<ChatModel> CreateAsync(ChatModel chat);
        Task<ChatModel> UpdateAsync(ChatModel chat);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
