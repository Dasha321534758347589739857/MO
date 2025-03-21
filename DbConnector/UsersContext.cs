using Microsoft.EntityFrameworkCore;
using MO_kursasch_25.DbConnector.Models;

namespace MO_kursasch_25.DbConnector
{
    public class UsersContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public UsersContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Users.db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
        }


    }
}
