public interface IUserRepository
{
    public Task<ApplicationUser> GetUserByIdAsync(string userId);
    public Task<ApplicationUser> GetUserByEmailAsync(string email);
    public Task<ApplicationUser> GetUserByUsernameAsync(string username);
    public Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
    public Task<bool> RegisterUserAsync(RegisterUserDto registerUser);
    
}