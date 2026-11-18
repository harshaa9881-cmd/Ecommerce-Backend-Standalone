
using Ecommerce.DTOs.Auth;

namespace Ecommerce.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterDto dto);
        Task<LoginResponseDto?> LoginAsync(LoginDto dto);
    }
}
