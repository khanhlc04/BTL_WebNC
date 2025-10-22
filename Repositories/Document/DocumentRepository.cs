using BTL_WebNC.Data;
using BTL_WebNC.Models.Document;
using Microsoft.EntityFrameworkCore;

namespace BTL_WebNC.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly AppDbContext _context;

        public DocumentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DocumentModel>> GetAllAsync()
        {
            return await _context.Documents.Where(d => !d.Deleted).OrderBy(d => d.Title).ToListAsync();
        }

        public async Task<DocumentModel?> GetByIdAsync(int id)
        {
            return await _context.Documents.FirstOrDefaultAsync(d => d.Id == id && !d.Deleted);
        }

        public async Task<DocumentModel> CreateAsync(DocumentModel document)
        {
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task<DocumentModel> UpdateAsync(DocumentModel document)
        {
            _context.Entry(document).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var document = await GetByIdAsync(id);
            if (document == null)
                return false;

            document.Deleted = true;
            await UpdateAsync(document);
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Documents.AnyAsync(d => d.Id == id && !d.Deleted);
        }

        public async Task<IEnumerable<DocumentModel>> SearchAsync(string searchTerm)
        {
            return await _context
                .Documents.Where(d => d.Title.Contains(searchTerm) && !d.Deleted)
                .OrderBy(d => d.Title)
                .ToListAsync();
        }
    }
}
