
using System;
using System.Collections.Generic;
using System.Linq;

namespace StudentFitnessTracker;

public class AdminMenu
{
    public void Run()
    {
        bool logout = false;

        while (!logout)
        {
            Console.Clear();

            Console.WriteLine("===== GYM MANAGEMENT SYSTEM =====");
            Console.WriteLine("===== ADMIN MENU =====");
            Console.WriteLine("1. View All Accounts");
            Console.WriteLine("2. View Member Fitness Progress");
            Console.WriteLine("3. View Member Workout History");
            Console.WriteLine("4. View Membership Information");
            Console.WriteLine("5. Register/Renew Membership");
            Console.WriteLine("6. Delete Member Account");
            Console.WriteLine("7. Logout");

            int choice = ReadInt("\nEnter choice: ", 1, 7);

            switch (choice)
            {
                case 1:
                    ViewAllAccounts();
                    break;

                case 2:
                    ViewMemberProgress();
                    break;

                case 3:
                    ViewMemberHistory();
                    break;

                case 4:
                    ViewMembershipInformation();
                    break;

                case 5:
                    RegisterMembership();
                    break;

                case 6:
                    DeleteMemberAccount();
                    break;

                case 7:
                    logout = true;
                    Console.WriteLine("\nLogging out...");
                    break;
            }

            if (!logout)
            {
                Console.WriteLine("\nPress Enter to return to the Admin Menu...");
                Console.ReadLine();
            }
        }
    }

    private void ViewAllAccounts()
    {
        Console.Clear();
        Console.WriteLine("===== REGISTERED ACCOUNTS =====");

        List<User> members = AuthenticationService.GetAllMembers();

        if (members.Count == 0)
        {
            Console.WriteLine("\nNo member accounts found.");
            return;
        }

        foreach (User member in members)
        {
            Console.WriteLine("\n------------------------------");
            Console.WriteLine($"Username: {member.Username}");
            Console.WriteLine($"User ID: {member.UserID}");
            Console.WriteLine($"Role: {member.Role}");
        }
    }

    private static User? SelectMember()
    {
        Console.Clear();
        List<User> members = AuthenticationService.GetAllMembers();

        if (members.Count == 0)
        {
            Console.WriteLine("\nNo member accounts found.");
            return null;
        }

        Console.WriteLine("\n===== MEMBER ACCOUNTS =====");

        for (int i = 0; i < members.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {members[i].Username}");
        }

        int choice = ReadInt("\nSelect a member (0 to cancel): ", 0, members.Count);

        if (choice == 0)
            return null;

        return members[choice - 1];
    }

    private void ViewMemberProgress()
    {
        Console.Clear();
        Console.WriteLine("===== MEMBER FITNESS PROGRESS =====");

        User? member = SelectMember();

        if (member == null)
            return;

        List<WorkoutRecord> history =
            WorkoutStorage.GetUserWorkouts(member.UserID);

        Console.WriteLine($"\nMember: {member.Username}");

        if (history.Count == 0)
        {
            Console.WriteLine("No workout records available.");
            return;
        }

        int totalWorkouts = history.Count;

        int totalExercises = history.Sum(
            workout => workout.Exercises.Count);

        double totalVolume = history.Sum(
            workout => workout.TotalVolume);

        int pushCount = history.Count(
            workout => workout.Category == "Push");

        int pullCount = history.Count(
            workout => workout.Category == "Pull");

        int legsCount = history.Count(
            workout => workout.Category == "Legs");

        Console.WriteLine("\n--- GENERAL STATISTICS ---");
        Console.WriteLine($"Total Workouts: {totalWorkouts}");
        Console.WriteLine($"Total Exercises: {totalExercises}");
        Console.WriteLine($"Total Volume: {totalVolume:N2} kg");

        Console.WriteLine("\n--- WORKOUT DISTRIBUTION ---");
        Console.WriteLine($"Push: {pushCount}");
        Console.WriteLine($"Pull: {pullCount}");
        Console.WriteLine($"Legs: {legsCount}");
    }

