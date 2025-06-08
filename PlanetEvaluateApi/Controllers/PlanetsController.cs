using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanetEvaluateApi.DTOs;
using PlanetEvaluateApi.Interfaces;
using PlanetEvaluateApi.Models;
using System.Security.Claims;

namespace PlanetEvaluateApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PlanetsController : ControllerBase
    {
        private readonly IPlanetService _planetService;

        public PlanetsController(IPlanetService planetService)
        {
            _planetService = planetService;
        }        
        
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,PlanetAdmin,Viewer1,Viewer2")]
        public async Task<ActionResult<IEnumerable<PlanetDto>>> GetAllPlanets()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                    return Unauthorized(new { message = "Invalid user token" });

                var planets = await _planetService.GetAllPlanetsAsync(userId.Value);
                
                var planetDtos = planets.Select(p => new PlanetDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Type = p.Type,
                    Mass = p.Mass,
                    Radius = p.Radius,
                    DistanceFromSun = p.DistanceFromSun,
                    NumberOfMoons = p.NumberOfMoons,
                    HasAtmosphere = p.HasAtmosphere,
                    OxygenVolume = p.OxygenVolume,
                    WaterVolume = p.WaterVolume,
                    HardnessOfRock = p.HardnessOfRock,
                    ThreateningCreature = p.ThreateningCreature,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    Description = p.Description
                });

                return Ok(planetDtos);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching planets", error = ex.Message });
            }
        }        
        
        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin,PlanetAdmin,Viewer1,Viewer2")]
        public async Task<ActionResult<PlanetDto>> GetPlanet(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                    return Unauthorized(new { message = "Invalid user token" });

                var planet = await _planetService.GetPlanetByIdAsync(id, userId.Value);
                
                if (planet == null)
                {
                    return NotFound(new { message = "Planet not found or access denied" });
                }

                var planetDto = new PlanetDto
                {
                    Id = planet.Id,
                    Name = planet.Name,
                    Type = planet.Type,
                    Mass = planet.Mass,
                    Radius = planet.Radius,
                    DistanceFromSun = planet.DistanceFromSun,
                    NumberOfMoons = planet.NumberOfMoons,
                    HasAtmosphere = planet.HasAtmosphere,
                    OxygenVolume = planet.OxygenVolume,
                    WaterVolume = planet.WaterVolume,
                    HardnessOfRock = planet.HardnessOfRock,
                    ThreateningCreature = planet.ThreateningCreature,
                    CreatedAt = planet.CreatedAt,
                    UpdatedAt = planet.UpdatedAt,
                    Description = planet.Description
                };

                return Ok(planetDto);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the planet", error = ex.Message });
            }
        }        
        
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,PlanetAdmin")]
        public async Task<ActionResult<PlanetDto>> CreatePlanet([FromBody] CreatePlanetDto createPlanetDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                    return Unauthorized(new { message = "Invalid user token" });

                var planet = new Planet
                {
                    Name = createPlanetDto.Name,
                    Type = createPlanetDto.Type,
                    Mass = createPlanetDto.Mass,
                    Radius = createPlanetDto.Radius,
                    DistanceFromSun = createPlanetDto.DistanceFromSun,
                    NumberOfMoons = createPlanetDto.NumberOfMoons,
                    HasAtmosphere = createPlanetDto.HasAtmosphere,
                    OxygenVolume = createPlanetDto.OxygenVolume,
                    WaterVolume = createPlanetDto.WaterVolume,
                    HardnessOfRock = createPlanetDto.HardnessOfRock,
                    ThreateningCreature = createPlanetDto.ThreateningCreature,
                    Description = createPlanetDto.Description
                };

                var createdPlanet = await _planetService.CreatePlanetAsync(planet, userId.Value);
                
                var planetDto = new PlanetDto
                {
                    Id = createdPlanet.Id,
                    Name = createdPlanet.Name,
                    Type = createdPlanet.Type,
                    Mass = createdPlanet.Mass,
                    Radius = createdPlanet.Radius,
                    DistanceFromSun = createdPlanet.DistanceFromSun,
                    NumberOfMoons = createdPlanet.NumberOfMoons,
                    HasAtmosphere = createdPlanet.HasAtmosphere,
                    OxygenVolume = createdPlanet.OxygenVolume,
                    WaterVolume = createdPlanet.WaterVolume,
                    HardnessOfRock = createdPlanet.HardnessOfRock,
                    ThreateningCreature = createdPlanet.ThreateningCreature,
                    CreatedAt = createdPlanet.CreatedAt,
                    UpdatedAt = createdPlanet.UpdatedAt,
                    Description = createdPlanet.Description
                };

                return CreatedAtAction(nameof(GetPlanet), new { id = createdPlanet.Id }, planetDto);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the planet", error = ex.Message });
            }
        }        
        
        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin,PlanetAdmin")]
        public async Task<ActionResult<PlanetDto>> UpdatePlanet(int id, [FromBody] UpdatePlanetDto updatePlanetDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                    return Unauthorized(new { message = "Invalid user token" });
                
                // Get existing planet to preserve unchanged fields
                var existingPlanet = await _planetService.GetPlanetByIdAsync(id, userId.Value);
                if (existingPlanet == null)
                {
                    return NotFound(new { message = "Planet not found or access denied" });
                }

                // Update only provided fields
                var planet = new Planet
                {
                    Id = id,
                    Name = updatePlanetDto.Name ?? existingPlanet.Name,
                    Type = updatePlanetDto.Type ?? existingPlanet.Type,
                    Mass = updatePlanetDto.Mass ?? existingPlanet.Mass,
                    Radius = updatePlanetDto.Radius ?? existingPlanet.Radius,
                    DistanceFromSun = updatePlanetDto.DistanceFromSun ?? existingPlanet.DistanceFromSun,
                    NumberOfMoons = updatePlanetDto.NumberOfMoons ?? existingPlanet.NumberOfMoons,
                    HasAtmosphere = updatePlanetDto.HasAtmosphere ?? existingPlanet.HasAtmosphere,
                    OxygenVolume = updatePlanetDto.OxygenVolume ?? existingPlanet.OxygenVolume,
                    WaterVolume = updatePlanetDto.WaterVolume ?? existingPlanet.WaterVolume,
                    HardnessOfRock = updatePlanetDto.HardnessOfRock ?? existingPlanet.HardnessOfRock,
                    ThreateningCreature = updatePlanetDto.ThreateningCreature ?? existingPlanet.ThreateningCreature,
                    Description = updatePlanetDto.Description ?? existingPlanet.Description,
                    CreatedAt = existingPlanet.CreatedAt
                };

                var updatedPlanet = await _planetService.UpdatePlanetAsync(id, planet, userId.Value);
                if (updatedPlanet == null)
                {
                    return NotFound(new { message = "Planet not found or access denied" });
                }

                var planetDto = new PlanetDto
                {
                    Id = updatedPlanet.Id,
                    Name = updatedPlanet.Name,
                    Type = updatedPlanet.Type,
                    Mass = updatedPlanet.Mass,
                    Radius = updatedPlanet.Radius,
                    DistanceFromSun = updatedPlanet.DistanceFromSun,
                    NumberOfMoons = updatedPlanet.NumberOfMoons,
                    HasAtmosphere = updatedPlanet.HasAtmosphere,
                    OxygenVolume = updatedPlanet.OxygenVolume,
                    WaterVolume = updatedPlanet.WaterVolume,
                    HardnessOfRock = updatedPlanet.HardnessOfRock,
                    ThreateningCreature = updatedPlanet.ThreateningCreature,
                    CreatedAt = updatedPlanet.CreatedAt,
                    UpdatedAt = updatedPlanet.UpdatedAt,
                    Description = updatedPlanet.Description
                };

                return Ok(planetDto);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the planet", error = ex.Message });
            }
        }        
        
        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult> DeletePlanet(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                    return Unauthorized(new { message = "Invalid user token" });

                var success = await _planetService.DeletePlanetAsync(id, userId.Value);
                
                if (!success)
                {
                    return NotFound(new { message = "Planet not found or access denied" });
                }

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the planet", error = ex.Message });
            }
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }
    }
}
