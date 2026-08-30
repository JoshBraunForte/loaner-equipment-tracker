namespace LoanTracker.Models;

public enum UserRole
{
    Employee,
    Manager,
    ItStaff
}

public enum EquipmentStatus
{
    Available,
    OnLoan,
    Retired
}

public enum RequestStatus
{
    Submitted,   // waiting on the requester's manager
    Approved,    // manager approved; waiting for IT to fulfill
    Denied,      // manager denied
    Fulfilled,   // IT assigned an item and created a loan
    Cancelled    // requester withdrew
}
