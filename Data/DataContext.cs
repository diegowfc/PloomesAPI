using Microsoft.EntityFrameworkCore;
using PloomesAPI.Models;
using StoreAPI.Model;

namespace StoreAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options)
        {
           
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<ItemCategory> ItemCategories { get; set; }
    }
}