    private void ViewMemberHistory()
    {
        Console.Clear();
        Console.WriteLine("===== MEMBER WORKOUT HISTORY =====");

        User? member = SelectMember();

        if (member == null)
            return;

        List<WorkoutRecord> history =
            WorkoutStorage.GetUserWorkouts(member.UserID);

        Console.WriteLine($"\nWorkout history for {member.Username}");

        if (history.Count == 0)
        {
            Console.WriteLine("\nNo workout records found.");
            return;
        }

        foreach (WorkoutRecord workout in history)
        {
            var completedExercises = workout.Exercises
                .Where(exercise => exercise.Completed)
                .ToList();

            if (completedExercises.Count == 0)
            {
                Console.WriteLine("No completed exercises in this workout.");
                continue;
            }

            foreach (ExerciseRecord exercise in completedExercises)
            {
                Console.WriteLine($"\n[{exercise.Phase}] {exercise.ExerciseName}");

                if (exercise.Phase.Equals(
                    "Main Exercise",
                    StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Sets: {exercise.Sets}");
                    Console.WriteLine($"Repetitions: {exercise.Repetitions}");
                    Console.WriteLine($"Weight: {exercise.Weight:N2} kg");
                    Console.WriteLine($"Volume: {exercise.TotalVolume:N2} kg");
                }
                else
                {
                    Console.WriteLine("Status: Completed");
                }
            }
        }
    }

    private void ViewMembershipInformation()
    {
        Console.Clear();
        Console.WriteLine("===== MEMBERSHIP INFORMATION =====");

        User? member = SelectMember();

        if (member == null)
            return;

        Membership? membership = MembershipStorage.GetMembership(member.UserID);

        Console.WriteLine($"\nMember: {member.Username}");

        if (membership == null)
        {
            Console.WriteLine("No membership information found.");
            return;
        }

        Console.WriteLine($"Membership Type: {membership.MembershipType}");
        Console.WriteLine($"Start Date: {membership.StartDate:d}");
        Console.WriteLine($"Expiration Date: {membership.ExpirationDate:d}");
        Console.WriteLine($"Status: {membership.Status}");
    }

    private static void RegisterMembership()
    {
        Console.Clear();
        List<User> members = AuthenticationService.GetAllMembers();

        if (members.Count == 0)
        {
            Console.WriteLine("No member accounts found.");
            return;
        }

        Console.WriteLine("\n--- Membership Registration ---");

        User? member = SelectMember();

        if (member == null)
            return;

        Membership? existing = MembershipStorage.GetMembership(member.UserID);

        if (existing != null)
        {
            Console.WriteLine("\nThis member already has a membership.");
            Console.WriteLine($"Expiry Date: {existing.ExpirationDate:d}");

            Console.Write("Renew membership? (Y/N): ");
            string choice = Console.ReadLine() ?? "";

            if (choice.ToUpper() != "Y")
                return;
        }

        DateTime startDate = DateTime.Today;
        DateTime expirationDate = startDate.AddMonths(3);

        Membership membership = new Membership
        {
            UserID = member.UserID,
            MembershipType = "Standard",
            StartDate = startDate,
            ExpirationDate = expirationDate,
            Status = "Active"
        };

        MembershipStorage.AddMembership(membership);

        Console.WriteLine("\nMembership successfully registered!");
        Console.WriteLine($"Member: {member.Username}");
        Console.WriteLine($"Type: Standard");
        Console.WriteLine($"Start Date: {startDate:d}");
        Console.WriteLine($"Expiration Date: {expirationDate:d}");
    }
    private void DeleteMemberAccount()
    {
        Console.Clear();
        Console.WriteLine("===== DELETE MEMBER ACCOUNT =====");

        User? member = SelectMember();

        if (member == null)
            return;

        Console.WriteLine($"\nSelected Account: {member.Username}");
        Console.WriteLine("WARNING: This will delete the account and its associated records.");

        Console.Write("\nConfirm deletion? (Y/N): ");
        string confirmation = Console.ReadLine() ?? "";

        if (!confirmation.Equals("Y", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("\nDeletion cancelled.");
            return;
        }

        bool deleted = AuthenticationService.DeleteMember(member.UserID);

        if (deleted)
        {
            WorkoutStorage.DeleteUserWorkouts(member.UserID);
            MembershipStorage.DeleteMembership(member.UserID);
            WorkoutStorage.ProfileStorage.DeleteProfile(member.UserID);

            Console.WriteLine("\nAccount and associated records successfully deleted.");
        }
        else
        {
            Console.WriteLine("\nUnable to delete account.");
        }
    }

    public static int ReadInt(string message, int min, int max)
    {
        while (true)
        {
            Console.Write(message);

            if (int.TryParse(Console.ReadLine(), out int value) && value >= min && value <= max)
            {
                return value;
            }

            Console.WriteLine($"Please enter a number between {min} and {max}.");
        }
    }
}
