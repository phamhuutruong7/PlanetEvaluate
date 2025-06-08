namespace PlanetEvaluateApi.Models
{
    /// <summary>
    /// Represents the individual factor scores for habitability evaluation
    /// </summary>
    public class HabitabilityFactorScores
    {
        /// <summary>
        /// Score based on oxygen percentage in atmosphere (0-100)
        /// </summary>
        public double OxygenScore { get; set; }

        /// <summary>
        /// Score based on water coverage percentage (0-100)
        /// </summary>
        public double WaterScore { get; set; }

        /// <summary>
        /// Score based on atmosphere presence and conditions (0-100)
        /// </summary>
        public double AtmosphereScore { get; set; }

        /// <summary>
        /// Score based on distance from sun and habitable zone (0-100)
        /// </summary>
        public double DistanceScore { get; set; }

        /// <summary>
        /// Score based on threatening creatures level (0-100)
        /// </summary>
        public double SafetyScore { get; set; }

        /// <summary>
        /// Score based on terrain hardness for construction (0-100)
        /// </summary>
        public double TerrainScore { get; set; }
    }
}
