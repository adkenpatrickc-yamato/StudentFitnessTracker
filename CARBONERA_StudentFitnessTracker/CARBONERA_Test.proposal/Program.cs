using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using static StudentFitnessTracker.WorkoutStorage;

namespace StudentFitnessTracker;

internal class Program
{
    static void Main(string[] args)
    {
        AuthenticationService.CreateAdmin();

        LoginMenu loginMenu = new LoginMenu();

        while (true)
        {

            User? currentUser = loginMenu.Run();

            if (currentUser == null)
            {
                Console.WriteLine("Exiting application...");
                break;
            }

            bool logout = false;

            if (currentUser.Role == "Admin")
            {
                AdminMenu adminMenu = new AdminMenu();
                adminMenu.Run();
            }
            else if (currentUser.Role != "Admin") { 
            MemberProfileMenu.EnsureProfile(currentUser);
            {
                // Main menu
                while (!logout)
                {
                    Console.Clear();

                    Console.WriteLine($"===== FITNESS TRACKER =====");
                    Console.WriteLine($"Logged in as: {currentUser.Username}");
                    Console.WriteLine("\n1. View Exercise Library");
                    Console.WriteLine("2. Start a Workout");
                    Console.WriteLine("3. View Workout History");
                    Console.WriteLine("4. View Progress Summary");
                    Console.WriteLine("5. Exit Application");
                    Console.WriteLine("6. Logout");

                    int choice = ReadIntInRange("\nEnter choice: ", 1, 6);

                    switch (choice)
                    {
                        case 1:
                            LibraryMenu library = new LibraryMenu();
                            library.ViewEL();
                            library.Halt();
                            break;

                        case 2:
                            WorkoutAction workout = new WorkoutAction();
                            workout.BeginWorkout(currentUser);
                            workout.Halt();
                            break;

                        case 3:
                            WorkoutHistoryMenu history = new WorkoutHistoryMenu();
                            history.ViewWH(currentUser);
                            history.Halt();
                            break;

                        case 4:
                            ProgressMenu progress = new ProgressMenu();
                            progress.ViewProg(currentUser);
                            progress.Halt();
                            break;

                        case 5:
                            Console.WriteLine("Exiting application...");
                            return;

                        case 6:
                            logout = true;
                            Console.WriteLine("Logging out...");
                            break;
                    }
                }
            } 
            }
        }
    }

    public static int ReadIntInRange(string message, int min, int max)
    {
        int value;

        while (true)
        {
            Console.Write(message);

            if (int.TryParse(Console.ReadLine(), out value)
                && value >= min && value <= max)
            {
                return value;
            }

            Console.WriteLine($"Please enter a number between {min} and {max}.");
        }
    }
    public static Random Randomizer = new Random();
}
public interface IMenuNavi
{
    void Halt();

    public static int ReadIntInRange(string message, int min, int max)
    {
        int value;

        while (true)
        {
            Console.Write(message);

            if (int.TryParse(Console.ReadLine(), out value)
                && value >= min && value <= max)
            {
                return value;
            }

            Console.WriteLine($"Please enter a number between {min} and {max}.");
        }
    }
}

public abstract class MenuNavi : IMenuNavi
{
    public void Halt()
    {
        Console.Write("\nPress Enter to return to the main menu...");
        Console.ReadLine();
    }
}

public class LibraryMenu : MenuNavi
{
    public void ViewEL()
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine("[]==============[] EXERCISE LIBRARY []==============[]");
            Console.WriteLine("1. View All Exercises");
            Console.WriteLine("2. Search Exercise");
            Console.WriteLine("3. Return");

            int choice = IMenuNavi.ReadIntInRange("\nChoose an option: ", 1, 3);

