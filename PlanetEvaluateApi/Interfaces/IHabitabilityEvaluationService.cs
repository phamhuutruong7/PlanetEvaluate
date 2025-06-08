using PlanetEvaluateApi.Models;

namespace PlanetEvaluateApi.Interfaces
{
    /// <summary>
    /// Service interface for planet habitability evaluation operations
    /// </summary>
    public interface IHabitabilityEvaluationService
    {
        /// <summary>
        /// Evaluate the habitability of a specific planet
        /// </summary>
        /// <param name="planetId">The ID of the planet to evaluate</param>
        /// <returns>Habitability evaluation results</returns>
        Task<HabitabilityEvaluation> EvaluatePlanetHabitabilityAsync(int planetId);

        /// <summary>
        /// Rank all planets by their habitability scores
        /// </summary>
        /// <returns>List of planets ranked by habitability (best to worst)</returns>
        Task<List<HabitabilityEvaluation>> RankPlanetsByHabitabilityAsync();        /// <summary>
        /// Find the most habitable planet
        /// </summary>
        /// <returns>The planet with the highest habitability score</returns>
        Task<HabitabilityEvaluation> FindMostHabitablePlanetAsync();

        /// <summary>
        /// Calculate individual factor scores for a planet
        /// </summary>
        /// <param name="planet">The planet to calculate scores for</param>
        /// <returns>Individual factor scores breakdown</returns>
        HabitabilityFactorScores CalculateFactorScores(Planet planet);
    }
}
