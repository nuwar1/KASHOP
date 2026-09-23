using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace KASHOP.BLL.Services
{
    public interface IFileService
    {
        Task<string?> UploadAsync(IFormFile file);
    }
}
