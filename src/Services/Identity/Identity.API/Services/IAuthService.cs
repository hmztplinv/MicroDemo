public interface IAuthService
{
    Task<TokenDto> LoginAsync(LoginDto loginDto);
    Task<bool> RegisterAsync(RegisterUserDto registerUser);
    Task<TokenDto> RefreshTokenAsync(string refreshToken);
}