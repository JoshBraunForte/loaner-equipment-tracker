using LoanTracker.Data;
using LoanTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LoanTracker.Pages.Queue;

[Authorize(Roles = "ItStaff")]
public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public IndexModel(AppDbContext db) => _db = db;

    public List<LoanRequest> Approved { get; private set; } = new();
    public List<SelectListItem> AvailableEquipment { get; private set; } = new();
    public string? Message { get; set; }

    public async Task OnGetAsync() => await LoadAsync();

    private async Task LoadAsync()
    {
        Approved = await _db.LoanRequests
            .Include(r => r.Requester)
            .Where(r => r.Status == RequestStatus.Approved)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();

        AvailableEquipment = await _db.Equipment
            .Where(e => e.Status == EquipmentStatus.Available)
            .OrderBy(e => e.AssetTag)
            .Select(e => new SelectListItem($"{e.AssetTag} — {e.Name}", e.Id.ToString()))
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostFulfillAsync(int requestId, int equipmentId, DateTime dueDate)
    {
        var req = await _db.LoanRequests.FirstOrDefaultAsync(r => r.Id == requestId);
        var equip = await _db.Equipment.FirstOrDefaultAsync(e => e.Id == equipmentId);

        if (req is null || req.Status != RequestStatus.Approved
            || equip is null || equip.Status != EquipmentStatus.Available)
        {
            Message = "Could not fulfill: the request or equipment is no longer available.";
            await LoadAsync();
            return Page();
        }

        var loan = new Loan
        {
            EquipmentId = equip.Id,
            BorrowerId = req.RequesterId,
            RequestId = req.Id,
            CheckedOutAt = DateTime.Now,
            DueDate = dueDate.Date
        };
        _db.Loans.Add(loan);

        equip.Status = EquipmentStatus.OnLoan;
        req.Status = RequestStatus.Fulfilled;

        await _db.SaveChangesAsync();
        return RedirectToPage();
    }
}
