using BTL_WebNC.Data;
using BTL_WebNC.Models.RoomChat;
using Microsoft.EntityFrameworkCore;

namespace BTL_WebNC.Repositories
{
    public class RoomChatRepository : IRoomChatRepository
    {
        private readonly AppDbContext _context;

        public RoomChatRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoomChatModel>> GetAllAsync()
        {
            return await _context
                .RoomChats.Include(r => r.Student)
                .Include(r => r.Teacher)
                .Where(r => !r.Deleted)
                .ToListAsync();
        }

        public async Task<RoomChatModel> GetByIdAsync(int id)
        {
            return await _context
                .RoomChats.Include(r => r.Student)
                .Include(r => r.Teacher)
                .FirstOrDefaultAsync(r => r.Id == id && !r.Deleted);
        }

        public async Task<RoomChatModel> GetByParticipantsAsync(int studentId, int teacherId)
        {
            return await _context
                .RoomChats.Include(r => r.Student)
                .Include(r => r.Teacher)
                .FirstOrDefaultAsync(r =>
                    r.StudentId == studentId && r.TeacherId == teacherId && !r.Deleted
                );
        }

        public async Task<IEnumerable<RoomChatModel>> GetByStudentIdAsync(int studentId)
        {
            return await _context
                .RoomChats.Include(r => r.Student)
                .Include(r => r.Teacher)
                .Where(r => r.StudentId == studentId && !r.Deleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<RoomChatModel>> GetByTeacherIdAsync(int teacherId)
        {
            return await _context
                .RoomChats.Include(r => r.Student)
                .Include(r => r.Teacher)
                .Where(r => r.TeacherId == teacherId && !r.Deleted)
                .ToListAsync();
        }

        public async Task<RoomChatModel> CreateAsync(RoomChatModel roomChat)
        {
            _context.RoomChats.Add(roomChat);
            await _context.SaveChangesAsync();
            return roomChat;
        }

        public async Task<RoomChatModel> UpdateAsync(RoomChatModel roomChat)
        {
            _context.Entry(roomChat).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return roomChat;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var roomChat = await GetByIdAsync(id);
            if (roomChat == null)
                return false;

            roomChat.Deleted = true;
            await UpdateAsync(roomChat);
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.RoomChats.AnyAsync(r => r.Id == id && !r.Deleted);
        }

        public async Task<bool> RoomExistsAsync(int studentId, int teacherId)
        {
            return await _context.RoomChats.AnyAsync(r =>
                r.StudentId == studentId && r.TeacherId == teacherId && !r.Deleted
            );
        }
    }
}
