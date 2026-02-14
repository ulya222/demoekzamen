using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Demo1.Entities;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Orderproduct> Orderproducts { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5050;Database=demo;Username=postgres;Password=ulyana26");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Addressid).HasName("address_pkey");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Categoryid).HasName("category_pkey");
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.Manufacturerid).HasName("manufacturer_pkey");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Orderid).HasName("ORDER_pkey");

            entity.HasOne(d => d.OrderaddressNavigation).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("ORDER_orderaddress_fkey");

            entity.HasOne(d => d.OrderstatusNavigation).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("ORDER_orderstatus_fkey");

            entity.HasOne(d => d.OrderuserNavigation).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("ORDER_orderuser_fkey");
        });

        modelBuilder.Entity<Orderproduct>(entity =>
        {
            entity.HasKey(e => e.Orderproductid).HasName("orderproduct_pkey");

            entity.HasOne(d => d.OrderNavigation).WithMany(p => p.Orderproducts)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("orderproduct_ORDER_fkey");

            entity.HasOne(d => d.ProductNavigation).WithMany(p => p.Orderproducts)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("orderproduct_product_fkey");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Productarticul).HasName("product_pkey");

            entity.HasOne(d => d.CategoryNavigation).WithMany(p => p.Products).HasConstraintName("product_category_fkey");

            entity.HasOne(d => d.ManufacturerNavigation).WithMany(p => p.Products).HasConstraintName("product_manufacturer_fkey");

            entity.HasOne(d => d.SupplierNavigation).WithMany(p => p.Products).HasConstraintName("product_supplier_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Roleid).HasName("role_pkey");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Statusid).HasName("status_pkey");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Supplierid).HasName("supplier_pkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("USER_pkey");

            entity.HasOne(d => d.RoleNavigation).WithMany(p => p.Users).HasConstraintName("USER_role_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
