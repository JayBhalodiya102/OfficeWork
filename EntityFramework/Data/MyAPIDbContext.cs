using EntityFramework.Model;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Data
{
    public class MyAPIDbContext : DbContext
    {
        public MyAPIDbContext(DbContextOptions Option) : base(Option)
        {

        }

        public DbSet<DetailsModel> Details { get; set; }
        public DbSet<LoginModel> Login { get; set; }

    }
}
