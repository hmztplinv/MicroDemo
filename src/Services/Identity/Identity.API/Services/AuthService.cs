using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IUserEventProducer _userEventProducer;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IUserRepository userRepository,
            IConfiguration configuration,
            IUserEventProducer userEventProducer)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _configuration = configuration;
            _userEventProducer=userEventProducer;
        }

        public async Task<TokenDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return null;
            }

            return await GenerateTokenAsync(user);
        }

        public async Task<bool> RegisterAsync(RegisterUserDto registerDto)
        {
            var result= await _userRepository.RegisterUserAsync(registerDto);
            if(result)
            {
                var user=await _userManager.FindByNameAsync(registerDto.UserName);
                await _userEventProducer.PublishUserCreatedAsync(user);
            }
            return result;
        }

        public async Task<TokenDto> RefreshTokenAsync(string refreshToken)
        {
            // Implementation of refresh token logic
            // This is a simplified version, in production you should validate the refresh token
            var principal = GetPrincipalFromExpiredToken(refreshToken);
            var username = principal.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);
            
            if (user == null)
            {
                return null;
            }

            return await GenerateTokenAsync(user);
        }

        private async Task<TokenDto> GenerateTokenAsync(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenDto
            {
                AccessToken = tokenString,
                RefreshToken = tokenString, // In a real scenario, generate a proper refresh token
                ExpiresAt = token.ValidTo
            };
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            
            if (securityToken is not JwtSecurityToken jwtSecurityToken || 
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }