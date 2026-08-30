using LoanTracker.Data;
using LoanTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LoanTracker.Pages.Equipment;

[Authorize(Roles = "ItStaff")]
public class UpsertModel : PageModel
{
    private readonly AppDbContext _db;
    public UpsertModel(AppDbContext db) => _db = db;

    [BindProperty] public int? Id { get; set; }
    [BindProperty] public string AssetTag { get; set; } = string.Empty;
    [BindProperty] public string Name { get; set; } = string.Empty;
    [BindProperty] public string Category { get; set; } = string.Empty;
    [BindProperty] public EquipmentStatus Status { get; set; } = EquipmentStatus.Available;
    [BindProperty] public string? Notes { get; set; }

    public bool IsEdit => Id.HasValue;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id.HasValue)
        {
            var e = await _db.Equipment.FindAsync(id.Value);
            if (e is null) return RedirectToPage("/Equipment/Index");
            Id = e.Id;
            AssetTag = e.AssetTag;
            Name = e.Name;
            Category = e.Category;
            Status = e.Status;
            Notes = e.Notes;
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(AssetTag) || string.IsNullOrWhiteSpace(Name))
        {
            ModelState.AddModelError(string.Empty, "Asset tag and name are required.");
            return Page();
        }

        // Enforce unique asset tag.
        var tagInUse = await _db.Equipment
            .AnyAsync(e => e.AssetTag == AssetTag && e.Id != (Id ?? 0));
        if (tagInUse)
        {
            ModelState.AddModelError(nameof(AssetTag), "That asset tag is already in use.");
            return Page();
        }

        if (Id.HasValue)
        {
            var e = await _db.Equipment.FindAsync(Id.Value);
            if (e is null) return RedirectToPage("/Equipment/Index");
            e.AssetTag = AssetTag.Trim();
            e.Name = Name.Trim();
            e.Category = Category.Trim();
            e.Status = Status;
            e.Notes = Notes;
        }
        else
        {
            _db.Equipment.Add(new LoanTracker.Models.Equipment
            {
                AssetTag = AssetTag.Trim(),
                Name = Name.Trim(),
                Category = Category.Trim(),
                Status = Status,
                Notes = Notes
            });
        }

        await _db.SaveChangesAsync();
        return RedirectToPage("/Equipment/Index");
    }
}
