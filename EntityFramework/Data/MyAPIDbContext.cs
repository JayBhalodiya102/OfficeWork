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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define the relationship between DetailsModel and LoginModel
            modelBuilder.Entity<LoginModel>().HasOne(X => X.DM).WithMany().HasForeignKey(X=>X.DId);

        }

    }
}
