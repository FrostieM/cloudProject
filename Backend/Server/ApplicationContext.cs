using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
