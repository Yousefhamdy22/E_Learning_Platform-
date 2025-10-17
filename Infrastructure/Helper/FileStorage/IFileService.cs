using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Helper.FileStorage
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file, string folder = "uploads");
        Task<byte[]> DownloadFileAsync(string filePath);
        Task<bool> DeleteFileAsync(string filePath);
        string GetFileUrl(string fileName, string folder = "uploads");
    }
}
