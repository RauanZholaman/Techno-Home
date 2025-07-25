using System;
using System.Collections.Generic;
using Techno_Home.Models;
using Microsoft.EntityFrameworkCore;

namespace Techno_Home.Data;


public partial class StoreDbContext : DbContext
{
    public StoreDbContext()
    {
    }

    public StoreDbContext(DbContextOptions<StoreDbContext> options)
        : base(options)
    {
        ChangeTracker.LazyLoadingEnabled = false;  
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Patron> Patrons { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductsInOrder> ProductsInOrders { get; set; }

    public virtual DbSet<Source> Sources { get; set; }

    public virtual DbSet<Stocktake> Stocktakes { get; set; }

    public virtual DbSet<SubCategory> SubCategories { get; set; }

    public virtual DbSet<To> Tos { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=Warmachine;Database=StoreDB;Encrypt=False;Trusted_Connection=True;MultipleActiveResultSets=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__23CAF1F8A0BFAAFB");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryId)
                .ValueGeneratedNever()
                .HasColumnName("categoryID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAF1B12E68C");

            entity.Property(e => e.OrderId)
                .ValueGeneratedNever()
                .HasColumnName("OrderID");
            entity.Property(e => e.Customer).HasColumnName("customer");
            entity.Property(e => e.State).HasMaxLength(50);
            entity.Property(e => e.StreetAddress).HasMaxLength(255);
            entity.Property(e => e.Suburb).HasMaxLength(255);

            entity.HasOne(d => d.CustomerNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Customer)
                .HasConstraintName("FK__Orders__customer__47DBAE45");
        });

        modelBuilder.Entity<Patron>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Patrons__1788CCAC2DF458CF");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("UserID");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.HashedPw)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("HashedPW");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Salt)
                .HasMaxLength(32)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Product__3214EC2792542439");

            entity.ToTable("Product");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.BrandName).HasMaxLength(255);
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.ImagePath)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LastUpdated).HasColumnType("datetime");
            entity.Property(e => e.LastUpdatedBy).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SubCategoryId).HasColumnName("SubCategoryID");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Product__Categor__4316F928");

            // entity.HasOne(d => d.LastUpdatedByNavigation)
            //     .WithMany()
            //     .HasForeignKey(d => d.LastUpdatedBy)
            //     .HasPrincipalKey(p => p.UserName)
            //     .HasConstraintName("FK__Product__LastUpd__44FF419A")
            //     .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.SubCategory).WithMany(p => p.Products)
                .HasForeignKey(d => d.SubCategoryId)
                .HasConstraintName("FK__Product__SubCate__440B1D61");
        }); 

        modelBuilder.Entity<ProductsInOrder>(entity =>
        {
            entity.HasNoKey();

            entity.HasOne(d => d.Order).WithMany()
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__ProductsI__Order__5070F446");

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__ProductsI__Produ__5165187F");
        });

        modelBuilder.Entity<Source>(entity =>
        {
            entity.HasKey(e => e.Sourceid).HasName("PK__Source__5ABF0FB8217B99A7");

            entity.ToTable("Source");

            entity.Property(e => e.Sourceid)
                .ValueGeneratedNever()
                .HasColumnName("sourceid");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.ExternalLink)
                .HasMaxLength(255)
                .HasColumnName("externalLink");
            entity.Property(e => e.SourceName)
                .HasMaxLength(255)
                .HasColumnName("Source_name");

            entity.HasOne(d => d.Category).WithMany(p => p.Sources)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Source__Category__4AB81AF0");
        });

        modelBuilder.Entity<Stocktake>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__Stocktak__727E838BDF0BC290");

            entity.ToTable("Stocktake");

            entity.Property(e => e.ItemId).ValueGeneratedNever();

            entity.HasOne(d => d.Product).WithMany(p => p.Stocktakes)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Stocktake__Produ__4E88ABD4");

            entity.HasOne(d => d.Source).WithMany(p => p.Stocktakes)
                .HasForeignKey(d => d.SourceId)
                .HasConstraintName("FK__Stocktake__Sourc__4D94879B");
        });

        modelBuilder.Entity<SubCategory>(entity =>
        {
            entity.HasKey(e => e.SubCategoryId).HasName("PK__SubCateg__26BE5BF9CF19DD0C");

            entity.ToTable("SubCategory");

            entity.Property(e => e.SubCategoryId)
                .ValueGeneratedNever()
                .HasColumnName("SubCategoryID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.Category).WithMany(p => p.SubCategories)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SubCatego__Categ__398D8EEE");
        });

        modelBuilder.Entity<To>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__TO__B611CB9DC15F216C");

            entity.ToTable("TO");

            entity.Property(e => e.CustomerId)
                .ValueGeneratedNever()
                .HasColumnName("customerID");
            entity.Property(e => e.CardNumber).HasMaxLength(50);
            entity.Property(e => e.CardOwner).HasMaxLength(50);
            entity.Property(e => e.Cvv).HasColumnName("CVV");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Expiry)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.State).HasMaxLength(50);
            entity.Property(e => e.StreetAddress).HasMaxLength(255);
            entity.Property(e => e.Suburb).HasMaxLength(50);

            entity.HasOne(d => d.Patron).WithMany(p => p.Tos)
                .HasForeignKey(d => d.PatronId)
                .HasConstraintName("FK__TO__PatronId__403A8C7D");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserName).HasName("PK__User__C9F284573D9ADC85");

            entity.ToTable("User");

            entity.Property(e => e.UserName).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.HashedPw)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("HashedPW");
            entity.Property(e => e.IsAdmin).HasColumnName("isAdmin");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Salt)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
