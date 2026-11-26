using System;
using System.Text;
namespace StudentFitnessTracker;

public class LoginMenu
{
    public User? Run()
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine("===== STUDENT FITNESS TRACKER =====");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Register");
            Console.WriteLine("3. Exit");
            Console.Write("\nEnter choice: ");

            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1":
                    User? loggedInUser = Login();

                    if (loggedInUser != null)
                        return loggedInUser;

                    break;

                case "2":
                    Register();
                    break;

                case "3":
                    return null;

                default:
                    Console.WriteLine("Invalid choice.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private User? Login()
    {
        Console.Clear();

        Console.WriteLine("===== LOGIN =====");

        Console.Write("Username: ");
        string username = Console.ReadLine() ?? "";

        Console.Write("Password: ");
        //string password = ReadPassword();
        string password = ReadInput();
            //Console.ReadLine() ?? "";

        User? user = AuthenticationService.Login(username, password);

        if (user != null)
        {
            Console.WriteLine("\nLogin successful!");
            Console.WriteLine($"Welcome, {user.Username}!");
        }
        else
        {
            Console.WriteLine("\nIncorrect username or password.");
        }

        Console.ReadKey();

        return user;
    }

    private void Register()
    {
        Console.Clear();

        Console.WriteLine("===== REGISTER =====");

        Console.Write("Create username: ");
        string username = Console.ReadLine() ?? "";

        Console.Write("Create password: ");
        string password = ReadInput();
            //Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(username))
        {
            Console.WriteLine("\nUsername cannot be empty.");
        }
        else if (password.Length < 8)
        {
            Console.WriteLine("\nPassword must contain at least 8 characters.");
        }
        else
        {
            bool registered = AuthenticationService.Register(username.Trim(), password);

            if (registered)
                Console.WriteLine("\nAccount successfully created!");
            else
                Console.WriteLine("\nUsername already exists.");
        }

        Console.ReadKey();
    }

    private string ReadPassword()
    {
        string password = "";

        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter)
                break;

            if (key.Key == ConsoleKey.Backspace)
            {
                if (password.Length > 0)
                    password = password[..^1];
            }
            else if (!char.IsControl(key.KeyChar))
            {
                password += key.KeyChar;
            }
        }

        return password;
    }
    static string ReadInput()
    {
        StringBuilder input = new StringBuilder();
        ConsoleKeyInfo keyinfo;

        do
        {
            keyinfo = Console.ReadKey(intercept: true);
            if (keyinfo.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input.Remove(input.Length - 1, 1);
                Console.Write("\b \b");
            }

            else if (!char.IsControl(keyinfo.KeyChar))
            {
                input.Append(keyinfo.KeyChar);
                Console.Write("*");
            }
        } while (keyinfo.Key != ConsoleKey.Enter);
        return input.ToString();
    }
}