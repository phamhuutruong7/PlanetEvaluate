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
                var userId = _planetService.GetCurrentUserId(User);
                if (!userId.HasValue)
                    return Unauthorized(new { message = "Invalid user token" });

                var planetDtos = await _planetService.GetAllPlanetDtosAsync(userId.Value);
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
                var userId = _planetService.GetCurrentUserId(User);
                if (!userId.HasValue)
                    return Unauthorized(new { message = "Invalid user token" });

                var planetDto = await _planetService.GetPlanetDtoByIdAsync(id, userId.Value);
                
                if (planetDto == null)
                {
                    return NotFound(new { message = "Planet not found or access denied" });
                }

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
                var userId = _planetService.GetCurrentUserId(User);
                if (!userId.HasValue)
                    return Unauthorized(new { message = "Invalid user token" });

                var planetDto = await _planetService.CreatePlanetFromDtoAsync(createPlanetDto, userId.Value);

                return CreatedAtAction(nameof(GetPlanet), new { id = planetDto.Id }, planetDto);
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
                var userId = _planetService.GetCurrentUserId(User);
                if (!userId.HasValue)
                    return Unauthorized(new { message = "Invalid user token" });

                var planetDto = await _planetService.UpdatePlanetFromDtoAsync(id, updatePlanetDto, userId.Value);

                if (planetDto == null)
                {
                    return NotFound(new { message = "Planet not found or access denied" });
                }

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
                var userId = _planetService.GetCurrentUserId(User);
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
    }
}
