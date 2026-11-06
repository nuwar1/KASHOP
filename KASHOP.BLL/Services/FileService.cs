using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KASHOP.DAL.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace KASHOP.BLL.Services
{
    public class FileService: IFileService
    {
        private readonly string[] _allowedExtensions = { ".png", ".jpg", ".jpeg", ".webp", ".svg" };
        const long _maxFileSize = 5 * 1024 * 1024;
        public async Task <Result<string>> UploadAsync(IFormFile file)
        {
            try
            {
                if (file is null || file.Length < 0 )
                {
                    return Result<string>.Fail("No file was provided");
                }
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!_allowedExtensions.Contains(extension))
                {
                    return Result<string>.Fail($"File type {extension} is not allowed");
                }
                if (file.Length > _maxFileSize)
                {
                    return Result<string>.Fail("File size exceeds 5MB limit");
                }
                var fileName = Guid.NewGuid().ToString() + extension;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }
                return Result<string>.OK(fileName,"File uploaded successfully");
            }
            catch (Exception ex) {
                return Result<string>.Fail(ex.InnerException.Message);
            }
        }
    }
}
