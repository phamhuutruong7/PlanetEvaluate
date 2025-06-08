namespace PlanetEvaluateApi.Enums
{
    /// <summary>
    /// Enum representing user roles in the Planet Evaluate system
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// Full system access - can manage users, planets, and all data
        /// </summary>
        SuperAdmin,

        /// <summary>
        /// Can manage planets and evaluate habitability
        /// </summary>
        PlanetAdmin,

        /// <summary>
        /// Can view and analyze assigned planets with basic permissions
        /// </summary>
        Viewer1,

        /// <summary>
        /// Can view assigned planets with limited permissions
        /// </summary>
        Viewer2
    }
}
