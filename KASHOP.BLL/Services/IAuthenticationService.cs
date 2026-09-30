using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KASHOP.DAL.Dto;

namespace KASHOP.BLL.Services
{
    public interface IAuthenticationService
    {
        Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request);
        Task<Result<LoginResponse>> LoginAsync(LoginRequest request);
        Task<Result<bool>> ConfirmEmail(ConfirmEmailRequest request);
    }
}
