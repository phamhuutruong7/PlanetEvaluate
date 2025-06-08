namespace PlanetEvaluateApi.Constants
{
    /// <summary>
    /// Constants used for planet habitability evaluation
    /// </summary>
    public static class PlanetConstants
    {
        // Optimal values for habitability (Earth-like conditions)
        public const double OPTIMAL_OXYGEN_PERCENTAGE = 21.0; // Earth's oxygen level
        public const double OPTIMAL_WATER_PERCENTAGE = 71.0; // Earth's water coverage
        public const double OPTIMAL_DISTANCE_FROM_SUN = 1.0; // Earth's distance (AU)
        public const int OPTIMAL_HARDNESS = 5; // Moderate hardness for construction

        // Habitability zone boundaries (AU)
        public const double MIN_HABITABLE_DISTANCE = 0.8;
        public const double MAX_HABITABLE_DISTANCE = 1.5;

        // Weighting factors for overall habitability score
        public const double OXYGEN_WEIGHT = 0.25;     // 25% - Critical for breathing
        public const double WATER_WEIGHT = 0.25;      // 25% - Essential for life
        public const double ATMOSPHERE_WEIGHT = 0.20; // 20% - Protection and pressure
        public const double DISTANCE_WEIGHT = 0.15;   // 15% - Temperature regulation
        public const double SAFETY_WEIGHT = 0.10;     // 10% - Creature threats
        public const double TERRAIN_WEIGHT = 0.05;    // 5% - Construction viability

        // Validation constants
        public const double MIN_SCORE = 0.0;
        public const double MAX_SCORE = 100.0;
    }
}
