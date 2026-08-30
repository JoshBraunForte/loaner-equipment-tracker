namespace LoanTracker.Models;

public class LoanRequest
{
    public int Id { get; set; }

    public int RequesterId { get; set; }
    public AppUser Requester { get; set; } = null!;

    public string ItemDescription { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Justification { get; set; }

    public RequestStatus Status { get; set; } = RequestStatus.Submitted;
    public DateTime CreatedAt { get; set; }

    // Manager decision (approve / deny)
    public int? DecidedById { get; set; }
    public AppUser? DecidedBy { get; set; }
    public DateTime? DecidedAt { get; set; }
    public string? DecisionNote { get; set; }

    // Fulfillment: one-to-one link to the loan IT creates
    public Loan? Loan { get; set; }
}