            switch (choice)
            {
                case 1:
                    Console.Clear();
                    DisplayExerciseTable(ExerciseLibrary.All.ToList());
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                    break;

                case 2:
                    Console.Clear();
                    SearchExercise();
                    break;

                case 3:
                    return;
            }
        }
    }


    public static void DisplayExerciseTable(List<Exercise> exercises)
    {
        if (exercises.Count == 0)
        {
            Console.WriteLine("No exercises found.");
            return;
        }

        int[] widths = { 25, 16, 16, 28, 40 };

        Console.WriteLine(new string('-', widths.Sum() + 12));

        PrintTableRow(
            new[] { "NAME", "PHASE", "CATEGORY", "TARGET MUSCLES", "FITNESS GOALS" },
            widths);

        Console.WriteLine(new string('-', widths.Sum() + 12));

        foreach (Exercise exercise in exercises)
        {
            string goals = exercise.GoalTypes == null ||
                           exercise.GoalTypes.Count == 0
                ? "All"
                : string.Join(", ", exercise.GoalTypes);

            PrintTableRow(
                new[]
                {
                exercise.Name,
                exercise.Phase,
                exercise.Category,
                exercise.TargetMuscles,
                goals
                },
                widths);
        }

        Console.WriteLine(new string('-', widths.Sum() + 12));
    }

    private static void PrintTableRow(string[] values, int[] widths)
    {
        List<List<string>> wrappedColumns = new();

        for (int i = 0; i < values.Length; i++)
        {
            wrappedColumns.Add(WrapText(values[i], widths[i]));
        }

        int rowHeight = wrappedColumns.Max(column => column.Count);

        for (int line = 0; line < rowHeight; line++)
        {
            for (int column = 0; column < widths.Length; column++)
            {
                string text = line < wrappedColumns[column].Count
                    ? wrappedColumns[column][line]
                    : "";

                Console.Write(text.PadRight(widths[column]));

                if (column < widths.Length - 1)
                    Console.Write(" | ");
            }

            Console.WriteLine();
        }
    }

    private static List<string> WrapText(string text, int width)
    {
        List<string> lines = new();
        string currentLine = "";

        foreach (string word in text.Split(' '))
        {
            if (currentLine.Length == 0)
            {
                currentLine = word;
            }
            else if (currentLine.Length + word.Length + 1 <= width)
            {
                currentLine += " " + word;
            }
            else
            {
                lines.Add(currentLine);
                currentLine = word;
            }
        }

        if (currentLine.Length > 0)
            lines.Add(currentLine);

        return lines;
    }
    private void SearchExercise()
    {
        Console.Clear();

        Console.WriteLine("[]==============[] SEARCH EXERCISE []==============[]");

        Console.Write("Enter exercise name: ");
        string keyword = Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(keyword))
        {
            Console.WriteLine("Search cannot be empty.");
        }
        else
        {
            List<Exercise> results = ExerciseLibrary.Search(keyword);

            Console.WriteLine("\nSearch Results:");
            DisplayExerciseTable(results);
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }
}

public class WorkoutAction : MenuNavi
{
    public static Random Randomizer = new Random();


