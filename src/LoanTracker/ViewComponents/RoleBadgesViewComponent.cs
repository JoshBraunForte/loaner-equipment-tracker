using System.Security.Claims;
using LoanTracker.Data;
using LoanTracker.Helpers;
using LoanTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoanTracker.ViewComponents;

// Renders a small count badge used as an in-app notification indicator in the nav bar.
public class RoleBadgesViewComponent : ViewComponent
{
    private readonly AppDbContext _db;
    public RoleBadgesViewComponent(AppDbContext db) => _db = db;

    public async Task<IViewComponentResult> InvokeAsync(string badge)
    {
        var principal = (ClaimsPrincipal)User;
        if (principal.Identity?.IsAuthenticated != true)
            return Content(string.Empty);

        var today = DateTime.Today;
        int count = badge switch
        {
            "approvals" => await _db.LoanRequests.CountAsync(r =>
                r.Status == RequestStatus.Submitted && r.Requester.ManagerId == principal.GetUserId()),
            "queue" => await _db.LoanRequests.CountAsync(r => r.Status == RequestStatus.Approved),
            "overdue" => await _db.Loans.CountAsync(l => l.ReturnedAt == null && l.DueDate < today),
            _ => 0
        };

        if (count == 0)
            return Content(string.Empty);

        var color = badge == "overdue" ? "bg-danger" : "bg-warning text-dark";
        return new Microsoft.AspNetCore.Mvc.ViewComponents.HtmlContentViewComponentResult(
            new Microsoft.AspNetCore.Html.HtmlString($"<span class=\"badge {color}\">{count}</span>"));
    }
}
