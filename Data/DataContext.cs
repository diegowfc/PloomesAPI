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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Role>()
                .HasMany(role => role.User)
                .WithOne(user => user.Role)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ItemCategory>()
                .HasMany(ItemCategory => ItemCategory.Item)
                .WithOne(item => item.ItemCategory)
                .OnDelete(DeleteBehavior.Restrict);
       
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<ItemCategory> ItemCategories { get; set; }
    }
}