    public void BeginWorkout(User currentUser)
    {
        Console.Clear();
        Console.WriteLine("[]================[] START WORKOUT []===============[]");
        Console.WriteLine("A guided workout includes a warm-up, main exercise, and cool-down.");

        Console.WriteLine("\nChoose your workout method:");
        Console.WriteLine("1. Recommended Workout");
        Console.WriteLine("2. Manual Workout Selection");
        Console.WriteLine("3. Cancel");

        int method = ReadIntInRange("\nChoose an option: ", 1, 3);

        if (method == 3)
        {
            Console.WriteLine("Workout cancelled.");
            return;
        }

        string category;
        List<Exercise> mainExercises;

        // RECOMMENDED WORKOUT
        if (method == 1)
        {
            MemberProfile? profile =
            ProfileStorage.GetProfile(currentUser.UserID);

            if (profile == null)
            {
                Console.WriteLine("Fitness profile not found.");
                Console.WriteLine("Please register your profile again.");
                return;
            }

            Console.WriteLine($"\nYour fitness goal: {profile.FitnessGoal}");
            Console.WriteLine("Generating recommended exercises...");

            category = "Recommended - " + profile.FitnessGoal;

            mainExercises = SelectRandomExercises(
                ExerciseLibrary.GetByGoal(profile.FitnessGoal), 3);
        }
        // MANUAL WORKOUT
        else
        {
            Console.WriteLine("\nChoose your main workout:");
            Console.WriteLine("1. Push");
            Console.WriteLine("2. Pull");
            Console.WriteLine("3. Legs");
            Console.WriteLine("4. Randomized workout");
            Console.WriteLine("5. Cancel");

            int choice = ReadIntInRange("\nChoose workout type: ", 1, 5);

            if (choice == 5)
            {
                Console.WriteLine("Workout cancelled.");
                return;
            }

            category = choice switch
            {
                1 => "Push",
                2 => "Pull",
                3 => "Legs",
                _ => new[] { "Push", "Pull", "Legs" }[Randomizer.Next(3)]
            };

            mainExercises = SelectRandomExercises(
                ExerciseLibrary.GetByCategory(category), 3);
        }

        if (mainExercises.Count == 0)
        {
            Console.WriteLine($"\nNo main exercises are available for {category}.");
            Console.WriteLine("Please check Data/exercises.json and try again.");
            return;
        }

        // SELECT WARM-UP AND COOL-DOWN
        List<Exercise> warmUps = SelectRandomExercises(
            ExerciseLibrary.GetByPhase("Warm-up"), 2);

        List<Exercise> coolDowns = SelectRandomExercises(
            ExerciseLibrary.GetByPhase("Cool-down"), 2);

        WorkoutRecord workout = new WorkoutRecord
        {
            UserID = currentUser.UserID,
            Category = category,
            WorkoutDate = DateTime.Now
        };

        Console.WriteLine($"\nMain workout selected: {category}");

        if (method == 2)
            Console.WriteLine("You selected a manual workout.");

        RunPreparationPhase("WARM-UP", "Warm-up", warmUps, workout);

        Console.WriteLine("\n[]================[] MAIN EXERCISE []===============[]");
        Console.WriteLine($"Workout category: {category}");
        Console.WriteLine("Use 0 for weight if the exercise does not use added weight.");

        foreach (Exercise exercise in mainExercises)
        {
            Console.WriteLine($"\nExercise: {exercise.Name} - {exercise.TargetMuscles}");

            int sets = ReadIntInRange("Sets completed: ", 1, 1000);
            int reps = ReadIntInRange("Repetitions performed (per set): ", 1, 1000);
            double kg = ReadDoubleInRange(
                "Weight in kg (0 if bodyweight/no equipment): ", 0, 1000);

            workout.Exercises.Add(new ExerciseRecord
            {
                ExerciseName = exercise.Name,
                Phase = exercise.Phase,
                Category = exercise.Category,
                Sets = sets,
                Repetitions = reps,
                Weight = kg,
                Completed = true
            });
        }

        RunPreparationPhase("COOL-DOWN", "Cool-down", coolDowns, workout);

        WorkoutStorage.AddWorkout(workout);

        Console.WriteLine("\nWorkout successfully recorded!");
        Console.WriteLine($"Exercises included: {workout.Exercises.Count}");
        Console.WriteLine($"Completed exercises: {workout.Exercises.Count(e => e.Completed)}");
        Console.WriteLine($"Total external weight volume: {workout.TotalVolume:N2} kg");
    }


    private static void RunPreparationPhase(
        string heading, string phase, List<Exercise> exercises, WorkoutRecord workout)
    {
        Console.WriteLine($"\n[]================[] {heading} []===============[]");

        if (exercises.Count == 0)
        {
            Console.WriteLine($"No {phase.ToLowerInvariant()} exercises are currently available.");
            return;
        }

        foreach (Exercise exercise in exercises)
        {
            Console.WriteLine($"\nExercise: {exercise.Name} - {exercise.TargetMuscles}");
            bool completed = ReadYesNo("Did you complete this exercise? (Y/N): ");

            workout.Exercises.Add(new ExerciseRecord
            {
                ExerciseName = exercise.Name,
                Phase = exercise.Phase,
                Category = exercise.Category,
                Sets = 0,
                Repetitions = 0,
                Weight = 0,
                Completed = completed
            });
        }
    }

    private static List<Exercise> SelectRandomExercises(List<Exercise> source, int count)
    {
        return source.OrderBy(_ => Randomizer.Next()).Take(count).ToList();
    }

