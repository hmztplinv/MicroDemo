using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            _logger.LogInformation("Login attempt for user: {Username}", loginDto.UserName);
            
            var result = await _authService.LoginAsync(loginDto);
            if (result == null)
            {
                return Unauthorized(new { Message = "Invalid username or password" });
            }

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            _logger.LogInformation("Registration attempt for user: {Username}", registerDto.UserName);
            
            var result = await _authService.RegisterAsync(registerDto);
            if (!result)
            {
                return BadRequest(new { Message = "User registration failed" });
            }

            return Ok(new { Message = "User registered successfully" });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            _logger.LogInformation("Token refresh attempt");
            
            var result = await _authService.RefreshTokenAsync(refreshToken);
            if (result == null)
            {
                return Unauthorized(new { Message = "Invalid refresh token" });
            }

            return Ok(result);
        }
    }
}