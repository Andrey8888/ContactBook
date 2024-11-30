using Microsoft.EntityFrameworkCore;

namespace ContactsBook
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Contact> Contacts { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=contactbook1.db");
        }
    }
}