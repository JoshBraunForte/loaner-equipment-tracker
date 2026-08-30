namespace LoanTracker.Models;

public class Equipment
{
    public int Id { get; set; }
    public string AssetTag { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public EquipmentStatus Status { get; set; } = EquipmentStatus.Available;
    public string? Notes { get; set; }

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
