namespace StudentFitnessTracker;

public static class ExerciseLibrary
{
    public static IReadOnlyList<Exercise> All => ExerciseStorage.LoadExercises();

    public static List<Exercise> GetByPhase(string phase)
    {
        return ExerciseStorage.LoadExercises()
            .Where(exercise => exercise.Phase.Equals(phase, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public static List<Exercise> GetByCategory(string category)
    {
        return ExerciseStorage.LoadExercises()
            .Where(exercise => exercise.Phase.Equals("Main Exercise", StringComparison.OrdinalIgnoreCase)
                && exercise.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
    public static List<Exercise> GetByGoal(string goal)
    {
        return ExerciseStorage.LoadExercises()
            .Where(e =>
                e.Phase.Equals("Main Exercise",
                    StringComparison.OrdinalIgnoreCase)
                && e.GoalTypes != null
                && e.GoalTypes.Contains(
                    goal,
                    StringComparer.OrdinalIgnoreCase))
            .ToList();
    }
    public static List<Exercise> Search(string keyword)
    {
        return ExerciseStorage.LoadExercises()
            .Where(e => e.Name.Contains(
                keyword,
                StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}
