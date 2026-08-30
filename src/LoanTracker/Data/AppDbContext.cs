using LoanTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace LoanTracker.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Equipment> Equipment => Set<Equipment>();
    public DbSet<LoanRequest> LoanRequests => Set<LoanRequest>();
    public DbSet<Loan> Loans => Set<Loan>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<AppUser>().HasIndex(u => u.UserName).IsUnique();
        b.Entity<Equipment>().HasIndex(e => e.AssetTag).IsUnique();

        // Store enums as readable text in the DB.
        b.Entity<AppUser>().Property(u => u.Role).HasConversion<string>();
        b.Entity<Equipment>().Property(e => e.Status).HasConversion<string>();
        b.Entity<LoanRequest>().Property(r => r.Status).HasConversion<string>();

        b.Entity<AppUser>()
            .HasOne(u => u.Manager)
            .WithMany(u => u.Reports)
            .HasForeignKey(u => u.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<LoanRequest>()
            .HasOne(r => r.Requester)
            .WithMany()
            .HasForeignKey(r => r.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<LoanRequest>()
            .HasOne(r => r.DecidedBy)
            .WithMany()
            .HasForeignKey(r => r.DecidedById)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<Loan>()
            .HasOne(l => l.Borrower)
            .WithMany()
            .HasForeignKey(l => l.BorrowerId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<Loan>()
            .HasOne(l => l.Equipment)
            .WithMany(e => e.Loans)
            .HasForeignKey(l => l.EquipmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // One request fulfilled by at most one loan.
        b.Entity<LoanRequest>()
            .HasOne(r => r.Loan)
            .WithOne(l => l.Request)
            .HasForeignKey<Loan>(l => l.RequestId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
