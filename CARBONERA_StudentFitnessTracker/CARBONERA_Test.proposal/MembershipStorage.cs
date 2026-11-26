
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace StudentFitnessTracker;

public static class MembershipStorage
{
    private static readonly string FilePath = Path.Combine(AppContext.BaseDirectory, "Data", "memberships.json");

    private static List<Membership> Memberships = LoadMemberships();

    private static List<Membership> LoadMemberships()
    {
        if (!File.Exists(FilePath))
            return new List<Membership>();

        string json = File.ReadAllText(FilePath);

        return JsonSerializer.Deserialize<List<Membership>>(json) ?? new List<Membership>();
    }

    private static void SaveMemberships()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);

        string json = JsonSerializer.Serialize(Memberships, new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(FilePath, json);
    }
    public static Membership? GetMembership(string userID)
    {
        Membership? membership = Memberships.FirstOrDefault(
            m => m.UserID == userID);

        if (membership == null)
            return null;

        string currentStatus = membership.GetStatus();

        if (membership.Status != currentStatus)
        {
            membership.Status = currentStatus;
            SaveMemberships();
        }

        return membership;
    }
    public static void AddMembership(Membership membership)
    {
        Memberships.RemoveAll(item => item.UserID == membership.UserID);

        Memberships.Add(membership);
        SaveMemberships();
    }

    public static void DeleteMembership(string userID)
    {
        Memberships.RemoveAll(membership => membership.UserID == userID);

        SaveMemberships();
    }
}
