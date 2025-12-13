using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace ShoeApp
{
    public partial class ShoeEntity : DbContext
    {
        public ShoeEntity()
            : base("name=ShoeEntity")
        {
        }

        public virtual DbSet<Categories> Categories { get; set; }
        public virtual DbSet<Clients> Clients { get; set; }
        public virtual DbSet<Manufacturers> Manufacturers { get; set; }
        public virtual DbSet<OrderItems> OrderItems { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<OrderStatus> OrderStatus { get; set; }
        public virtual DbSet<PickupPoints> PickupPoints { get; set; }
        public virtual DbSet<Products> Products { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<Suppliers> Suppliers { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categories>()
                .HasMany(e => e.Products)
                .WithOptional(e => e.Categories)
                .HasForeignKey(e => e.CategoryId);

            modelBuilder.Entity<Clients>()
                .HasMany(e => e.Orders)
                .WithOptional(e => e.Clients)
                .HasForeignKey(e => e.ClientId);

            modelBuilder.Entity<Manufacturers>()
                .HasMany(e => e.Products)
                .WithOptional(e => e.Manufacturers)
                .HasForeignKey(e => e.ManufacturerId);

            modelBuilder.Entity<Orders>()
                .HasMany(e => e.OrderItems)
                .WithOptional(e => e.Orders)
                .HasForeignKey(e => e.OrderId);

            modelBuilder.Entity<OrderStatus>()
                .HasMany(e => e.Orders)
                .WithOptional(e => e.OrderStatus)
                .HasForeignKey(e => e.StatusId);

            modelBuilder.Entity<PickupPoints>()
                .HasMany(e => e.Orders)
                .WithOptional(e => e.PickupPoints)
                .HasForeignKey(e => e.PickupPointId);

            modelBuilder.Entity<Products>()
                .HasMany(e => e.OrderItems)
                .WithOptional(e => e.Products)
                .HasForeignKey(e => e.ProductId);

            modelBuilder.Entity<Roles>()
                .HasMany(e => e.Users)
                .WithOptional(e => e.Roles)
                .HasForeignKey(e => e.RoleId);

            modelBuilder.Entity<Suppliers>()
                .HasMany(e => e.Products)
                .WithOptional(e => e.Suppliers)
                .HasForeignKey(e => e.SupplierId);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Orders)
                .WithOptional(e => e.Users)
                .HasForeignKey(e => e.ClientId);
        }
    }
}
