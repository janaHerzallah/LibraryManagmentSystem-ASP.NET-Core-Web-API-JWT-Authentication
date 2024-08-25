using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace video.NET8.Data
{
    public class AuthDbContext : IdentityDbContext<IdentityUser>
    {

        public AuthDbContext(DbContextOptions options) : base(options)
        {

        }

    }
}
