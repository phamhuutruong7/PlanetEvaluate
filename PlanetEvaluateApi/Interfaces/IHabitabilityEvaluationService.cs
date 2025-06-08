using PlanetEvaluateApi.Models;
using PlanetEvaluateApi.Enums;
using System.Security.Claims;

namespace PlanetEvaluateApi.Interfaces
{
    /// <summary>
    /// Interface for habitability evaluation services
    /// </summary>
    public interface IHabitabilityEvaluationService
    {
        /// <summary>
        /// Evaluates the habitability of a single planet
        /// </summary>
        /// <param name="planet">The planet to evaluate</param>
        /// <returns>Complete habitability evaluation</returns>
        Task<HabitabilityEvaluation> EvaluatePlanetAsync(Planet planet);

        /// <summary>
        /// Evaluates and ranks multiple planets by habitability
        /// </summary>
        /// <param name="planets">List of planets to evaluate and rank</param>
        /// <returns>List of planets ranked by habitability (best first)</returns>
        Task<List<HabitabilityEvaluation>> RankPlanetsByHabitabilityAsync(List<Planet> planets);

        /// <summary>
        /// Finds the most habitable planet from a list
        /// </summary>
        /// <param name="planets">List of planets to evaluate</param>
        /// <returns>The most habitable planet evaluation, or null if no planets provided</returns>
        Task<HabitabilityEvaluation?> FindMostHabitablePlanetAsync(List<Planet> planets);

        /// <summary>
        /// Calculates individual factor scores for a planet
        /// </summary>
        /// <param name="planet">The planet to score</param>
        /// <returns>Individual factor scores</returns>
        HabitabilityFactorScores CalculateFactorScores(Planet planet);

        /// <summary>
        /// Determines habitability level based on overall score
        /// </summary>
        /// <param name="score">Overall habitability score (0-100)</param>
        /// <returns>Corresponding habitability level</returns>
        HabitabilityLevel DetermineHabitabilityLevel(double score);

        /// <summary>
        /// Generates evaluation summary and recommendations
        /// </summary>
        /// <param name="planet">The planet being evaluated</param>
        /// <param name="factorScores">Individual factor scores</param>
        /// <param name="overallScore">Overall habitability score</param>
        /// <returns>Tuple containing summary and recommendations</returns>
        (string summary, List<string> recommendations) GenerateEvaluationDetails(
            Planet planet, 
            HabitabilityFactorScores factorScores, 
            double overallScore);

        // Business logic operations for controllers
        Task<HabitabilityEvaluation?> EvaluatePlanetByIdAsync(int planetId, int userId);
        Task<List<HabitabilityEvaluation>> RankAccessiblePlanetsByHabitabilityAsync(int userId);
        Task<HabitabilityEvaluation?> FindMostHabitableAccessiblePlanetAsync(int userId);
        Task<BatchEvaluationResult> EvaluatePlanetsBatchAsync(List<int> planetIds, int userId);
        Task<HabitabilityFactorScores?> GetPlanetFactorScoresAsync(int planetId, int userId);
        int? GetCurrentUserId(ClaimsPrincipal principal);
    }

    /// <summary>
    /// Result of a batch evaluation operation
    /// </summary>
    public class BatchEvaluationResult
    {
        public List<HabitabilityEvaluation> Evaluations { get; set; } = new();
        public List<int> NotFoundIds { get; set; } = new();
    }
}
