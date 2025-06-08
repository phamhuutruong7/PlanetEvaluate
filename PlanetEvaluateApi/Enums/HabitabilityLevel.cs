namespace PlanetEvaluateApi.Enums
{
    /// <summary>
    /// Represents different levels of planet habitability based on overall score
    /// </summary>
    public enum HabitabilityLevel
    {
        /// <summary>
        /// Completely unsuitable for human habitation (0-20)
        /// </summary>
        Uninhabitable = 0,

        /// <summary>
        /// Very poor conditions, extreme challenges (21-35)
        /// </summary>
        Poor = 1,

        /// <summary>
        /// Below average conditions, significant challenges (36-50)
        /// </summary>
        Fair = 2,

        /// <summary>
        /// Acceptable conditions with some challenges (51-65)
        /// </summary>
        Good = 3,

        /// <summary>
        /// Very good conditions, minor challenges (66-80)
        /// </summary>
        Excellent = 4,

        /// <summary>
        /// Perfect or near-perfect conditions for human habitation (81-100)
        /// </summary>
        Ideal = 5
    }
}
