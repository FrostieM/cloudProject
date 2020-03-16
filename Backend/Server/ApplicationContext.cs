using System.Data.Entity;

namespace Server
{
    class ApplicationContext : DbContext
    {
        public ApplicationContext() : base("DefaultConnection")
        {
            Database.SetInitializer<ApplicationContext>(null);
        }
        public DbSet<Computer> Computers { get; set; }
        public DbSet<Program> Programs { get; set; }
    }
}
