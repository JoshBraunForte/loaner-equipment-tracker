using LoanTracker.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LoanTracker.Tests;

/// <summary>
/// Exercises the request -> approval -> fulfillment -> check-in lifecycle by calling
/// the real Razor Page handlers with a signed-in user, asserting persisted state.
/// </summary>
public class WorkflowTests
{
    private static (int reqId, int managerId) SubmittedDockingRequest(TestDb t)
    {
        var req = t.Db.LoanRequests.Include(r => r.Requester)
            .Single(r => r.Status == RequestStatus.Submitted && r.Requester.UserName == "emp1");
        return (req.Id, req.Requester.ManagerId!.Value);
    }

    [Fact]
    public async Task Manager_can_approve_own_report_request()
    {
        using var t = new TestDb();
        var (reqId, managerId) = SubmittedDockingRequest(t);

        var page = new LoanTracker.Pages.Approvals.IndexModel(t.Db);
        TestDb.SignIn(page, managerId, UserRole.Manager);

        await page.OnPostDecideAsync(reqId, "approve", "ok");

        var req = t.Db.LoanRequests.Single(r => r.Id == reqId);
        Assert.Equal(RequestStatus.Approved, req.Status);
        Assert.Equal(managerId, req.DecidedById);
        Assert.NotNull(req.DecidedAt);
    }

    [Fact]
    public async Task Manager_can_deny_own_report_request()
    {
        using var t = new TestDb();
        var (reqId, managerId) = SubmittedDockingRequest(t);

        var page = new LoanTracker.Pages.Approvals.IndexModel(t.Db);
        TestDb.SignIn(page, managerId, UserRole.Manager);

        await page.OnPostDecideAsync(reqId, "deny", "not now");

        Assert.Equal(RequestStatus.Denied, t.Db.LoanRequests.Single(r => r.Id == reqId).Status);
    }

    [Fact]
    public async Task Other_manager_cannot_decide_someone_elses_report()
    {
        using var t = new TestDb();
        var (reqId, _) = SubmittedDockingRequest(t);
        var otherManager = t.Db.Users.Single(u => u.UserName == "mgr2").Id;

        var page = new LoanTracker.Pages.Approvals.IndexModel(t.Db);
        TestDb.SignIn(page, otherManager, UserRole.Manager);

        await page.OnPostDecideAsync(reqId, "approve", "sneaky");

        // Guard holds: still awaiting the correct manager.
        Assert.Equal(RequestStatus.Submitted, t.Db.LoanRequests.Single(r => r.Id == reqId).Status);
    }

    [Fact]
    public async Task It_fulfilling_an_approved_request_creates_a_loan_and_marks_equipment_on_loan()
    {
        using var t = new TestDb();
        var approved = t.Db.LoanRequests.Single(r => r.Status == RequestStatus.Approved); // Evan's iPad
        var ipad = t.Db.Equipment.Single(e => e.AssetTag == "TB-001");
        var it = t.Db.Users.Single(u => u.UserName == "it1").Id;

        var page = new LoanTracker.Pages.Queue.IndexModel(t.Db);
        TestDb.SignIn(page, it, UserRole.ItStaff);

        var due = DateTime.Today.AddDays(14);
        await page.OnPostFulfillAsync(approved.Id, ipad.Id, due);

        var req = t.Db.LoanRequests.Single(r => r.Id == approved.Id);
        Assert.Equal(RequestStatus.Fulfilled, req.Status);

        Assert.Equal(EquipmentStatus.OnLoan, t.Db.Equipment.Single(e => e.Id == ipad.Id).Status);

        var loan = t.Db.Loans.Single(l => l.RequestId == approved.Id);
        Assert.Equal(approved.RequesterId, loan.BorrowerId);
        Assert.Equal(due.Date, loan.DueDate.Date);
        Assert.Null(loan.ReturnedAt);
    }

