using System.Text.Json;

namespace StudentFitnessTracker;

public static class ExerciseStorage
{
    private static readonly string FilePath = Path.Combine(AppContext.BaseDirectory, "Data", "exercises.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public static List<Exercise> LoadExercises()
    {
        if (!File.Exists(FilePath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
            File.WriteAllText(FilePath, "[]");
            return new List<Exercise>();
        }

        string json = File.ReadAllText(FilePath);

        try
        {
            return JsonSerializer.Deserialize<List<Exercise>>(json, JsonOptions) ?? new List<Exercise>();
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException($"The exercise database contains invalid JSON. Correct Data/exercises.json and restart the application. Details: {ex.Message}", ex);
        }
    }

    public static void SaveExercises(List<Exercise> exercises)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        string json = JsonSerializer.Serialize(exercises, JsonOptions);
        File.WriteAllText(FilePath, json);
    }
}
