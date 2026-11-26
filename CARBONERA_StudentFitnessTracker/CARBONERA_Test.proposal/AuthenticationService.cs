using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text.Json;

namespace StudentFitnessTracker;

public static class AuthenticationService
{
    private static readonly string FilePath = Path.Combine(AppContext.BaseDirectory, "Data", "users.json");

    private static List<User> Users = LoadUsers();

    private const int Iterations = 600000;

    private static List<User> LoadUsers()
    {
        if (!File.Exists(FilePath)) return new List<User>();

        string json = File.ReadAllText(FilePath);

        return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
    }

    private static void SaveUsers()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);

        string json = JsonSerializer.Serialize(Users,new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(FilePath, json);
    }

    public static bool Register(string username, string password)
    {

        if (Users.Any(user => user.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        byte[] salt = RandomNumberGenerator.GetBytes(16);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password,salt,Iterations,HashAlgorithmName.SHA256,32);

        User newUser = new User
        {
            UserID = Guid.NewGuid().ToString(),
            Username = username,
            PasswordSalt = Convert.ToBase64String(salt),
            PasswordHash = Convert.ToBase64String(hash),
            Role = "Member"
        };

        Users.Add(newUser);

        SaveUsers();

        return true;
    }

    public static User? Login(string username, string password)
    {
        User? user = Users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        if (user == null)
            return null;

        byte[] salt = Convert.FromBase64String(user.PasswordSalt);

        byte[] storedHash = Convert.FromBase64String(user.PasswordHash);

        byte[] enteredHash = Rfc2898DeriveBytes.Pbkdf2(password,salt,Iterations,HashAlgorithmName.SHA256,32);

        bool passwordCorrect = CryptographicOperations.FixedTimeEquals(storedHash, enteredHash);

        return passwordCorrect ? user : null;
    }


    private static void RegisterAdminAccount(string username, string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password,salt,Iterations,HashAlgorithmName.SHA256,32);

        User admin = new User
        {
            UserID = Guid.NewGuid().ToString(),
            Username = username,
            PasswordSalt = Convert.ToBase64String(salt),
            PasswordHash = Convert.ToBase64String(hash),
            Role = "Admin"
        };

        Users.Add(admin);

        SaveUsers();
    }
    public static void CreateAdmin()
    {
        bool adminExists = Users.Any(user => user.Role == "Admin");

        if (adminExists) return;

        RegisterAdminAccount("Adomin", "AdminCode123");
    }

    public static List<User> GetAllMembers()
    {
        return Users
            .Where(user => user.Role == "Member")
            .ToList();
    }

    public static bool DeleteMember(string userID)
    {
        User? member = Users.FirstOrDefault(
            user => user.UserID == userID &&
                    user.Role == "Member");

        if (member == null)
            return false;

        Users.Remove(member);
        SaveUsers();

        return true;
    }
}