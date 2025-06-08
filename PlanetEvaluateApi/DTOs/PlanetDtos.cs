namespace PlanetEvaluateApi.DTOs
{
    public class PlanetDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Type { get; set; }
        public double? Mass { get; set; }
        public double? Radius { get; set; }
        public double? DistanceFromSun { get; set; }
        public int? NumberOfMoons { get; set; }
        public bool HasAtmosphere { get; set; }
        public double? OxygenVolume { get; set; }
        public double? WaterVolume { get; set; }
        public int? HardnessOfRock { get; set; }
        public int? ThreateningCreature { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Description { get; set; }
    }

    public class CreatePlanetDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Type { get; set; }
        public double? Mass { get; set; }
        public double? Radius { get; set; }
        public double? DistanceFromSun { get; set; }
        public int? NumberOfMoons { get; set; }
        public bool HasAtmosphere { get; set; }
        public double? OxygenVolume { get; set; }
        public double? WaterVolume { get; set; }
        public int? HardnessOfRock { get; set; }
        public int? ThreateningCreature { get; set; }
        public string? Description { get; set; }
    }

    public class UpdatePlanetDto
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public double? Mass { get; set; }
        public double? Radius { get; set; }
        public double? DistanceFromSun { get; set; }
        public int? NumberOfMoons { get; set; }
        public bool? HasAtmosphere { get; set; }
        public double? OxygenVolume { get; set; }
        public double? WaterVolume { get; set; }
        public int? HardnessOfRock { get; set; }
        public int? ThreateningCreature { get; set; }
        public string? Description { get; set; }
    }
}
