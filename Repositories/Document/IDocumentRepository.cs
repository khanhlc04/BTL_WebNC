using BTL_WebNC.Models.Document;
using BTL_WebNC.Models.Subject;

namespace BTL_WebNC.Repositories
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
