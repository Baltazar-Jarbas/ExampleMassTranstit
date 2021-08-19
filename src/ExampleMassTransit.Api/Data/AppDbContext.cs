using ExampleMassTransit.Order.Api.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace ExampleMassTransit.Order.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options)
              : base(options)
        {
        }

        public DbSet<Api.Domain.Model.Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Api.Domain.Model.Order>().HasKey(x => x.Id);
            modelBuilder.Entity<OrderItem>().HasKey(x => x.Id);
            modelBuilder.Entity<OrderItem>().HasOne(x => x.Order).WithMany(c => c.Items).HasForeignKey(x => x.OrderId);
        }
    }
}