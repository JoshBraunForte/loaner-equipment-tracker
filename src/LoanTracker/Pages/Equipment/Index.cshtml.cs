using LoanTracker.Data;
using LoanTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LoanTracker.Pages.Equipment;

[Authorize(Roles = "ItStaff")]
public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public IndexModel(AppDbContext db) => _db = db;

    public List<LoanTracker.Models.Equipment> Items { get; private set; } = new();

    public async Task OnGetAsync()
    {
        Items = await _db.Equipment
            .OrderBy(e => e.AssetTag)
            .ToListAsync();
    }
}
