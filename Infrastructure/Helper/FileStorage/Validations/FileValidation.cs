using Domain.Common.Results;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Helper.FileStorage.Validations
{
    public class FileValidation
    {
        private readonly IFileService _fileService;

        public FileValidation(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<Result<string>> UploadQuestionImageAsync(IFormFile imageFile, CancellationToken ct)
        {
            try
            {
                // Validate image
                if (imageFile.Length == 0)
                    return Result<string>.FromError(Error.Validation("Image.Empty", "Image file is empty"));

                if (imageFile.Length > 5 * 1024 * 1024) 
                    return Result<string>.FromError(Error.Validation("Image.TooLarge", "Image size exceeds 5MB"));

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                    return Result<string>.FromError(Error.Validation("Image.InvalidType", "Only JPG, PNG, and GIF images are allowed"));

                // Upload to questionsImages folder
                var fileName = await _fileService.UploadFileAsync(imageFile, "questionsImages");
                var imageUrl = _fileService.GetFileUrl(fileName, "questionsImages");

                return Result<string>.FromValue(imageUrl);
            }
            catch (Exception ex)
            {
               
                return Result<string>.FromError(Error.Failure("Image.UploadFailed", ex.Message));
            }
        }
    }
}
