using HaveServer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace HaveServer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<HSeller> Sellers { get; set; }
        public DbSet<HShop> Shops { get; set; }
        public DbSet<HProduct> Products { get; set; }
        public DbSet<HCategory> Categories { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Указываем точность и масштаб для свойства 'Cost' в HProduct
            modelBuilder.Entity<HProduct>()
                .Property(p => p.Cost)
                .HasColumnType("decimal(18,2)"); // Явно указываем тип данных и точность

            // Альтернативное использование HasPrecision для задания точности и масштаба
            modelBuilder.Entity<HProduct>()
                .Property(p => p.Cost)
                .HasPrecision(18, 2); // Устанавливаем точность 18 и масштаб 2
        }
    }
}
