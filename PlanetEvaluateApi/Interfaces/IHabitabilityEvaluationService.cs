using PlanetEvaluateApi.Models;

namespace PlanetEvaluateApi.Interfaces
{
    public interface IHabitabilityEvaluationService
    {
        Task<HabitabilityEvaluation> EvaluatePlanetHabitabilityAsync(int planetId);

        Task<List<HabitabilityEvaluation>> RankPlanetsByHabitabilityAsync();        
        Task<HabitabilityEvaluation> FindMostHabitablePlanetAsync();

        HabitabilityFactorScores CalculateFactorScores(Planet planet);
    }
}
