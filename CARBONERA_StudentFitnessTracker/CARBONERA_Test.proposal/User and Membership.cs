using System;

namespace StudentFitnessTracker;

public class User
{
    public string UserID { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;

    public string Role { get; set; } = "Member";
}

public class Membership
{
    public string UserID { get; set; } = string.Empty;
    public string MembershipType { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }
    public DateTime ExpirationDate { get; set; }

    public string Status { get; set; } = "Active";
    public string GetStatus()
    {
        return DateTime.Now.Date >= ExpirationDate.Date ? "Expired" : "Active";
    }
}
