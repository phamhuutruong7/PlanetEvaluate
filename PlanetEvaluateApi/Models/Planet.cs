using System.ComponentModel.DataAnnotations;

namespace PlanetEvaluateApi.Models
{
    public class Planet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Type { get; set; }

        public double? Mass { get; set; } // In Earth masses

        public double? Radius { get; set; } // In Earth radii

        public double? DistanceFromSun { get; set; } // In AU

        public int? NumberOfMoons { get; set; }

        public bool HasAtmosphere { get; set; }

        [Range(0, 100)]
        public double? OxygenVolume { get; set; } // Percentage of oxygen in atmosphere (0-100)

        [Range(0, 100)]
        public double? WaterVolume { get; set; } // Percentage of surface covered by water (0-100)

        [Range(1, 10)]
        public int? HardnessOfRock { get; set; } // Scale 1-10 (1 is softest, 10 is hardest)

        [Range(1, 10)]
        public int? ThreateningCreature { get; set; } // Scale 1-10 (1 is lowest threat, 10 is highest threat)

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }
    }
}
