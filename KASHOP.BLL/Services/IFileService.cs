using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KASHOP.DAL.Dto;
using Microsoft.AspNetCore.Http;

namespace KASHOP.BLL.Services
{
    public interface IFileService
    {
        Task <Result<string>> UploadAsync(IFormFile file);
    }
}
