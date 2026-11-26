using System.Text.Json.Serialization;

namespace StudentFitnessTracker;

public class Exercise
{
    public string Name { get; set; } = string.Empty;
    public string Phase { get; set; } = "Main Exercise";
    public string Category { get; set; } = string.Empty;
    public string TargetMuscles { get; set; } = string.Empty;
    public List<string> GoalTypes { get; set; } = new();
}