    private static int ReadIntInRange(string message, int min, int max)
    {
        while (true)
        {
            Console.Write(message);
            if (int.TryParse(Console.ReadLine(), out int value) && value >= min && value <= max)
                return value;
            Console.WriteLine($"Please enter a whole number between {min} and {max}.");
        }
    }

    private static double ReadDoubleInRange(string message, double min, double max)
    {
        while (true)
        {
            Console.Write(message);
            if (double.TryParse(Console.ReadLine(), NumberStyles.Float,
                    CultureInfo.InvariantCulture, out double value)
                && value >= min && value <= max)
                return value;
            Console.WriteLine($"Please enter a number between {min} and {max}.");
        }
    }

    private static bool ReadYesNo(string message)
    {
        while (true)
        {
            Console.Write(message);
            string answer = (Console.ReadLine() ?? "").Trim();
            if (answer.Equals("Y", StringComparison.OrdinalIgnoreCase)) return true;
            if (answer.Equals("N", StringComparison.OrdinalIgnoreCase)) return false;
            Console.WriteLine("Please enter Y or N.");
        }
    }
}

public class WorkoutHistoryMenu : MenuNavi
{
    public void ViewWH(User currentUser)
    {
        Console.Clear();

        Console.WriteLine("===== WORKOUT HISTORY =====");

        List<WorkoutRecord> history =
            WorkoutStorage.GetUserWorkouts(currentUser.UserID);

        if (history.Count == 0)
        {
            Console.WriteLine("No workout records found.");
            return;
        }

        foreach (WorkoutRecord workout in history)
        {
            Console.WriteLine("\n----------------------------");
            Console.WriteLine($"Category: {workout.Category}");
            Console.WriteLine($"Date: {workout.WorkoutDate}");

            foreach (ExerciseRecord exercise in workout.Exercises)
            {
                Console.WriteLine($"\n[{exercise.Phase}] {exercise.ExerciseName}");
                Console.WriteLine($"Status: {(exercise.Completed ? "Completed" : "Skipped")}");
                if (exercise.Phase == "Main Exercise")
                {
                    Console.WriteLine($"Sets: {exercise.Sets}");
                    Console.WriteLine($"Repetitions: {exercise.Repetitions}");
                    Console.WriteLine($"Weight: {exercise.Weight} kg");
                    Console.WriteLine($"Volume: {exercise.TotalVolume} kg");
                }
            }

            Console.WriteLine($"\nTotal Volume: {workout.TotalVolume} kg");
            Console.WriteLine("----------------------------");
        }

        Console.WriteLine($"\nTotal Workouts: {history.Count}");
    }
}


