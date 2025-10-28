using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace BTL_WebNC.Helpers
{
    public class FileHelper : IFileHelper
    {
        private readonly IWebHostEnvironment _env;

        public FileHelper(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", folder);
            Directory.CreateDirectory(uploadsFolder);
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return Path.Combine("uploads", folder, fileName).Replace("\\", "/");
        }

        public void DeleteFile(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return;
            var fullPath = GetAbsolutePath(relativePath);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }

        public string GetContentType(string path)
        {
            var types = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { ".pdf", "application/pdf" },
                { ".doc", "application/vnd.ms-word" },
                {
                    ".docx",
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                },
                { ".xls", "application/vnd.ms-excel" },
                { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                { ".ppt", "application/vnd.ms-powerpoint" },
                {
                    ".pptx",
                    "application/vnd.openxmlformats-officedocument.presentationml.presentation"
                },
                { ".zip", "application/zip" },
                { ".txt", "text/plain" },
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".png", "image/png" },
                { ".gif", "image/gif" },
            };

            var ext = Path.GetExtension(path)?.ToLowerInvariant() ?? string.Empty;
            return types.ContainsKey(ext) ? types[ext] : "application/octet-stream";
        }

        public string GetAbsolutePath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return string.Empty;
            return Path.Combine(
                _env.WebRootPath,
                relativePath.Replace('/', Path.DirectorySeparatorChar)
            );
        }
    }
}
