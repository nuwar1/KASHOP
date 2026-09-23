using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace KASHOP.BLL.Services
{
    public class FileService: IFileService
    {
        public async Task<string?> UploadAsync(IFormFile file)
        {
            if (file is not  null && file.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(),"Uploads", fileName);
                using (var stream = System.IO.File.Create(filePath)) {
                    await file.CopyToAsync(stream);
                }
                return fileName;
            }
            return null;
        }

    }
}