public class ProgressMenu : MenuNavi
{
    public void ViewProg(User currentUser)
    {
        Console.Clear();

        Console.WriteLine("===== PROGRESS SUMMARY =====");
        Console.WriteLine($"User: {currentUser.Username}");

        MemberProfile? profile = ProfileStorage.GetProfile(currentUser.UserID);
        if (profile != null)
        {
            Console.WriteLine("\n--- FITNESS PROFILE ---");
            Console.WriteLine($"Current Weight: {profile.Weight:N2} kg");

            if (profile.TargetWeight > 0)
                Console.WriteLine($"Target Weight: {profile.TargetWeight:N2} kg");
            else
                Console.WriteLine("Target Weight: Not set");

            Console.WriteLine($"Fitness Goal: {profile.FitnessGoal}");
            Console.WriteLine($"BMI: {profile.CalculateBMI():N2}");

            Console.WriteLine("\n1. Update Fitness Profile");
            Console.WriteLine("2. Continue to Progress Statistics");

            int choice = IMenuNavi.ReadIntInRange(
                "\nChoose an option: ", 1, 2);

            if (choice == 1)
            {
                MemberProfileMenu.UpdateProfile(currentUser);
                Console.Clear();
                profile = ProfileStorage.GetProfile(currentUser.UserID);

                Console.WriteLine("Updated Fitness Profile:");
                Console.WriteLine($"Current Weight: {profile!.Weight:N2} kg");
                Console.WriteLine($"Target Weight: {profile.TargetWeight:N2} kg");
                Console.WriteLine($"Fitness Goal: {profile.FitnessGoal}");
                Console.WriteLine($"BMI: {profile.CalculateBMI():N2}");
                //Console.WriteLine("\nPress Enter to continue...");
                //Console.ReadLine();
                return;
            }
        }
        Console.Clear();
        Console.WriteLine("===== PROGRESS SUMMARY =====");
        Console.WriteLine($"User: {currentUser.Username}");
        List<WorkoutRecord> history =
            WorkoutStorage.GetUserWorkouts(currentUser.UserID);

        if (history.Count == 0)
        {
            Console.WriteLine("\nNo workout data available yet.");
            return;
        }

        int totalWorkouts = history.Count;

        int totalExercises = history.Sum(
            workout => workout.Exercises.Count);

        double totalVolume = history.Sum(
            workout => workout.TotalVolume);

        int pushCount = history.Count(
            workout => workout.Category.Equals(
                "Push", StringComparison.OrdinalIgnoreCase));

        int pullCount = history.Count(
            workout => workout.Category.Equals(
                "Pull", StringComparison.OrdinalIgnoreCase));

        int legsCount = history.Count(
            workout => workout.Category.Equals(
                "Legs", StringComparison.OrdinalIgnoreCase));

        string recommendedPrefix = "Recommended - ";

        List<WorkoutRecord> recommendedWorkouts = history
            .Where(workout => workout.Category.StartsWith(
                recommendedPrefix,
                StringComparison.OrdinalIgnoreCase))
            .ToList();

        int recommendedCount = recommendedWorkouts.Count;

        var goalDistribution = recommendedWorkouts
            .GroupBy(workout =>
                workout.Category.Substring(recommendedPrefix.Length))
            .ToList();

        var recommendedExerciseDistribution = recommendedWorkouts
            .SelectMany(workout => workout.Exercises)
            .Where(exercise =>
                exercise.Phase.Equals(
                    "Main Exercise",
                    StringComparison.OrdinalIgnoreCase)
                && exercise.Completed)
            .GroupBy(exercise => exercise.Category)
            .ToList();

        Console.WriteLine("\n--- GENERAL STATISTICS ---");
        Console.WriteLine($"Total Workouts: {totalWorkouts}");
        Console.WriteLine($"Total Exercises: {totalExercises}");
        Console.WriteLine($"Total Volume: {totalVolume:N2} kg");

        Console.WriteLine("\n--- MANUAL WORKOUT DISTRIBUTION ---");
        Console.WriteLine($"Push Workouts: {pushCount}");
        Console.WriteLine($"Pull Workouts: {pullCount}");
        Console.WriteLine($"Legs Workouts: {legsCount}");

        Console.WriteLine("\n--- RECOMMENDED WORKOUT DISTRIBUTION ---");
        Console.WriteLine($"Total Recommended Workouts: {recommendedCount}");

        if (goalDistribution.Count == 0)
        {
            Console.WriteLine("No recommended workouts recorded.");
        }
        else
        {
            foreach (var group in goalDistribution)
            {
                Console.WriteLine(
                    $"{group.Key}: {group.Count()} workout(s)");
            }
        }
        Console.WriteLine("\n--- RECOMMENDED EXERCISE DISTRIBUTION ---");

        if (recommendedExerciseDistribution.Count == 0)
        {
            Console.WriteLine("No completed recommended exercises recorded.");
        }
        else
        {
            foreach (var group in recommendedExerciseDistribution)
            {
                Console.WriteLine(
                    $"{group.Key}: {group.Count()} completed exercise(s)");
            }
        }
    }
}
public class ExerciseRecord
{
    public string ExerciseName { get; set; } = string.Empty;
    public string Phase { get; set; } = "Main Exercise";
    public string Category { get; set; } = string.Empty;
    public int Sets { get; set; }
    public int Repetitions { get; set; }
    public double Weight { get; set; }
    public bool Completed { get; set; } = true;
    public double TotalVolume => Sets * Repetitions * Weight;
}
public class WorkoutRecord
{
    public string UserID { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime WorkoutDate { get; set; }
    public List<ExerciseRecord> Exercises { get; set; } = new();

    public double TotalVolume => Exercises.Sum(exercise => exercise.TotalVolume);
}
public static class WorkoutStorage
{
    private static readonly string FilePath = Path.Combine(AppContext.BaseDirectory, "Data", "workouts.json");

