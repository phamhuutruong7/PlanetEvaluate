using PlanetEvaluateApi.Enums;

namespace PlanetEvaluateApi.Models
{
    /// <summary>
    /// Represents a complete habitability evaluation for a planet
    /// </summary>
    public class HabitabilityEvaluation
    {
        /// <summary>
        /// The planet being evaluated
        /// </summary>
        public Planet Planet { get; set; } = null!;

        /// <summary>
        /// Overall habitability score (0-100)
        /// </summary>
        public double OverallScore { get; set; }

        /// <summary>
        /// Individual factor scores breakdown
        /// </summary>
        public HabitabilityFactorScores FactorScores { get; set; } = new();

        /// <summary>
        /// Habitability level based on overall score
        /// </summary>
        public HabitabilityLevel HabitabilityLevel { get; set; }

        /// <summary>
        /// Detailed evaluation summary and recommendations
        /// </summary>
        public string EvaluationSummary { get; set; } = string.Empty;

        /// <summary>
        /// List of specific recommendations for improving habitability
        /// </summary>
        public List<string> Recommendations { get; set; } = new();

        /// <summary>
        /// Timestamp when evaluation was performed
        /// </summary>
        public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;
    }
}
