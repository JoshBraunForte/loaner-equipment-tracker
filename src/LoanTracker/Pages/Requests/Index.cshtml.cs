using LoanTracker.Data;
using LoanTracker.Helpers;
using LoanTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LoanTracker.Pages.Requests;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public IndexModel(AppDbContext db) => _db = db;

    public List<LoanRequest> Requests { get; private set; } = new();

    public async Task OnGetAsync()
    {
        var uid = User.GetUserId();
        Requests = await _db.LoanRequests
            .Include(r => r.DecidedBy)
            .Where(r => r.RequesterId == uid)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostCancelAsync(int id)
    {
        var uid = User.GetUserId();
        var req = await _db.LoanRequests.FirstOrDefaultAsync(r => r.Id == id && r.RequesterId == uid);
        if (req is not null && req.Status == RequestStatus.Submitted)
        {
            req.Status = RequestStatus.Cancelled;
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}
