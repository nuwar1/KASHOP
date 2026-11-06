using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using KASHOP.DAL.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace KASHOP.BLL.Services
{
    public class FileService: IFileService
    {
        private readonly string[] _allowedExtensions = { ".png", ".jpg", ".jpeg", ".webp", ".svg" };

        const long _maxFileSize = 5 * 1024 * 1024;

        private readonly Cloudinary _cloudinary;
        
        public FileService(IConfiguration configuration)
        {
            var account = new Account(configuration["CloudinarySettings:CloudName"],
                                        configuration["CloudinarySettings:APIKey"],
                                        configuration["CloudinarySettings:APISecret"]
                );

            _cloudinary = new Cloudinary(account);
        }
        public async Task<Result<string>> UploadAsync(IFormFile file)
        {
            if (file is null || file.Length == 0)
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

            using(var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "KA14"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if(uploadResult.Error != null)
                {
                    return Result<string>.Fail(uploadResult.Error.Message);
                }

                return Result<string>.OK(uploadResult.SecureUrl.ToString(),
                "File uploaded successfully");
            }
        }
    }
}
