using Microsoft.EntityFrameworkCore;

namespace cloudSite.Model
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) 
            : base(options) {}
        public DbSet<User> Users { get; set; }
        public DbSet<LoggerUserInfo> LoggerUserInfos { get; set; }
    }
}