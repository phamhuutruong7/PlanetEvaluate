using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanetEvaluateApi.Interfaces;
using PlanetEvaluateApi.Models;

namespace PlanetEvaluateApi.Controllers
{
    /// <summary>
    /// Controller for planet habitability evaluation operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HabitabilityController : ControllerBase
    {
        private readonly IHabitabilityEvaluationService _habitabilityService;
        private readonly ILogger<HabitabilityController> _logger;

        public HabitabilityController(
            IHabitabilityEvaluationService habitabilityService,
            ILogger<HabitabilityController> logger)
        {
            _habitabilityService = habitabilityService;
            _logger = logger;
        }

        /// <summary>
        /// Evaluates the habitability of a specific planet
        /// </summary>
        /// <param name="planetId">The ID of the planet to evaluate</param>
        /// <returns>Complete habitability evaluation for the planet</returns>
        [HttpGet("evaluate/{planetId}")]
        [Authorize(Roles = "SuperAdmin,PlanetAdmin,Viewer1,Viewer2")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HabitabilityEvaluation))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<HabitabilityEvaluation>> EvaluatePlanet(int planetId)
        {
            try
            {
                var userId = _habitabilityService.GetCurrentUserId(User);
                if (!userId.HasValue)
                    return Unauthorized(new { message = "Invalid user token" });

                _logger.LogInformation("Evaluating habitability for planet ID: {PlanetId} by user: {UserId}", planetId, userId.Value);

                var evaluation = await _habitabilityService.EvaluatePlanetByIdAsync(planetId, userId.Value);
                
                if (evaluation == null)
                {
                    _logger.LogWarning("Planet with ID {PlanetId} not found or access denied for user {UserId}", planetId, userId.Value);
                    return NotFound($"Planet with ID {planetId} not found or access denied.");
                }

                _logger.LogInformation("Successfully evaluated planet {PlanetName} with overall score: {Score}", 
                    evaluation.Planet.Name, evaluation.OverallScore);

                return Ok(evaluation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating planet with ID: {PlanetId}", planetId);
                return StatusCode(500, "An error occurred while evaluating the planet.");
            }
        }

        /// <summary>
        /// Ranks all accessible planets by their habitability scores
        /// </summary>
        /// <returns>List of planets ranked by habitability (best first)</returns>
        [HttpGet("rank")]
        [Authorize(Roles = "SuperAdmin,PlanetAdmin,Viewer1,Viewer2")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<HabitabilityEvaluation>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<HabitabilityEvaluation>>> RankPlanetsByHabitability()
        {
            try
            {
                var userId = _habitabilityService.GetCurrentUserId(User);
                if (!userId.HasValue)
                    return Unauthorized(new { message = "Invalid user token" });

                _logger.LogInformation("Ranking accessible planets by habitability for user: {UserId}", userId.Value);

                var rankedEvaluations = await _habitabilityService.RankAccessiblePlanetsByHabitabilityAsync(userId.Value);
                
                _logger.LogInformation("Successfully ranked {Count} planets by habitability for user {UserId}", rankedEvaluations.Count, userId.Value);

                return Ok(rankedEvaluations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ranking planets by habitability");
                return StatusCode(500, "An error occurred while ranking planets.");
            }
        }

        /// <summary>
        /// Finds the most habitable planet from all accessible planets
        /// </summary>
        /// <returns>The most habitable planet evaluation</returns>
        [HttpGet("most-habitable")]
        [Authorize(Roles = "SuperAdmin,PlanetAdmin,Viewer1,Viewer2")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HabitabilityEvaluation))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<HabitabilityEvaluation>> FindMostHabitablePlanet()
        {
            try
            {
                var userId = _habitabilityService.GetCurrentUserId(User);
                if (!userId.HasValue)
                    return Unauthorized(new { message = "Invalid user token" });

                _logger.LogInformation("Finding most habitable planet for user: {UserId}", userId.Value);

                var mostHabitable = await _habitabilityService.FindMostHabitableAccessiblePlanetAsync(userId.Value);
                
                if (mostHabitable == null)
                {
                    _logger.LogInformation("No accessible planets available for evaluation for user {UserId}", userId.Value);
                    return NotFound("No accessible planets available for habitability evaluation.");
                }

                _logger.LogInformation("Most habitable planet found: {PlanetName} with score: {Score} for user {UserId}", 
                    mostHabitable.Planet.Name, mostHabitable.OverallScore, userId.Value);

                return Ok(mostHabitable);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding most habitable planet");
                return StatusCode(500, "An error occurred while finding the most habitable planet.");
            }
        }

        /// <summary>
        /// Evaluates habitability for a list of specific planet IDs (if accessible to user)
        /// </summary>
        /// <param name="planetIds">List of planet IDs to evaluate</param>
        /// <returns>Batch evaluation result with successful evaluations and failed IDs</returns>
        [HttpPost("evaluate-batch")]
        [Authorize(Roles = "SuperAdmin,PlanetAdmin,Viewer1,Viewer2")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BatchEvaluationResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BatchEvaluationResult>> EvaluatePlanetsBatch([FromBody] List<int> planetIds)
        {
            try
            {
                if (planetIds == null || !planetIds.Any())
                {
                    return BadRequest("Planet IDs list cannot be empty.");
                }

                if (planetIds.Count > 50) // Reasonable limit
                {
                    return BadRequest("Cannot evaluate more than 50 planets at once.");
                }

                var userId = _habitabilityService.GetCurrentUserId(User);
                if (!userId.HasValue)
                    return Unauthorized(new { message = "Invalid user token" });

                _logger.LogInformation("Batch evaluating {Count} planets for user: {UserId}", planetIds.Count, userId.Value);

                var result = await _habitabilityService.EvaluatePlanetsBatchAsync(planetIds.Distinct().ToList(), userId.Value);

                if (result.NotFoundIds.Any())
                {
                    _logger.LogWarning("Some planets not found or access denied during batch evaluation for user {UserId}: {NotFoundIds}", 
                        userId.Value, string.Join(", ", result.NotFoundIds));
                }

                _logger.LogInformation("Successfully evaluated {Count} planets in batch for user {UserId}", result.Evaluations.Count, userId.Value);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during batch planet evaluation");
                return StatusCode(500, "An error occurred during batch planet evaluation.");
            }
        }

        /// <summary>
        /// Gets habitability factor scores for a specific planet without full evaluation
        /// </summary>
        /// <param name="planetId">The ID of the planet to score</param>
        /// <returns>Individual factor scores breakdown</returns>
        [HttpGet("scores/{planetId}")]
        [Authorize(Roles = "SuperAdmin,PlanetAdmin,Viewer1,Viewer2")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HabitabilityFactorScores))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<HabitabilityFactorScores>> GetPlanetFactorScores(int planetId)
        {
            try
            {
                var userId = _habitabilityService.GetCurrentUserId(User);
                if (!userId.HasValue)
                    return Unauthorized(new { message = "Invalid user token" });

                _logger.LogInformation("Getting factor scores for planet ID: {PlanetId} by user: {UserId}", planetId, userId.Value);

                var factorScores = await _habitabilityService.GetPlanetFactorScoresAsync(planetId, userId.Value);
                
                if (factorScores == null)
                {
                    _logger.LogWarning("Planet with ID {PlanetId} not found or access denied for user {UserId}", planetId, userId.Value);
                    return NotFound($"Planet with ID {planetId} not found or access denied.");
                }

                _logger.LogInformation("Successfully calculated factor scores for planet ID: {PlanetId}", planetId);

                return Ok(factorScores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating factor scores for planet with ID: {PlanetId}", planetId);
                return StatusCode(500, "An error occurred while calculating factor scores.");
            }
        }
    }
}
