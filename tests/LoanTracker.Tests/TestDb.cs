using System.Security.Claims;
using LoanTracker.Data;
using LoanTracker.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace LoanTracker.Tests;

/// <summary>
/// Creates a fresh, fully-seeded AppDbContext backed by a private in-memory SQLite
/// database. The connection is held open for the lifetime of the context so the
/// schema and data survive between queries.
/// </summary>
public sealed class TestDb : IDisposable
{
    private readonly SqliteConnection _connection;
    public AppDbContext Db { get; }

    public TestDb(bool seed = true)
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        Db = new AppDbContext(options);
        Db.Database.EnsureCreated();
        if (seed) DbSeeder.Seed(Db);
    }

    public void Dispose()
    {
        Db.Dispose();
        _connection.Dispose();
    }

    /// <summary>Attach a signed-in user (id + role) to a Razor Page model.</summary>
    public static void SignIn(PageModel page, int userId, UserRole role)
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role.ToString())
        }, CookieAuthenticationDefaults.AuthenticationScheme);

        page.PageContext = new PageContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
    }
}
