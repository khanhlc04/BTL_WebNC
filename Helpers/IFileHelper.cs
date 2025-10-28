using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BTL_WebNC.Helpers
{
    public interface IFileHelper
    {
        Task<string> SaveFileAsync(IFormFile file, string folder);
        void DeleteFile(string relativePath);
        string GetContentType(string path);
        string GetAbsolutePath(string relativePath);
    }
}
