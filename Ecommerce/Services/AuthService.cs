
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Ecommerce.DTOs.Auth;
using Ecommerce.Models;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Ecommerce.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthService(
            IAuthRepository authRepository,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IConfiguration configuration)
        {
            _authRepository = authRepository;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        public async Task<bool> RegisterAsync(RegisterDto dto)
        {
            string email = dto.Email.Trim().ToLowerInvariant();

            var existingUser = await _authRepository.GetByEmailAsync(email);

            if (existingUser != null)
            {
                return false;
            }

            var user = new ApplicationUser
            {
                FullName = dto.FullName.Trim(),
                Email = email,
                Role = "Customer",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(
                user,
                dto.Password
            );

            await _authRepository.AddAsync(user);
            await _authRepository.SaveChangesAsync();

            return true;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
        {
            string email = dto.Email.Trim().ToLowerInvariant();

            var user = await _authRepository.GetByEmailAsync(email);

            if (user == null || !user.IsActive)
            {
                return null;
            }

            var passwordResult = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                dto.Password
            );

            if (passwordResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            string token = GenerateJwtToken(user);

            return new LoginResponseDto
            {
                Message = "Login successful.",
                Token = token
            };
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            string? key = _configuration["Jwt:Key"];
            string? issuer = _configuration["Jwt:Issuer"];
            string? audience = _configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(key) ||
                string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(audience))
            {
                throw new InvalidOperationException(
                    "JWT configuration is missing."
                );
            }

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key)
            );

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256
            );

            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.UserId.ToString()
                ),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            int expiryMinutes = int.Parse(
                _configuration["Jwt:ExpiryMinutes"] ?? "60"
            );

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
