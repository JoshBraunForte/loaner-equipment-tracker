using LoanTracker.Data;
using LoanTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LoanTracker.Pages.Loans;

[Authorize(Roles = "ItStaff")]
public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public IndexModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)] public bool OverdueOnly { get; set; }

    public List<Loan> Active { get; private set; } = new();

    public async Task OnGetAsync()
    {
        var today = DateTime.Today;
        var query = _db.Loans
            .Include(l => l.Equipment)
            .Include(l => l.Borrower)
            .Where(l => l.ReturnedAt == null);

        if (OverdueOnly)
            query = query.Where(l => l.DueDate < today);

        Active = await query.OrderBy(l => l.DueDate).ToListAsync();
    }

    public async Task<IActionResult> OnPostCheckInAsync(int id)
    {
        var loan = await _db.Loans.Include(l => l.Equipment).FirstOrDefaultAsync(l => l.Id == id);
        if (loan is not null && loan.ReturnedAt == null)
        {
            loan.ReturnedAt = DateTime.Now;
            loan.Equipment.Status = EquipmentStatus.Available;
            await _db.SaveChangesAsync();
        }
        return RedirectToPage(new { OverdueOnly });
    }
}
