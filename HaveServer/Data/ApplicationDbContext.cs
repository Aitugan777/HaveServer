using AitukServer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AitukServer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<ASeller> Sellers { get; set; }

        public DbSet<AProduct> Products { get; set; }
        public DbSet<AProductSize> ProductSizes { get; set; }
        public DbSet<AProductPhoto> ProductPhotos { get; set; }
        public DbSet<AProductShop> ProductShops { get; set; }

        public DbSet<AShop> Shops { get; set; }
        public DbSet<AShopPhoto> ShopPhotos { get; set; }
        public DbSet<AShopContact> ShopContacts { get; set; }
        public DbSet<AContactType> ContactTypes { get; set; }
        public DbSet<AWorkSheldure> WorkSheldures { get; set; }

        public DbSet<AColor> Colors { get; set; }
        public DbSet<ASize> Sizes { get; set; }
        public DbSet<AGender> Genders { get; set; }
        public DbSet<ACategory> Categories { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AProductShop>()
                .HasKey(ps => new { ps.ProductId, ps.ShopId });

            modelBuilder.Entity<AProductShop>()
                .HasOne(ps => ps.Product)
                .WithMany(p => p.ProductShops)
                .HasForeignKey(ps => ps.ProductId);

            modelBuilder.Entity<AProductShop>()
                .HasOne(ps => ps.Shop)
                .WithMany(s => s.ProductShops)
                .HasForeignKey(ps => ps.ShopId);

            modelBuilder.Entity<AShop>()
                .HasOne(s => s.WorkSheldure)
                .WithOne(w => w.Shop)
                .HasForeignKey<AWorkSheldure>(w => w.ShopId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AWorkSheldure>().OwnsOne(w => w.Monday);
            modelBuilder.Entity<AWorkSheldure>().OwnsOne(w => w.Tuesday);
            modelBuilder.Entity<AWorkSheldure>().OwnsOne(w => w.Wednesday);
            modelBuilder.Entity<AWorkSheldure>().OwnsOne(w => w.Thursday);
            modelBuilder.Entity<AWorkSheldure>().OwnsOne(w => w.Friday);
            modelBuilder.Entity<AWorkSheldure>().OwnsOne(w => w.Saturday);
            modelBuilder.Entity<AWorkSheldure>().OwnsOne(w => w.Sunday);
        }
    }

}