    [Fact]
    public async Task It_cannot_fulfill_with_equipment_that_is_not_available()
    {
        using var t = new TestDb();
        var approved = t.Db.LoanRequests.Single(r => r.Status == RequestStatus.Approved);
        var onLoanItem = t.Db.Equipment.Single(e => e.AssetTag == "MN-001"); // already on loan to Evan
        var it = t.Db.Users.Single(u => u.UserName == "it1").Id;

        var page = new LoanTracker.Pages.Queue.IndexModel(t.Db);
        TestDb.SignIn(page, it, UserRole.ItStaff);

        await page.OnPostFulfillAsync(approved.Id, onLoanItem.Id, DateTime.Today.AddDays(7));

        // Nothing changed: request still Approved, no new loan for it.
        Assert.Equal(RequestStatus.Approved, t.Db.LoanRequests.Single(r => r.Id == approved.Id).Status);
        Assert.False(t.Db.Loans.Any(l => l.RequestId == approved.Id));
    }

    [Fact]
    public async Task Checking_in_a_loan_returns_it_and_frees_the_equipment()
    {
        using var t = new TestDb();
        var loan = t.Db.Loans.Include(l => l.Equipment).First(l => l.Equipment.AssetTag == "MN-001");
        var it = t.Db.Users.Single(u => u.UserName == "it1").Id;

        var page = new LoanTracker.Pages.Loans.IndexModel(t.Db);
        TestDb.SignIn(page, it, UserRole.ItStaff);

        await page.OnPostCheckInAsync(loan.Id);

        var reloaded = t.Db.Loans.Include(l => l.Equipment).Single(l => l.Id == loan.Id);
        Assert.NotNull(reloaded.ReturnedAt);
        Assert.Equal(EquipmentStatus.Available, reloaded.Equipment.Status);
    }

    [Fact]
    public async Task Employee_can_cancel_own_submitted_request()
    {
        using var t = new TestDb();
        var (reqId, _) = SubmittedDockingRequest(t);
        var emma = t.Db.Users.Single(u => u.UserName == "emp1").Id;

        var page = new LoanTracker.Pages.Requests.IndexModel(t.Db);
        TestDb.SignIn(page, emma, UserRole.Employee);

        await page.OnPostCancelAsync(reqId);

        Assert.Equal(RequestStatus.Cancelled, t.Db.LoanRequests.Single(r => r.Id == reqId).Status);
    }

    [Fact]
    public async Task Employee_cannot_cancel_another_users_request()
    {
        using var t = new TestDb();
        var (reqId, _) = SubmittedDockingRequest(t); // belongs to emp1
        var evan = t.Db.Users.Single(u => u.UserName == "emp2").Id;

        var page = new LoanTracker.Pages.Requests.IndexModel(t.Db);
        TestDb.SignIn(page, evan, UserRole.Employee);

        await page.OnPostCancelAsync(reqId);

        Assert.Equal(RequestStatus.Submitted, t.Db.LoanRequests.Single(r => r.Id == reqId).Status);
    }

    [Fact]
    public async Task New_request_from_employee_is_submitted_and_routes_to_their_manager()
    {
        using var t = new TestDb();
        var emma = t.Db.Users.Single(u => u.UserName == "emp1");

        var page = new LoanTracker.Pages.Requests.CreateModel(t.Db)
        {
            ItemDescription = "Second monitor",
            Category = "Monitor"
        };
        TestDb.SignIn(page, emma.Id, UserRole.Employee);

        await page.OnPostAsync();

        var created = t.Db.LoanRequests.Where(r => r.RequesterId == emma.Id)
            .OrderByDescending(r => r.Id).First();
        Assert.Equal("Second monitor", created.ItemDescription);
        Assert.Equal(RequestStatus.Submitted, created.Status);
        Assert.Equal(emma.ManagerId, t.Db.Users.Single(u => u.Id == created.RequesterId).ManagerId);
    }
}
