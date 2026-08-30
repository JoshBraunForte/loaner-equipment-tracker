using LoanTracker.Data;
using LoanTracker.Helpers;
using LoanTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LoanTracker.Pages.Requests;

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;
    public CreateModel(AppDbContext db) => _db = db;

    [BindProperty] public string ItemDescription { get; set; } = string.Empty;
    [BindProperty] public string? Category { get; set; }
    [BindProperty] public string? Justification { get; set; }

    public string? ManagerName { get; private set; }
    public bool HasManager { get; private set; }

    public async Task OnGetAsync()
    {
        var uid = User.GetUserId();
        var me = await _db.Users.Include(u => u.Manager).FirstAsync(u => u.Id == uid);
        HasManager = me.Manager is not null;
        ManagerName = me.Manager?.DisplayName;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(ItemDescription))
        {
            ModelState.AddModelError(nameof(ItemDescription), "Please describe what you need.");
            await OnGetAsync();
            return Page();
        }

        var uid = User.GetUserId();
        _db.LoanRequests.Add(new LoanRequest
        {
            RequesterId = uid,
            ItemDescription = ItemDescription.Trim(),
            Category = string.IsNullOrWhiteSpace(Category) ? null : Category.Trim(),
            Justification = string.IsNullOrWhiteSpace(Justification) ? null : Justification.Trim(),
            Status = RequestStatus.Submitted,
            CreatedAt = DateTime.Now
        });
        await _db.SaveChangesAsync();
        return RedirectToPage("/Requests/Index");
    }
}
