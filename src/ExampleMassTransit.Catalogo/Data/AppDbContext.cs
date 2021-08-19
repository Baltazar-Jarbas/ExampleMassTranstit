using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ExampleMassTransit.Catalog.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options)
              : base(options)
        {
        }

        public DbSet<Domain.Model.Catalog> Catalogs { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Domain.Model.Catalog>().HasKey(x => x.Id);
        }
    }
}