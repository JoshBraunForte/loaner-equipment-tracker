using LoanTracker.Data;
using LoanTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LoanTracker.Pages.Audit;

[Authorize(Roles = "ItStaff")]
public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public IndexModel(AppDbContext db) => _db = db;

    public DateTime GeneratedAt { get; private set; }
    public List<Loan> OnLoan { get; private set; } = new();
    public List<LoanTracker.Models.Equipment> Available { get; private set; } = new();
    public int OverdueCount { get; private set; }

    public async Task OnGetAsync()
    {
        GeneratedAt = DateTime.Now;
        var today = DateTime.Today;

        OnLoan = await _db.Loans
            .Include(l => l.Equipment)
            .Include(l => l.Borrower)
            .Where(l => l.ReturnedAt == null)
            .OrderBy(l => l.Borrower.DisplayName)
            .ThenBy(l => l.Equipment.AssetTag)
            .ToListAsync();

        Available = await _db.Equipment
            .Where(e => e.Status == EquipmentStatus.Available)
            .OrderBy(e => e.AssetTag)
            .ToListAsync();

        OverdueCount = OnLoan.Count(l => l.DueDate.Date < today);
    }
}
