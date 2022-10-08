using HomeOrganizer.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HomeOrganizer.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Group> Groups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Ad>()
                .HasOne(a => a.Voting)
                .WithOne()
                .HasForeignKey<Voting>(a => a.Id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
