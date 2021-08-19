using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ExampleMassTransit.Product.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options)
              : base(options)
        {
        }

        public DbSet<Domain.Model.Product> Products { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Domain.Model.Product>().HasKey(x => x.Id);
        }
    }
}