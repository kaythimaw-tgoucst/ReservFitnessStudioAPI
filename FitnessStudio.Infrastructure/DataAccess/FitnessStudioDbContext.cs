using System;
using System.Collections.Generic;
using FitnessStudio.Infrastructure.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace FitnessStudio.Infrastructure.DataAccess;

public partial class FitnessStudioDbContext : DbContext
{
    public FitnessStudioDbContext(DbContextOptions<FitnessStudioDbContext> options)
        : base(options)
    {
    }

    // Allows derived contexts (e.g. AppWriteDbContext) to reuse this context's
    // entity mappings while being registered with their own DbContextOptions<TContext>.
    protected FitnessStudioDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public virtual DbSet<TBooking> TBookings { get; set; }

    public virtual DbSet<TBookingWaitlist> TBookingWaitlists { get; set; }

    public virtual DbSet<TBusinessStudio> TBusinessStudios { get; set; }

    public virtual DbSet<TCompany> TCompanies { get; set; }

    public virtual DbSet<TNumberingFormat> TNumberingFormats { get; set; }

    public virtual DbSet<TPackage> TPackages { get; set; }

    public virtual DbSet<TTimetableSchedule> TTimetableSchedules { get; set; }

    public virtual DbSet<TTransaction> TTransactions { get; set; }

    public virtual DbSet<TUser> TUsers { get; set; }

    public virtual DbSet<TUserBusinessStudioMembership> TUserBusinessStudioMemberships { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TBooking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tBooking__3214EC07DF63E64D");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.BookedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Package).WithMany(p => p.TBookings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_Package");

            entity.HasOne(d => d.TimetableSchedule).WithMany(p => p.TBookings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_Schedule");

            entity.HasOne(d => d.User).WithMany(p => p.TBookings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_User");
        });

        modelBuilder.Entity<TBookingWaitlist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tBooking__3214EC07282F208E");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.JoinedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.TimetableSchedule).WithMany(p => p.TBookingWaitlists)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Waitlist_Schedule");

            entity.HasOne(d => d.User).WithMany(p => p.TBookingWaitlists)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Waitlist_User");
        });

        modelBuilder.Entity<TBusinessStudio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tBusines__3214EC07B9B97412");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Company).WithMany(p => p.TBusinessStudios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BusinessStudio_Company");
        });

        modelBuilder.Entity<TCompany>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tCompany__3214EC070E9D1D04");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<TNumberingFormat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tNumberi__3214EC077DEB095C");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.YearMonth).IsFixedLength();

            entity.HasOne(d => d.BusinessStudio).WithMany(p => p.TNumberingFormats)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NumberingFormat_BusinessStudio");
        });

        modelBuilder.Entity<TPackage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tPackage__3214EC07507CAC5A");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.BusinessStudio).WithMany(p => p.TPackages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Package_BusinessStudio");

            entity.HasOne(d => d.User).WithMany(p => p.TPackages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Package_User");
        });

        modelBuilder.Entity<TTimetableSchedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tTimetab__3214EC07ABC6C4D6");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.BusinessStudio).WithMany(p => p.TTimetableSchedules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Schedule_BusinessStudio");
        });

        modelBuilder.Entity<TTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tTransac__3214EC071E042731");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Booking).WithMany(p => p.TTransactions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_Booking");

            entity.HasOne(d => d.BusinessStudio).WithMany(p => p.TTransactions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_BusinessStudio");
        });

        modelBuilder.Entity<TUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tUser__3214EC0706B92EC6");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<TUserBusinessStudioMembership>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.BusinessStudioId }).HasName("PK_tUserBusinessStudioMembership");

            entity.HasIndex(e => new { e.BusinessStudioId, e.UserId }, "IX_tUserBusinessStudioMembership_BusinessStudioId_UserId");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User).WithMany(p => p.TUserBusinessStudioMemberships)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserBusinessStudioMembership_User");

            entity.HasOne(d => d.BusinessStudio).WithMany(p => p.TUserBusinessStudioMemberships)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserBusinessStudioMembership_BusinessStudio");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
