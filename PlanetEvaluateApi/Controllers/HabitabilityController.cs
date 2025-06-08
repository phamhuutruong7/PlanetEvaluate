using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanetEvaluateApi.Interfaces;
using PlanetEvaluateApi.Models;
using PlanetEvaluateApi.Enums;

namespace PlanetEvaluateApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class HabitabilityController : ControllerBase
    {
        private readonly ILogger<HabitabilityController> _logger;
        private readonly IHabitabilityEvaluationService _habitabilityService;

        public HabitabilityController(
            ILogger<HabitabilityController> logger,
            IHabitabilityEvaluationService habitabilityService)
        {
            _logger = logger;
            _habitabilityService = habitabilityService;
        }

        /// <summary>
        /// Evaluate the habitability of a specific planet
        /// </summary>
        /// <param name="id">Planet ID</param>
        /// <returns>Habitability evaluation for the planet</returns>
        [HttpGet("planet/{id}")]
        public async Task<ActionResult<HabitabilityEvaluation>> EvaluatePlanetHabitability(int id)
        {
            try
            {
                var evaluation = await _habitabilityService.EvaluatePlanetHabitabilityAsync(id);
                _logger.LogInformation("Successfully evaluated habitability for planet {PlanetId}", id);
                return Ok(evaluation);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Planet {PlanetId} not found for habitability evaluation", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating habitability for planet {PlanetId}", id);
                return StatusCode(500, new { message = "An error occurred while evaluating planet habitability" });
            }
        }

        /// <summary>
        /// Rank all planets by their habitability scores
        /// </summary>
        /// <returns>List of planets ranked by habitability (best to worst)</returns>
        [HttpGet("rank")]
        public async Task<ActionResult<List<HabitabilityEvaluation>>> RankPlanetsByHabitability()
        {
            try
            {
                var rankedPlanets = await _habitabilityService.RankPlanetsByHabitabilityAsync();
                _logger.LogInformation("Successfully ranked {Count} planets by habitability", rankedPlanets.Count);
                return Ok(rankedPlanets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ranking planets by habitability");
                return StatusCode(500, new { message = "An error occurred while ranking planets" });
            }
        }

        /// <summary>
        /// Find the most habitable planet
        /// </summary>
        /// <returns>The planet with the highest habitability score</returns>
        [HttpGet("most-habitable")]
        public async Task<ActionResult<HabitabilityEvaluation>> FindMostHabitablePlanet()
        {
            try
            {
                var mostHabitable = await _habitabilityService.FindMostHabitablePlanetAsync();
                _logger.LogInformation("Successfully found most habitable planet: {PlanetName}", mostHabitable.PlanetName);
                return Ok(mostHabitable);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "No planets available for habitability evaluation");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding most habitable planet");
                return StatusCode(500, new { message = "An error occurred while finding the most habitable planet" });
            }
        }
    }
}
