using Microsoft.EntityFrameworkCore;
using PlanetEvaluateApi.Constants;
using PlanetEvaluateApi.Enums;
using PlanetEvaluateApi.Interfaces;
using PlanetEvaluateApi.Models;
using PlanetEvaluateApi.Data;

namespace PlanetEvaluateApi.Services
{
    public class HabitabilityEvaluationService : IHabitabilityEvaluationService
    {
        private readonly PlanetEvaluateDbContext _context;
        private readonly ILogger<HabitabilityEvaluationService> _logger;

        public HabitabilityEvaluationService(
            PlanetEvaluateDbContext context,
            ILogger<HabitabilityEvaluationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // TODO: These following function is based on suggestion from chatbot.
        // I can comback to this later and use the condition to send to OpenRouter to use chatbot for reasoning 
        // which planet is suitable to live. If I have enough time.

        public async Task<HabitabilityEvaluation> EvaluatePlanetHabitabilityAsync(int planetId)
        {
            var planet = await _context.Planets.FindAsync(planetId);
            if (planet == null)
                throw new ArgumentException($"Planet with ID {planetId} not found");

            var evaluation = new HabitabilityEvaluation();
            evaluation.PlanetId = planetId;
            evaluation.PlanetName = planet.Name;

            // Calculate factor scores
            var factorScores = CalculateFactorScores(planet);
            evaluation.FactorScores = factorScores;

            // Calculate overall score
            evaluation.OverallHabitabilityScore = CalculateOverallScore(factorScores);

            // Determine habitability level
            evaluation.HabitabilityLevel = DetermineHabitabilityLevel(evaluation.OverallHabitabilityScore);

            // Generate factors lists
            var (positive, negative) = GenerateFactorLists(planet, factorScores);
            evaluation.PositiveFactors = positive;
            evaluation.NegativeFactors = negative;

            // Generate summary
            evaluation.Summary = GenerateHabitabilitySummary(evaluation.HabitabilityLevel, evaluation.OverallHabitabilityScore);

            evaluation.EvaluatedAt = DateTime.UtcNow;

            return evaluation;
        }

        public async Task<List<HabitabilityEvaluation>> RankPlanetsByHabitabilityAsync()
        {
            var planets = await _context.Planets.ToListAsync();
            var evaluations = new List<HabitabilityEvaluation>();

            foreach (var planet in planets)
            {
                try
                {
                    var evaluation = await EvaluatePlanetHabitabilityAsync(planet.Id);
                    evaluations.Add(evaluation);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to evaluate planet {PlanetId}", planet.Id);
                }
            }

            return evaluations.OrderByDescending(e => e.OverallHabitabilityScore).ToList();
        }

        public async Task<HabitabilityEvaluation> FindMostHabitablePlanetAsync()
        {
            var rankings = await RankPlanetsByHabitabilityAsync();
            
            if (!rankings.Any())
                throw new InvalidOperationException("No planets available for evaluation");

            return rankings.First();
        }

        public HabitabilityFactorScores CalculateFactorScores(Planet planet)
        {
            return new HabitabilityFactorScores
            {
                OxygenScore = CalculateOxygenScore(planet.OxygenVolume),
                WaterScore = CalculateWaterScore(planet.WaterVolume),
                AtmosphereScore = CalculateAtmosphereScore(planet.HasAtmosphere),
                DistanceScore = CalculateDistanceScore(planet.DistanceFromSun),
                SafetyScore = CalculateSafetyScore(planet.ThreateningCreature),
                TerrainScore = CalculateTerrainScore(planet.HardnessOfRock)
            };
        }

        private double CalculateOxygenScore(double? oxygenVolume)
        {
            if (!oxygenVolume.HasValue) return 0;

            const double optimal = 21.0; // Earth's oxygen percentage
            const double minViable = 16.0; // Minimum for human survival
            const double maxViable = 23.5; // Maximum safe level

            if (oxygenVolume.Value < minViable)
                return Math.Max(0, (oxygenVolume.Value / minViable) * 50);
            
            if (oxygenVolume.Value > maxViable)
                return Math.Max(0, 100 - ((oxygenVolume.Value - maxViable) / (35 - maxViable) * 100));
            
            // In optimal range
            var distanceFromOptimal = Math.Abs(oxygenVolume.Value - optimal);
            return 100 - (distanceFromOptimal / (optimal - minViable) * 30);
        }

        private double CalculateWaterScore(double? waterVolume)
        {
            if (!waterVolume.HasValue) return 0;

            return waterVolume.Value switch
            {
                >= 70 => 100,
                >= 50 => 90,
                >= 30 => 70,
                >= 15 => 50,
                >= 5 => 30,
                > 0 => 20,
                _ => 0
            };
        }

        private double CalculateAtmosphereScore(bool hasAtmosphere)
        {
            return hasAtmosphere ? 100 : 0;
        }

        private double CalculateDistanceScore(double? distanceFromSun)
        {
            if (!distanceFromSun.HasValue) return 0;

            // Habitable zone distances (in AU, like Earth = 1 AU)
            const double innerHZ = 0.95;
            const double outerHZ = 1.37;
            const double optimal = 1.0; // Earth distance

            if (distanceFromSun.Value < innerHZ || distanceFromSun.Value > outerHZ)
                return 0;

            var distanceFromOptimal = Math.Abs(distanceFromSun.Value - optimal);
            var maxDeviation = Math.Max(optimal - innerHZ, outerHZ - optimal);
            
            return Math.Max(0, 100 - (distanceFromOptimal / maxDeviation * 100));
        }        private double CalculateSafetyScore(int? threateningCreature)
        {
            if (!threateningCreature.HasValue) return 50; // Neutral if unknown

            // Lower threat level = higher safety score
            // Scale: 1 (safest) to 10 (most dangerous)
            return Math.Max(0, 100 - ((threateningCreature.Value - 1) * 100.0 / 9.0));
        }

        private double CalculateTerrainScore(int? hardnessOfRock)
        {
            if (!hardnessOfRock.HasValue) return 50; // Neutral if unknown

            const int optimal = 4; // Moderate hardness
            var difference = Math.Abs(hardnessOfRock.Value - optimal);
            var maxDifference = Math.Max(optimal - 1, 10 - optimal);
            
            return Math.Max(0, 100 - (difference / (double)maxDifference * 100));
        }

        private double CalculateOverallScore(HabitabilityFactorScores scores)
        {
            return (scores.OxygenScore * PlanetConstants.OxygenWeight +
                   scores.WaterScore * PlanetConstants.WaterWeight +
                   scores.AtmosphereScore * PlanetConstants.AtmosphereWeight +
                   scores.DistanceScore * PlanetConstants.DistanceWeight +
                   scores.SafetyScore * PlanetConstants.SafetyWeight +
                   scores.TerrainScore * PlanetConstants.TerrainWeight);
        }

        private HabitabilityLevel DetermineHabitabilityLevel(double score)
        {
            return score switch
            {
                >= 81 => HabitabilityLevel.Ideal,
                >= 66 => HabitabilityLevel.Excellent,
                >= 51 => HabitabilityLevel.Good,
                >= 36 => HabitabilityLevel.Fair,
                >= 21 => HabitabilityLevel.Poor,
                _ => HabitabilityLevel.Uninhabitable
            };
        }

        private (List<string> positive, List<string> negative) GenerateFactorLists(Planet planet, HabitabilityFactorScores scores)
        {
            var positive = new List<string>();
            var negative = new List<string>();

            if (scores.OxygenScore >= 70) positive.Add("Good oxygen levels");
            else if (scores.OxygenScore < 30) negative.Add("Poor oxygen levels");

            if (scores.WaterScore >= 70) positive.Add("Abundant water resources");
            else if (scores.WaterScore < 30) negative.Add("Limited water availability");

            if (scores.AtmosphereScore >= 70) positive.Add("Suitable atmosphere");
            else negative.Add("No atmosphere or unsuitable atmosphere");

            if (scores.DistanceScore >= 70) positive.Add("Optimal distance from sun");
            else if (scores.DistanceScore < 30) negative.Add("Too close or too far from sun");

            if (scores.SafetyScore >= 70) positive.Add("Safe from dangerous creatures");
            else if (scores.SafetyScore < 30) negative.Add("High threat from dangerous creatures");

            if (scores.TerrainScore >= 70) positive.Add("Suitable terrain for settlement");
            else if (scores.TerrainScore < 30) negative.Add("Challenging terrain conditions");

            return (positive, negative);
        }

        private string GenerateHabitabilitySummary(HabitabilityLevel level, double score)
        {
            return level switch
            {
                HabitabilityLevel.Ideal => $"Excellent habitability (Score: {score:F1}). This planet offers ideal conditions for human settlement.",
                HabitabilityLevel.Excellent => $"Very good habitability (Score: {score:F1}). This planet is well-suited for colonization with minor challenges.",
                HabitabilityLevel.Good => $"Good habitability (Score: {score:F1}). Settlement is feasible with some adaptation required.",
                HabitabilityLevel.Fair => $"Fair habitability (Score: {score:F1}). Settlement is possible but may require significant adaptation.",
                HabitabilityLevel.Poor => $"Poor habitability (Score: {score:F1}). Survival would be challenging and require extensive life support.",
                _ => $"Uninhabitable (Score: {score:F1}). This planet cannot support human life under current conditions."
            };
        }
    }
}
