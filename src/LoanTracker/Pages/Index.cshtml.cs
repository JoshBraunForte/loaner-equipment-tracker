using LoanTracker.Data;
using LoanTracker.Helpers;
using LoanTracker.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LoanTracker.Pages;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public IndexModel(AppDbContext db) => _db = db;

    public UserRole Role { get; private set; }
    public string DisplayName { get; private set; } = "";

    // Employee view
    public List<Loan> MyLoans { get; private set; } = new();
    public List<LoanRequest> MyRequests { get; private set; } = new();

    // Manager view
    public int PendingApprovals { get; private set; }
    public List<Loan> TeamLoans { get; private set; } = new();

    // IT view
    public int QueueCount { get; private set; }
    public int ActiveLoans { get; private set; }
    public int OverdueCount { get; private set; }
    public int AvailableCount { get; private set; }
    public List<Loan> OverdueLoans { get; private set; } = new();

    public async Task OnGetAsync()
    {
        var uid = User.GetUserId();
        DisplayName = User.GetDisplayName();
        Role = Enum.Parse<UserRole>(User.FindFirst(System.Security.Claims.ClaimTypes.Role)!.Value);
        var today = DateTime.Today;

        // Everyone can see their own loans and requests.
        MyLoans = await _db.Loans
            .Include(l => l.Equipment)
            .Where(l => l.BorrowerId == uid && l.ReturnedAt == null)
            .OrderBy(l => l.DueDate)
            .ToListAsync();

        MyRequests = await _db.LoanRequests
            .Where(r => r.RequesterId == uid)
            .OrderByDescending(r => r.CreatedAt)
            .Take(5)
            .ToListAsync();

        if (Role == UserRole.Manager)
        {
            PendingApprovals = await _db.LoanRequests
                .CountAsync(r => r.Status == RequestStatus.Submitted && r.Requester.ManagerId == uid);

            TeamLoans = await _db.Loans
                .Include(l => l.Equipment)
                .Include(l => l.Borrower)
                .Where(l => l.ReturnedAt == null && l.Borrower.ManagerId == uid)
                .OrderBy(l => l.DueDate)
                .ToListAsync();
        }

        if (Role == UserRole.ItStaff)
        {
            QueueCount = await _db.LoanRequests.CountAsync(r => r.Status == RequestStatus.Approved);
            ActiveLoans = await _db.Loans.CountAsync(l => l.ReturnedAt == null);
            OverdueCount = await _db.Loans.CountAsync(l => l.ReturnedAt == null && l.DueDate < today);
            AvailableCount = await _db.Equipment.CountAsync(e => e.Status == EquipmentStatus.Available);

            OverdueLoans = await _db.Loans
                .Include(l => l.Equipment)
                .Include(l => l.Borrower)
                .Where(l => l.ReturnedAt == null && l.DueDate < today)
                .OrderBy(l => l.DueDate)
                .ToListAsync();
        }
    }
}
