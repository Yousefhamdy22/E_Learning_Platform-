using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.GoogleDeriveServices
{
    public interface IGoogleDriveService
    {
        Task<string> UploadRecordingAsync(string downloadUrl, string fileName);
    }

}