    public static List<WorkoutRecord> WorkoutHistory { get; private set; } = LoadWorkouts();

    private static List<WorkoutRecord> LoadWorkouts()
    {
        if (!File.Exists(FilePath))
            return new List<WorkoutRecord>();

        string json = File.ReadAllText(FilePath);

        return JsonSerializer.Deserialize<List<WorkoutRecord>>(json)
               ?? new List<WorkoutRecord>();
    }

    private static void SaveWorkouts()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);

        string json = JsonSerializer.Serialize(WorkoutHistory,new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(FilePath, json);
    }

    public static void AddWorkout(WorkoutRecord workout)
    {
        WorkoutHistory.Add(workout);
        SaveWorkouts();
    }

    public static List<WorkoutRecord> GetUserWorkouts(string userID)
    {
        return WorkoutHistory.Where(workout => workout.UserID == userID).ToList();
    }
    public static void DeleteUserWorkouts(string userID)
    {
        WorkoutHistory.RemoveAll(
            workout => workout.UserID == userID);

        SaveWorkouts();
    }
    public class MemberProfile
    {
        public string UserID { get; set; } = "";

        public int Age { get; set; }
        public string Gender { get; set; } = "";

        public double Height { get; set; }
        public double Weight { get; set; }
        public double TargetWeight { get; set; }
        public string FitnessGoal { get; set; } = "";

        public double CalculateBMI()
        {
            double heightMeters = Height / 100;

            return Weight / (heightMeters * heightMeters);
        }
    }

    public static class ProfileStorage
    {
        private static readonly string FilePath =
            Path.Combine(AppContext.BaseDirectory,
            "Data", "profiles.json");

        private static List<MemberProfile> Profiles =
            LoadProfiles();

        private static List<MemberProfile> LoadProfiles()
        {
            if (!File.Exists(FilePath))
                return new List<MemberProfile>();

            string json = File.ReadAllText(FilePath);

            return JsonSerializer.Deserialize<List<MemberProfile>>(json)
                   ?? new List<MemberProfile>();
        }

        private static void SaveProfiles()
        {
            Directory.CreateDirectory(
                Path.GetDirectoryName(FilePath)!);

            string json = JsonSerializer.Serialize(
                Profiles,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            File.WriteAllText(FilePath, json);
        }
        public static void DeleteProfile(string userID)
        {
            Profiles.RemoveAll(p => p.UserID == userID);
            SaveProfiles();
        }

        public static MemberProfile? GetProfile(string userID)
        {
            return Profiles.FirstOrDefault(
                p => p.UserID == userID);
        }

        public static void SaveProfile(MemberProfile profile)
        {
            Profiles.RemoveAll(
                p => p.UserID == profile.UserID);

            Profiles.Add(profile);

            SaveProfiles();
        }
    }
    public class MemberProfileMenu
    {
        public static void EnsureProfile(User currentUser)
        {
            MemberProfile? profile =
                ProfileStorage.GetProfile(currentUser.UserID);
            if (profile != null)
                return;

            Console.Clear();

            Console.WriteLine("===== FITNESS PROFILE REGISTRATION =====");
            Console.WriteLine("Please provide your personal information.\n");

            profile = new MemberProfile();

            profile.UserID = currentUser.UserID;

            profile.Age = ReadInt("Enter your age: ", 5, 120);

            Console.WriteLine("\nSelect Gender:");
            Console.WriteLine("1. Male");
            Console.WriteLine("2. Female");
            Console.WriteLine("3. Prefer not to say");

            int genderChoice = ReadInt("Choice: ", 1, 3);

            profile.Gender = genderChoice switch
            {
                1 => "Male",
                2 => "Female",
                _ => "Prefer not to say"
            };

            profile.Height = ReadDouble(
                "\nEnter your height (cm): ", 50, 250);

            profile.Weight = ReadDouble(
                "Enter your weight (kg): ", 1, 500);

            profile.TargetWeight = ReadDouble(
                "Enter your target weight (kg): ", 1, 500);

            double bmi = profile.CalculateBMI();

            Console.WriteLine("\n===== FITNESS ASSESSMENT =====");
            Console.WriteLine($"Age: {profile.Age}");
            Console.WriteLine($"Gender: {profile.Gender}");
            Console.WriteLine($"Height: {profile.Height} cm");
            Console.WriteLine($"Weight: {profile.Weight} kg");
            Console.WriteLine($"BMI: {bmi:F2}");

            DisplayBMI(bmi, profile.Age);

            Console.WriteLine("\n===== FITNESS GOAL SETTING =====");
            Console.WriteLine("What is your primary fitness goal?");
            Console.WriteLine("1. Lose Weight");
            Console.WriteLine("2. Gain Weight");
            Console.WriteLine("3. Build Muscle");

            int goalChoice = ReadInt("Select goal: ", 1, 3);

            profile.FitnessGoal = goalChoice switch
            {
                1 => "Weight Loss",
                2 => "Weight Gain",
                3 => "Muscle Building",
                _ => ""
            };

            Console.WriteLine(
                $"\nYour selected goal: {profile.FitnessGoal}");

            ProfileStorage.SaveProfile(profile);

            Console.WriteLine("\nFitness profile successfully saved!");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }

