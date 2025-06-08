using PlanetEvaluateApi.Enums;

namespace PlanetEvaluateApi.Models
{
    /// <summary>
    /// Represents a complete habitability evaluation for a planet
    /// </summary>
    public class HabitabilityEvaluation
    {
        /// <summary>
        /// The planet ID being evaluated
        /// </summary>
        public int PlanetId { get; set; }

        /// <summary>
        /// The planet name being evaluated
        /// </summary>
        public string PlanetName { get; set; } = string.Empty;

        /// <summary>
        /// Overall habitability score (0-100)
        /// </summary>
        public double OverallHabitabilityScore { get; set; }

        /// <summary>
        /// Individual factor scores breakdown
        /// </summary>
        public HabitabilityFactorScores FactorScores { get; set; } = new();

        /// <summary>
        /// Habitability level based on overall score
        /// </summary>
        public HabitabilityLevel HabitabilityLevel { get; set; }

        /// <summary>
        /// List of positive habitability factors
        /// </summary>
        public List<string> PositiveFactors { get; set; } = new();

        /// <summary>
        /// List of negative habitability factors
        /// </summary>
        public List<string> NegativeFactors { get; set; } = new();

        /// <summary>
        /// Detailed evaluation summary
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when evaluation was performed
        /// </summary>
        public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;
    }
}
