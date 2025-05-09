public class TokenDto
{
    public string? AccessToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string? RefreshToken { get; set; }
}