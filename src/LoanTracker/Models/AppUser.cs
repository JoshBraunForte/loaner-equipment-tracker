namespace LoanTracker.Models;

public class AppUser
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }

    // Self-referencing manager relationship. Seeded for the test; sourced from Entra ID later.
    public int? ManagerId { get; set; }
    public AppUser? Manager { get; set; }
    public ICollection<AppUser> Reports { get; set; } = new List<AppUser>();
}
