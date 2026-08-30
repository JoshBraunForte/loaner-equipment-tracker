namespace LoanTracker.Models;

public class Loan
{
    public int Id { get; set; }

    public int EquipmentId { get; set; }
    public Equipment Equipment { get; set; } = null!;

    public int BorrowerId { get; set; }
    public AppUser Borrower { get; set; } = null!;

    // Optional link back to the request this loan fulfilled (walk-up loans have none).
    public int? RequestId { get; set; }
    public LoanRequest? Request { get; set; }

    public DateTime CheckedOutAt { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnedAt { get; set; }

    // Convenience flags for views (not mapped; do not use inside EF queries).
    public bool IsReturned => ReturnedAt.HasValue;
    public bool IsOverdue => !ReturnedAt.HasValue && DueDate.Date < DateTime.Today;
}
