using PlanetEvaluateApi.Enums;

namespace PlanetEvaluateApi.Models
{
    public class HabitabilityEvaluation
    {
        public int PlanetId { get; set; }

        public string PlanetName { get; set; } = string.Empty;

        public double OverallHabitabilityScore { get; set; }

        public HabitabilityFactorScores FactorScores { get; set; } = new();

        public HabitabilityLevel HabitabilityLevel { get; set; }

        public List<string> PositiveFactors { get; set; } = new();

        public List<string> NegativeFactors { get; set; } = new();

        public string Summary { get; set; } = string.Empty;

        public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;
    }
}
