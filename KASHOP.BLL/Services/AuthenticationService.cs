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
using Azure.Core;
using KASHOP.BLL.Common;
using KASHOP.DAL.Dto;
using KASHOP.DAL.Models;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            var user = request.Adapt<ApplicationUser>();
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors
                    .Select(e => e.Description)
                    .ToList();

                return Result<RegisterResponse>.Fail("Failed to register", errors);
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
            return Result<RegisterResponse>.Ok(new RegisterResponse
            {
                Message = "Success"
            },
                "Registration successful");
        }

        public async Task <Result<bool>> ConfirmEmail(ConfirmEmailRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user is null)
            {
                return Result<bool>.Fail("User not found");
            }
            request.Token = Uri.UnescapeDataString(request.Token);
            var result = await _userManager.ConfirmEmailAsync(user, request.Token);
            if (!result.Succeeded)
            {
                return Result<bool>.Fail("Failed to confirm email",
                                        result.Errors.Select(e => e.Description).ToList());
            }
            return Result<bool>.Ok(true, "Email confirmed successfully");
        }
        public async Task <Result<LoginResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                return Result<LoginResponse>.Fail("Invalid email");
            }
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return Result<LoginResponse>.Fail("Email is not confirmed");
            }
            var result = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!result)
            {
                return Result<LoginResponse>.Fail("Invalid password");
            }
            return Result<LoginResponse>.Ok(
                new LoginResponse
                {
                    Message = "Login successful",
                    AccessToken = await GenerateJwt(user)
                },
                "Login successful"
            );
        }
        private async Task<string> GenerateJwt(ApplicationUser user)
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