        private static void DisplayBMI(double bmi, int age)
        {
            if (age < 20)
            {
                Console.WriteLine(
                    "BMI interpretation requires age-specific assessment.");
                return;
            }

            string classification = bmi switch
            {
                < 18.5 => "Underweight",
                < 25 => "Healthy Weight",
                < 30 => "Overweight",
                _ => "Obesity"
            };

            Console.WriteLine($"Classification: {classification}");
            Console.WriteLine(
                "BMI is a screening measure, not a complete health assessment.");
        }

        private static int ReadInt(
            string message, int min, int max)
        {
            while (true)
            {
                Console.Write(message);

                if (int.TryParse(Console.ReadLine(), out int value)
                    && value >= min && value <= max)
                {
                    return value;
                }

                Console.WriteLine(
                    $"Please enter a number between {min} and {max}.");
            }
        }

        private static double ReadDouble(
            string message, double min, double max)
        {
            while (true)
            {
                Console.Write(message);

                if (double.TryParse(
                    Console.ReadLine(),
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out double value)
                    && double.IsFinite(value)
                    && value >= min && value <= max)
                {
                    return value;
                }

                Console.WriteLine(
                    $"Please enter a valid number between {min} and {max}.");
            }
        }
        public static void UpdateProfile(User currentUser)
        {
            MemberProfile? profile =
                ProfileStorage.GetProfile(currentUser.UserID);

            if (profile == null)
            {
                Console.WriteLine("No fitness profile found.");
                return;
            }

            Console.Clear();
            Console.WriteLine("===== UPDATE FITNESS PROFILE =====");

            Console.WriteLine($"\nCurrent Weight: {profile.Weight} kg");
            profile.Weight = ReadDouble(
                "Enter your updated current weight (kg): ", 1, 500);

            Console.WriteLine($"\nTarget Weight: {profile.TargetWeight} kg");
            profile.TargetWeight = ReadDouble(
                "Enter your new target weight (kg): ", 1, 500);

            Console.WriteLine("\nSelect your fitness goal:");
            Console.WriteLine("1. Lose Weight");
            Console.WriteLine("2. Gain Weight");
            Console.WriteLine("3. Build Muscle");

            int goalChoice = ReadInt("Select goal: ", 1, 3);

            profile.FitnessGoal = goalChoice switch
            {
                1 => "Weight Loss",
                2 => "Weight Gain",
                3 => "Muscle Building",
                _ => profile.FitnessGoal
            };

            ProfileStorage.SaveProfile(profile);

            Console.WriteLine("\nFitness profile successfully updated!");
        }
    }
}