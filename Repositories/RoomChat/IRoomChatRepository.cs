using BTLChatDemo.Models.RoomChat;

namespace BTLChatDemo.Repositories
{
    public interface IRoomChatRepository
    {
        Task<IEnumerable<RoomChatModel>> GetAllAsync();
        Task<RoomChatModel> GetByIdAsync(int id);
        Task<RoomChatModel> GetByParticipantsAsync(int studentId, int teacherId);
        Task<IEnumerable<RoomChatModel>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<RoomChatModel>> GetByTeacherIdAsync(int teacherId);
        Task<RoomChatModel> CreateAsync(RoomChatModel roomChat);
        Task<RoomChatModel> UpdateAsync(RoomChatModel roomChat);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> RoomExistsAsync(int studentId, int teacherId);
    }
}
