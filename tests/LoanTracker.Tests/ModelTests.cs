using LoanTracker.Models;
using Xunit;

namespace LoanTracker.Tests;

public class ModelTests
{
    [Fact]
    public void Loan_is_overdue_when_past_due_and_not_returned()
    {
        var loan = new Loan { DueDate = DateTime.Today.AddDays(-1), ReturnedAt = null };
        Assert.True(loan.IsOverdue);
    }

    [Fact]
    public void Loan_not_overdue_when_due_in_future()
    {
        var loan = new Loan { DueDate = DateTime.Today.AddDays(1), ReturnedAt = null };
        Assert.False(loan.IsOverdue);
    }

    [Fact]
    public void Returned_loan_is_never_overdue()
    {
        var loan = new Loan { DueDate = DateTime.Today.AddDays(-10), ReturnedAt = DateTime.Today };
        Assert.False(loan.IsOverdue);
        Assert.True(loan.IsReturned);
    }

    [Fact]
    public void Loan_due_today_is_not_overdue()
    {
        var loan = new Loan { DueDate = DateTime.Today, ReturnedAt = null };
        Assert.False(loan.IsOverdue);
    }
}
