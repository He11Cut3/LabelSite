using LabelSite.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LabelSite.Infrastructure
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        public DbSet<SalesData> SalesDatas { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<SalesProduct> SalesProducts { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(p => p.UserId);

            modelBuilder.Entity<SalesProduct>()
                .HasKey(sp => new { sp.SalesDataId, sp.ProductId });

            modelBuilder.Entity<SalesProduct>()
                .HasOne(sp => sp.SalesData)
                .WithMany(sd => sd.SalesProducts)
                .HasForeignKey(sp => sp.SalesDataId);

            modelBuilder.Entity<SalesProduct>()
                .HasOne(sp => sp.Product)
                .WithMany(p => p.SalesProducts)
                .HasForeignKey(sp => sp.ProductId);

            // Конфигурация связи между SalesData и User
            modelBuilder.Entity<SalesData>()
                .HasOne(sd => sd.User)
                .WithMany(u => u.SalesDatas)
                .HasForeignKey(sd => sd.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Настройка поведения при удалении
        }
    }
}
