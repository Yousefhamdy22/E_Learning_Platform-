using Google.Apis.Drive.v3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.GoogleDeriveServices
{
    public class GoogleDriveService : IGoogleDriveService
    {
        private readonly DriveService _driveService;

        public GoogleDriveService(DriveService driveService)
        {
            _driveService = driveService;
        }

        public async Task<string> UploadRecordingAsync(string downloadUrl, string fileName)
        {
            // Download file from Zoom
            using var http = new HttpClient();
            var fileBytes = await http.GetByteArrayAsync(downloadUrl);

            // Create Google Drive file metadata
            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = fileName,
                Parents = new List<string> { "YOUR_FOLDER_ID" }
            };

            // Upload to Drive
            using var stream = new MemoryStream(fileBytes);
            var request = _driveService.Files.Create(fileMetadata, stream, "video/mp4");
            request.Fields = "id, webViewLink";
            await request.UploadAsync();

            var uploadedFile = request.ResponseBody;
            return uploadedFile.WebViewLink;
        }
    }
}
