using FerticaStockManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FerticaStockManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ==============================
        // BUSINESS TABLES
        // ==============================

        public DbSet<Supplier> Suppliers { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<RawMaterial> RawMaterials { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductionOrder> ProductionOrders { get; set; }

        public DbSet<ProductionDetail> ProductionDetails { get; set; }

        public DbSet<StockEntry> StockEntries { get; set; }

        public DbSet<StockOutput> StockOutputs { get; set; }

        public DbSet<StockMovement> StockMovements { get; set; }

        public DbSet<AuditLog> AuditLogs { get; set; }

        public DbSet<Report> Reports { get; set; }


        // ==============================
        // DATABASE CONFIGURATION
        // ==============================

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            // ==========================================
            // SUPPLIER → RAW MATERIAL
            // ==========================================

            builder.Entity<RawMaterial>()
                .HasOne(r => r.Supplier)
                .WithMany(s => s.RawMaterials)
                .HasForeignKey(r => r.SupplierId)
                .OnDelete(DeleteBehavior.SetNull);


            // ==========================================
            // CATEGORY → PRODUCT
            // ==========================================

            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);


            // ==========================================
            // PRODUCTION ORDER → PRODUCT
            // ==========================================

            builder.Entity<ProductionOrder>()
                .HasOne(p => p.Product)
                .WithMany(p => p.ProductionOrders)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Restrict);


            // ==========================================
            // PRODUCTION DETAIL → PRODUCTION ORDER
            // ==========================================

            builder.Entity<ProductionDetail>()
                .HasOne(d => d.ProductionOrder)
                .WithMany(p => p.ProductionDetails)
                .HasForeignKey(d => d.ProductionOrderId)
                .OnDelete(DeleteBehavior.Cascade);


            // ==========================================
            // PRODUCTION DETAIL → RAW MATERIAL
            // ==========================================

            builder.Entity<ProductionDetail>()
                .HasOne(d => d.RawMaterial)
                .WithMany(r => r.ProductionDetails)
                .HasForeignKey(d => d.RawMaterialId)
                .OnDelete(DeleteBehavior.Restrict);


            // ==========================================
            // STOCK ENTRY → RAW MATERIAL
            // ==========================================

            builder.Entity<StockEntry>()
                .HasOne(e => e.RawMaterial)
                .WithMany(r => r.StockEntries)
                .HasForeignKey(e => e.RawMaterialId)
                .OnDelete(DeleteBehavior.Restrict);


            // ==========================================
            // STOCK ENTRY → SUPPLIER
            // ==========================================

            builder.Entity<StockEntry>()
                .HasOne(e => e.Supplier)
                .WithMany(s => s.StockEntries)
                .HasForeignKey(e => e.SupplierId)
                .OnDelete(DeleteBehavior.SetNull);


            // ==========================================
            // STOCK OUTPUT → PRODUCT
            // ==========================================

            builder.Entity<StockOutput>()
                .HasOne(o => o.Product)
                .WithMany(p => p.StockOutputs)
                .HasForeignKey(o => o.ProductId)
                .OnDelete(DeleteBehavior.Restrict);


            // ==========================================
            // STOCK MOVEMENT → RAW MATERIAL
            // ==========================================

            builder.Entity<StockMovement>()
                .HasOne(m => m.RawMaterial)
                .WithMany(r => r.StockMovements)
                .HasForeignKey(m => m.RawMaterialId)
                .OnDelete(DeleteBehavior.Restrict);


            // ==========================================
            // STOCK MOVEMENT → PRODUCT
            // ==========================================

            builder.Entity<StockMovement>()
                .HasOne(m => m.Product)
                .WithMany(p => p.StockMovements)
                .HasForeignKey(m => m.ProductId)
                .OnDelete(DeleteBehavior.Restrict);


            // ==========================================
            // DECIMAL PRECISION
            // ==========================================

            builder.Entity<RawMaterial>()
                .Property(r => r.CurrentStock)
                .HasPrecision(18, 3);

            builder.Entity<RawMaterial>()
                .Property(r => r.MinimumStock)
                .HasPrecision(18, 3);

            builder.Entity<RawMaterial>()
                .Property(r => r.UnitPrice)
                .HasPrecision(18, 2);


            builder.Entity<Product>()
                .Property(p => p.CurrentStock)
                .HasPrecision(18, 3);

            builder.Entity<Product>()
                .Property(p => p.MinimumStock)
                .HasPrecision(18, 3);

            builder.Entity<Product>()
                .Property(p => p.SellingPrice)
                .HasPrecision(18, 2);


            builder.Entity<ProductionOrder>()
                .Property(p => p.PlannedQuantity)
                .HasPrecision(18, 3);

            builder.Entity<ProductionOrder>()
                .Property(p => p.ProducedQuantity)
                .HasPrecision(18, 3);


            builder.Entity<ProductionDetail>()
                .Property(d => d.QuantityUsed)
                .HasPrecision(18, 3);


            builder.Entity<StockEntry>()
                .Property(e => e.Quantity)
                .HasPrecision(18, 3);

            builder.Entity<StockEntry>()
                .Property(e => e.UnitPrice)
                .HasPrecision(18, 2);


            builder.Entity<StockOutput>()
                .Property(o => o.Quantity)
                .HasPrecision(18, 3);


            builder.Entity<StockMovement>()
                .Property(m => m.Quantity)
                .HasPrecision(18, 3);

            builder.Entity<StockMovement>()
                .Property(m => m.StockBefore)
                .HasPrecision(18, 3);

            builder.Entity<StockMovement>()
                .Property(m => m.StockAfter)
                .HasPrecision(18, 3);


            // ==========================================
            // UNIQUE CONSTRAINTS
            // ==========================================

            builder.Entity<RawMaterial>()
                .HasIndex(r => r.Code)
                .IsUnique();

            builder.Entity<Product>()
                .HasIndex(p => p.Code)
                .IsUnique();

            builder.Entity<ProductionOrder>()
                .HasIndex(p => p.OrderNumber)
                .IsUnique();

            builder.Entity<StockEntry>()
                .HasIndex(e => e.Reference)
                .IsUnique();

            builder.Entity<StockOutput>()
                .HasIndex(o => o.Reference)
                .IsUnique();

            builder.Entity<StockMovement>()
                .HasIndex(m => m.Reference)
                .IsUnique();
        }
    }
}