using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Admin;

public class DataContext : IdentityDbContext<AppUser> // Type of Models > AppUser.cs
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        
    }
}
