using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }=null!;
    public string LastName { get; set; }=null!;
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
}