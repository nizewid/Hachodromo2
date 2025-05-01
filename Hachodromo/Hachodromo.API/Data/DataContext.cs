using Hachodromo.Shared.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hachodromo.API.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Country> Countries { get; set; } = null!;
        public DbSet<Region> Regions { get; set; } = null!;
        public DbSet<City> Cities { get; set; } = null!;
        public DbSet<Category> Categories { get; set; }
        public DbSet<Item> Items { get; set; } = null!;
        public DbSet<ItemCategory> ItemCategories { get; set; } = null!;
        public DbSet<ItemImage> ItemImages { get; set; } = null!;

        public DbSet<Membership> Memberships { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().HasIndex(x => x.Name).IsUnique();
            modelBuilder.Entity<Region>().HasIndex(x => new { x.RegionName, x.CountryId }).IsUnique();
            modelBuilder.Entity<City>().HasIndex(x => new { x.CityName, x.RegionId }).IsUnique();
            modelBuilder.Entity<Category>().HasIndex(x => x.Name).IsUnique();
            modelBuilder.Entity<Item>().HasIndex(x => x.Name).IsUnique();
            modelBuilder.Entity<Membership>().HasIndex(x => x.Name).IsUnique();
            modelBuilder.Entity<User>()
                                    .HasOne(u => u.Membership)
                                    .WithMany()
                                    .HasForeignKey(u => u.MembershipId)
                                    .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
