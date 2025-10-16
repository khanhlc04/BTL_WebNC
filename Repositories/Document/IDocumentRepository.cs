using BTLChatDemo.Models.Document;
using BTLChatDemo.Models.Subject;

namespace BTLChatDemo.Repositories
{
    public interface IDocumentRepository
    {
        Task<IEnumerable<DocumentModel>> GetAllAsync();
        Task<DocumentModel?> GetByIdAsync(int id);
        Task<DocumentModel> CreateAsync(DocumentModel document);
        Task<DocumentModel> UpdateAsync(DocumentModel document);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<DocumentModel>> SearchAsync(string searchTerm);
        Task<IEnumerable<SubjectModel>> GetSubjectsAsync();
    }
}
