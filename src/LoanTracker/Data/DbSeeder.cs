using LoanTracker.Models;
using Microsoft.AspNetCore.Identity;

namespace LoanTracker.Data;

public static class DbSeeder
{
    // Test-only shared password for every seeded account. Documented in README.
    public const string DefaultPassword = "Password123!";

    public static void Seed(AppDbContext db)
    {
        if (db.Users.Any()) return; // already seeded

        var hasher = new PasswordHasher<AppUser>();
        AppUser NewUser(string userName, string display, UserRole role, int? managerId = null)
        {
            var u = new AppUser { UserName = userName, DisplayName = display, Role = role, ManagerId = managerId };
            u.PasswordHash = hasher.HashPassword(u, DefaultPassword);
            return u;
        }

        // --- IT staff & managers first (so employees can reference manager ids) ---
        var ivan = NewUser("it1", "Ivan (IT Staff)", UserRole.ItStaff);
        var maria = NewUser("mgr1", "Maria Manager", UserRole.Manager);
        var mark = NewUser("mgr2", "Mark Manager", UserRole.Manager);
        db.Users.AddRange(ivan, maria, mark);
        db.SaveChanges();

        // --- Employees ---
        var emma = NewUser("emp1", "Emma Employee", UserRole.Employee, maria.Id);
        var evan = NewUser("emp2", "Evan Employee", UserRole.Employee, maria.Id);
        var olivia = NewUser("emp3", "Olivia Ops", UserRole.Employee, mark.Id);
        var oscar = NewUser("emp4", "Oscar Ops", UserRole.Employee, mark.Id);
        db.Users.AddRange(emma, evan, olivia, oscar);
        db.SaveChanges();

        // --- Equipment inventory ---
        var laptop1 = new Equipment { AssetTag = "LT-001", Name = "Dell Latitude 7440", Category = "Laptop" };
        var laptop2 = new Equipment { AssetTag = "LT-002", Name = "Dell Latitude 7440", Category = "Laptop" };
        var laptop3 = new Equipment { AssetTag = "LT-003", Name = "MacBook Pro 14\"", Category = "Laptop" };
        var monitor1 = new Equipment { AssetTag = "MN-001", Name = "Dell 27\" Monitor", Category = "Monitor" };
        var monitor2 = new Equipment { AssetTag = "MN-002", Name = "Dell 27\" Monitor", Category = "Monitor" };
        var dock1 = new Equipment { AssetTag = "DK-001", Name = "WD19 Docking Station", Category = "Dock" };
        var headset1 = new Equipment { AssetTag = "HS-001", Name = "Jabra Evolve2 Headset", Category = "Headset" };
        var headset2 = new Equipment { AssetTag = "HS-002", Name = "Jabra Evolve2 Headset", Category = "Headset" };
        var ipad1 = new Equipment { AssetTag = "TB-001", Name = "iPad Air", Category = "Tablet" };
        var projector1 = new Equipment { AssetTag = "PJ-001", Name = "Epson Projector", Category = "Projector" };
        db.Equipment.AddRange(laptop1, laptop2, laptop3, monitor1, monitor2, dock1, headset1, headset2, ipad1, projector1);
        db.SaveChanges();

        var today = DateTime.Today;

        // --- Active loans (one of them deliberately overdue) ---
        var loanEmma = new Loan { EquipmentId = laptop1.Id, BorrowerId = emma.Id, CheckedOutAt = today.AddDays(-10), DueDate = today.AddDays(5) };
        laptop1.Status = EquipmentStatus.OnLoan;

        var loanEvan = new Loan { EquipmentId = monitor1.Id, BorrowerId = evan.Id, CheckedOutAt = today.AddDays(-30), DueDate = today.AddDays(-3) }; // OVERDUE
        monitor1.Status = EquipmentStatus.OnLoan;

        var loanOlivia = new Loan { EquipmentId = headset1.Id, BorrowerId = olivia.Id, CheckedOutAt = today.AddDays(-2), DueDate = today.AddDays(10) };
        headset1.Status = EquipmentStatus.OnLoan;

        db.Loans.AddRange(loanEmma, loanEvan, loanOlivia);
        db.SaveChanges();

        // --- Requests in various workflow states ---
        // Pending Maria's approval
        db.LoanRequests.Add(new LoanRequest
        {
            RequesterId = emma.Id,
            ItemDescription = "Docking station for home office",
            Category = "Dock",
            Justification = "Working from home 3 days/week",
            Status = RequestStatus.Submitted,
            CreatedAt = today.AddDays(-1)
        });
        // Pending Mark's approval
        db.LoanRequests.Add(new LoanRequest
        {
            RequesterId = olivia.Id,
            ItemDescription = "Projector for client demo",
            Category = "Projector",
            Justification = "On-site customer demo next week",
            Status = RequestStatus.Submitted,
            CreatedAt = today
        });
        // Approved by Maria, waiting in IT's fulfillment queue
        db.LoanRequests.Add(new LoanRequest
        {
            RequesterId = evan.Id,
            ItemDescription = "iPad for field testing",
            Category = "Tablet",
            Justification = "QA on tablet form factor",
            Status = RequestStatus.Approved,
            CreatedAt = today.AddDays(-2),
            DecidedById = maria.Id,
            DecidedAt = today.AddDays(-1),
            DecisionNote = "Approved — go ahead."
        });
        db.SaveChanges();
    }
}
