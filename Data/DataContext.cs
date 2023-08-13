using Microsoft.EntityFrameworkCore;
using StoreAPI.Model;

namespace StoreAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Item> Itens { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
