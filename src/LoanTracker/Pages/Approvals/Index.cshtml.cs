using LoanTracker.Data;
using LoanTracker.Helpers;
using LoanTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LoanTracker.Pages.Approvals;

[Authorize(Roles = "Manager")]
public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public IndexModel(AppDbContext db) => _db = db;

    public List<LoanRequest> Pending { get; private set; } = new();
    public List<LoanRequest> Decided { get; private set; } = new();

    public async Task OnGetAsync()
    {
        var uid = User.GetUserId();

        Pending = await _db.LoanRequests
            .Include(r => r.Requester)
            .Where(r => r.Status == RequestStatus.Submitted && r.Requester.ManagerId == uid)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();

        Decided = await _db.LoanRequests
            .Include(r => r.Requester)
            .Where(r => r.Requester.ManagerId == uid
                        && (r.Status == RequestStatus.Approved
                            || r.Status == RequestStatus.Denied
                            || r.Status == RequestStatus.Fulfilled))
            .OrderByDescending(r => r.DecidedAt)
            .Take(10)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDecideAsync(int id, string decision, string? note)
    {
        var uid = User.GetUserId();
        var req = await _db.LoanRequests
            .Include(r => r.Requester)
            .FirstOrDefaultAsync(r => r.Id == id);

        // Only the requester's own manager may decide, and only while still submitted.
        if (req is null || req.Requester.ManagerId != uid || req.Status != RequestStatus.Submitted)
            return RedirectToPage();

        req.Status = decision == "approve" ? RequestStatus.Approved : RequestStatus.Denied;
        req.DecidedById = uid;
        req.DecidedAt = DateTime.Now;
        req.DecisionNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }
}
