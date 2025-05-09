using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(e => e.Id).HasColumnName("UserId").IsRequired();
            entity.Property(e => e.UserName).HasColumnName("UserName").IsRequired();
            entity.Property(e => e.Email).HasColumnName("Email").IsRequired();
            entity.Property(e => e.FirstName).HasColumnName("FirstName").IsRequired();
            entity.Property(e => e.LastName).HasColumnName("LastName").IsRequired();
        });
    }
}