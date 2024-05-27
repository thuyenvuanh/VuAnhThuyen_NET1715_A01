using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace Repository.Models
{
    public partial class HotelManagementContext : DbContext
    {
        private IConfiguration configuration = null;

        public HotelManagementContext()
        {
            this.configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }

        public HotelManagementContext(DbContextOptions<HotelManagementContext> options)
            : base(options)
        {
            this.configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }

        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Reservation> Reservations { get; set; } = null!;
        public virtual DbSet<Room> Rooms { get; set; } = null!;
        public virtual DbSet<RoomType> RoomTypes { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("HotelManagement"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("CUSTOMER");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.FullName).HasMaxLength(50);

                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.Status).HasDefaultValueSql("((0))");

                entity.Property(e => e.Telephone).HasMaxLength(12);
            });

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.ToTable("RESERVATION");

                entity.Property(e => e.Status)
                    .HasMaxLength(10)
                    .HasDefaultValueSql("('PENDING')");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Reservations)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__RESERVATI__Custo__412EB0B6");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Reservations)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("FK__RESERVATI__RoomI__403A8C7D");
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("ROOM");

                entity.Property(e => e.Description).HasMaxLength(220);

                entity.Property(e => e.PricePerDate).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Status).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Rooms)
                    .HasForeignKey(d => d.TypeId)
                    .HasConstraintName("FK__ROOM__TypeId__3D5E1FD2");
            });

            modelBuilder.Entity<RoomType>(entity =>
            {
                entity.ToTable("ROOM_TYPE");

                entity.Property(e => e.Description).HasMaxLength(220);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Note).HasMaxLength(220);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
