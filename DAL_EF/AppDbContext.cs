using System.Data.Entity;
using MySellerApp.Models;

namespace MySellerApp.DAL_EF
{
    public class AppDbContext : DbContext
    {
        public AppDbContext()
            : base("name=ElectronicShopDB")
        {
            Database.SetInitializer<AppDbContext>(null);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
        public DbSet<SaleOrder> SaleOrders { get; set; }
        public DbSet<SaleOrderDetail> SaleOrderDetails { get; set; }
        public DbSet<ImportOrder> ImportOrders { get; set; }
        public DbSet<ImportOrderDetail> ImportOrderDetails { get; set; }
        public DbSet<ExportOrder> ExportOrders { get; set; }
        public DbSet<ExportOrderDetail> ExportOrderDetails { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== Users =====
            modelBuilder.Entity<User>()
                .ToTable("Users")
                .HasKey(u => u.Id);
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasDatabaseGeneratedOption(
                    System.ComponentModel.DataAnnotations.Schema
                    .DatabaseGeneratedOption.Identity);

            // ===== Categories =====
            modelBuilder.Entity<Category>()
                .ToTable("Categories")
                .HasKey(c => c.Id);
            modelBuilder.Entity<Category>()
                .Property(c => c.Id)
                .HasDatabaseGeneratedOption(
                    System.ComponentModel.DataAnnotations.Schema
                    .DatabaseGeneratedOption.Identity);

            // ===== Suppliers =====
            modelBuilder.Entity<Supplier>()
                .ToTable("Suppliers")
                .HasKey(s => s.Id);
            modelBuilder.Entity<Supplier>()
                .Property(s => s.Id)
                .HasDatabaseGeneratedOption(
                    System.ComponentModel.DataAnnotations.Schema
                    .DatabaseGeneratedOption.Identity);

            // ===== Products =====
            modelBuilder.Entity<Product>()
                .ToTable("Products")
                .HasKey(p => p.Id);
            modelBuilder.Entity<Product>()
                .Property(p => p.Id)
                .HasDatabaseGeneratedOption(
                    System.ComponentModel.DataAnnotations.Schema
                    .DatabaseGeneratedOption.Identity);
            // Ignore computed/display properties
            modelBuilder.Entity<Product>()
                .Ignore(p => p.Category);
            modelBuilder.Entity<Product>()
                .Ignore(p => p.Supplier);

            // ===== StockMovements =====
            modelBuilder.Entity<StockMovement>()
                .ToTable("StockMovements")
                .HasKey(s => s.Id);
            modelBuilder.Entity<StockMovement>()
                .Property(s => s.Id)
                .HasDatabaseGeneratedOption(
                    System.ComponentModel.DataAnnotations.Schema
                    .DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<StockMovement>()
                .Ignore(s => s.ProductName);
            modelBuilder.Entity<StockMovement>()
                .Ignore(s => s.UserName);

            // ===== SaleOrders =====
            modelBuilder.Entity<SaleOrder>()
                .ToTable("SaleOrders")
                .HasKey(s => s.Id);
            modelBuilder.Entity<SaleOrder>()
                .Property(s => s.Id)
                .HasDatabaseGeneratedOption(
                    System.ComponentModel.DataAnnotations.Schema
                    .DatabaseGeneratedOption.Identity);

            // ===== SaleOrderDetails =====
            modelBuilder.Entity<SaleOrderDetail>()
                .ToTable("SaleOrderDetails")
                .HasKey(s => s.Id);
            modelBuilder.Entity<SaleOrderDetail>()
                .Property(s => s.Id)
                .HasDatabaseGeneratedOption(
                    System.ComponentModel.DataAnnotations.Schema
                    .DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<SaleOrderDetail>()
                .Ignore(s => s.ProductName);
            modelBuilder.Entity<SaleOrderDetail>()
                .Ignore(s => s.SKU);
            modelBuilder.Entity<SaleOrderDetail>()
                .Ignore(s => s.SubTotal);

            // ===== ImportOrders =====
            modelBuilder.Entity<ImportOrder>()
                .ToTable("ImportOrders")
                .HasKey(i => i.Id);
            modelBuilder.Entity<ImportOrder>()
                .Property(i => i.Id)
                .HasDatabaseGeneratedOption(
                    System.ComponentModel.DataAnnotations.Schema
                    .DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<ImportOrder>()
                .Ignore(i => i.UserName);
            modelBuilder.Entity<ImportOrder>()
                .Ignore(i => i.Supplier);

            // ===== ImportOrderDetails =====
            modelBuilder.Entity<ImportOrderDetail>()
                .ToTable("ImportOrderDetails")
                .HasKey(i => i.Id);
            modelBuilder.Entity<ImportOrderDetail>()
                .Property(i => i.Id)
                .HasDatabaseGeneratedOption(
                    System.ComponentModel.DataAnnotations.Schema
                    .DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<ImportOrderDetail>()
                .Ignore(i => i.ProductName);

            // ===== ExportOrders =====
            modelBuilder.Entity<ExportOrder>()
                .ToTable("ExportOrders")
                .HasKey(e => e.Id);
            modelBuilder.Entity<ExportOrder>()
                .Property(e => e.Id)
                .HasDatabaseGeneratedOption(
                    System.ComponentModel.DataAnnotations.Schema
                    .DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<ExportOrder>()
                .Ignore(e => e.UserName);

            // ===== ExportOrderDetails =====
            modelBuilder.Entity<ExportOrderDetail>()
                .ToTable("ExportOrderDetails")
                .HasKey(e => e.Id);
            modelBuilder.Entity<ExportOrderDetail>()
                .Property(e => e.Id)
                .HasDatabaseGeneratedOption(
                    System.ComponentModel.DataAnnotations.Schema
                    .DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<ExportOrderDetail>()
                .Ignore(e => e.ProductName);
        }
    }
}