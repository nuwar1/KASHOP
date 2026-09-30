using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Numerics;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KASHOP.BLL.Common;
using KASHOP.DAL.Dto;
using KASHOP.DAL.Models;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace KASHOP.BLL.Services
{
    public class AuthenticationService: IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;

        public AuthenticationService(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IConfiguration config)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _config = config;
        }
        public async Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var user = request.Adapt<ApplicationUser>();
                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    return new Result<RegisterResponse>()
                    {
                        Success = false,
                        Message = "Failed to register",
                        Data = new RegisterResponse() {
                            Message = "Error",
                            Errors = result.Errors.Select(e => e.Description).ToList()
                        }
                    };
                }
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                token = Uri.EscapeDataString(token);
                var emailUrl = $"https://localhost:7249/api/account/ConfirmEmail?token={token}&userId={user.Id}";
                await _emailSender.SendEmailAsync(request.Email, "Confirm Email",
                       $"""
                    <div>
                        <h1>Confirm Your Email</h1>
                        <p>Please click the verification link below to activate your account.</p>
                        <a href="{emailUrl}">Confirm Email</a>
                    </div>
                   """);
                return new Result<RegisterResponse>()
                {
                    Success = true,
                    Message = "Registration successful",
                    Data = new RegisterResponse()
                    {
                        Message = "Success"
                    }
                };
            }
            catch(Exception ex) 
            {
                return new Result<RegisterResponse>
                {
                    Success = false,
                    Message = ex.InnerException.Message,
                    Data = null
                };
            }
        }

        public async Task <Result<bool>> ConfirmEmail(ConfirmEmailRequest request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user is null)
                {
                    return new Result<bool>
                    {
                        Success = false,
                        Message = "User not founf",
                        Data = false
                    };
                }
                request.Token = Uri.UnescapeDataString(request.Token);
                var result = await _userManager.ConfirmEmailAsync(user, request.Token);
                if (!result.Succeeded)
                {
                    return new Result<bool>
                    {
                        Success = false,
                        Message = "Failed to confirm email",
                        Data = false
                    };
                }
                return new Result<bool>
                {
                    Success = true,
                    Message = "Email confirmed successfuly",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new Result<bool>
                {
                    Success = false,
                    Message = ex.InnerException.Message,
                    Data = false
                };
            }
        }
        public async Task <Result<LoginResponse>> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user is null)
                {
                    return new Result<LoginResponse>()
                    {
                        Success = false,
                        Message = "Invalid Email",
                    };
                }
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    return new Result<LoginResponse>()
                    {
                        Success = false,
                        Message = "Email is not confirmed"
                    };
                }
                var result = await _userManager.CheckPasswordAsync(user, request.Password);
                if (!result)
                {
                    return new Result<LoginResponse>()
                    {
                        Success = false,
                        Message = "Invalid Password"
                    };
                }
                return new Result<LoginResponse>()
                {
                    Success = true,
                    Message = "Success",
                    Data = new LoginResponse()
                    {
                        Message = "Login successful",
                        AccessToken = await GenerateJwt(user)
                    }
                };
            }
            catch (Exception ex) {
                return new Result<LoginResponse>()
                {
                    Success = false,
                    Message = ex.InnerException.Message
                };
            }
        }

        private async Task<String> GenerateJwt(ApplicationUser user)
        {
            var roles = _userManager.GetRolesAsync(user);
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, string.Join(",",roles))
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Apisettings:SecretKey"]));
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _config["ApiSettings:Issuer"],
                audience: _config["ApiSettings:Audience"],
                claims: userClaims,
                expires: DateTime.UtcNow.AddDays(20),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
