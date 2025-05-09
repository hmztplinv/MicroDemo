
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    public UserRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }
    public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public Task<ApplicationUser> GetUserByEmailAsync(string email)
    {
        return _userManager.FindByEmailAsync(email);
    }

    public async Task<ApplicationUser> GetUserByIdAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public Task<ApplicationUser> GetUserByUsernameAsync(string username)
    {
        return _userManager.FindByNameAsync(username);
    }

    public async Task<bool> RegisterUserAsync(RegisterUserDto registerUser)
    {
        var user = new ApplicationUser
        {
            UserName = registerUser.UserName,
            Email = registerUser.Email,
            FirstName = registerUser.FirstName,
            LastName = registerUser.LastName,
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        };

        var result =await _userManager.CreateAsync(user, registerUser.Password);
        return result.Succeeded;
    }
}