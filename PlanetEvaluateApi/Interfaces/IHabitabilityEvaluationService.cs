using PlanetEvaluateApi.Models;
using PlanetEvaluateApi.Enums;

namespace PlanetEvaluateApi.Interfaces
{
    /// <summary>
    /// Interface for habitability evaluation services
    /// </summary>
    public interface IHabitabilityEvaluationService
    {
        /// <summary>
        /// Evaluates the habitability of a planet by its ID
        /// </summary>
        /// <param name="planetId">The ID of the planet to evaluate</param>
        /// <returns>Complete habitability evaluation</returns>
        Task<HabitabilityEvaluation> EvaluatePlanetHabitabilityAsync(int planetId);

        /// <summary>
        /// Ranks all planets by their habitability scores
        /// </summary>
        /// <returns>List of planets ranked by habitability (best first)</returns>
        Task<List<HabitabilityEvaluation>> RankPlanetsByHabitabilityAsync();

        /// <summary>
        /// Finds the most habitable planet
        /// </summary>
        /// <returns>The most habitable planet evaluation</returns>
        Task<HabitabilityEvaluation> FindMostHabitablePlanetAsync();
    }
}
