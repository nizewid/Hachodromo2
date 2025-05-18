using Hachodromo.Shared.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hachodromo.API.Data
{
    public class DataContext : IdentityDbContext<User, IdentityRole<Guid>,Guid>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Country> Countries { get; set; } = null!;
        public DbSet<Region> Regions { get; set; } = null!;
        public DbSet<City> Cities { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Item> Items { get; set; } = null!;
        public DbSet<ItemCategory> ItemCategories { get; set; } = null!;
        public DbSet<ItemImage> ItemImages { get; set; } = null!;

        public DbSet<Membership> Memberships { get; set; } = null!;

        public DbSet<Site> Sites { get; set; } = null!;

        public DbSet<Target> Targets { get; set; } = null!;

        public DbSet<Reservation> Reservations { get; set; } = null!;

        public DbSet<ReservationTarget> ReservationTargets { get; set; } = null!;

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
                                    .WithMany(m => m.Users)      // ← bind to the Membership.Users nav
                                    .HasForeignKey(u => u.MembershipId)
                                    .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Site>().HasIndex(x => x.Name).IsUnique();
            modelBuilder.Entity<Site>().HasIndex(x => x.Name).IsUnique();
            modelBuilder.Entity<ReservationTarget>()
                                    .HasOne(rt => rt.Reservation)
                                    .WithMany(r => r.ReservationTargets)
                                    .HasForeignKey(rt => rt.ReservationId)
                                    .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ReservationTarget>()
                                    .HasOne(rt => rt.Target)
                                    .WithMany(t => t.ReservationTargets)
                                    .HasForeignKey(rt => rt.TargetId)
                                    .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
