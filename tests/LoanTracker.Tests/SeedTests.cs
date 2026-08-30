using LoanTracker.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LoanTracker.Tests;

public class SeedTests
{
    [Fact]
    public void Seed_creates_expected_users_equipment_loans_and_requests()
    {
        using var t = new TestDb();

        Assert.Equal(7, t.Db.Users.Count());
        Assert.Equal(10, t.Db.Equipment.Count());
        Assert.Equal(3, t.Db.Loans.Count());
        Assert.Equal(3, t.Db.LoanRequests.Count());
    }

    [Fact]
    public void Seed_is_idempotent()
    {
        using var t = new TestDb();
        DbSeeder_SeedAgain(t);
        Assert.Equal(7, t.Db.Users.Count()); // not doubled
    }

    private static void DbSeeder_SeedAgain(TestDb t) => LoanTracker.Data.DbSeeder.Seed(t.Db);

    [Fact]
    public void Employees_have_managers_and_managers_do_not()
    {
        using var t = new TestDb();

        var emma = t.Db.Users.Single(u => u.UserName == "emp1");
        var maria = t.Db.Users.Single(u => u.UserName == "mgr1");

        Assert.Equal(maria.Id, emma.ManagerId);
        Assert.Null(maria.ManagerId);
        Assert.Equal(UserRole.Employee, emma.Role);
        Assert.Equal(UserRole.Manager, maria.Role);
    }

    [Fact]
    public void Passwords_are_hashed_not_plaintext()
    {
        using var t = new TestDb();
        var emma = t.Db.Users.Single(u => u.UserName == "emp1");

        Assert.NotEqual(DbSeederPassword, emma.PasswordHash);
        Assert.NotEmpty(emma.PasswordHash);
    }

    private const string DbSeederPassword = "Password123!";

    [Fact]
    public void Seed_includes_one_overdue_loan()
    {
        using var t = new TestDb();
        var overdue = t.Db.Loans.Include(l => l.Equipment).ToList().Count(l => l.IsOverdue);
        Assert.Equal(1, overdue);
    }
}
