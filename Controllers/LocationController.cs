using Microsoft.AspNetCore.Mvc;
using AvyyanBackend.DTOs.Location;
using AvyyanBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace AvyyanBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;
        private readonly ILogger<LocationController> _logger;

        public LocationController(ILocationService locationService, ILogger<LocationController> logger)
        {
            _locationService = locationService;
            _logger = logger;
        }

        /// <summary>
        /// Get all locations
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocationResponseDto>>> GetLocations()
        {
            try
            {
                var locations = await _locationService.GetAllLocationsAsync();
                return Ok(locations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting locations");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get location by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<LocationResponseDto>> GetLocation(int id)
        {
            try
            {
                var location = await _locationService.GetLocationByIdAsync(id);
                if (location == null)
                {
                    return NotFound($"Location with ID {id} not found");
                }
                return Ok(location);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting location {LocationId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Search locations by various criteria
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<LocationResponseDto>>> SearchLocations(
            [FromQuery] string? warehousename,
            [FromQuery] string? location,
            [FromQuery] string? sublocation,
            [FromQuery] string? locationcode,
            [FromQuery] bool? isActive)
        {
            try
            {
                var searchDto = new LocationSearchRequestDto
                {
                    Warehousename = warehousename,
                    Location = location,
                    Sublocation = sublocation,
                    Locationcode = locationcode,
                    IsActive = isActive
                };
                var locations = await _locationService.SearchLocationsAsync(searchDto);
                return Ok(locations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching locations");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Create a new location
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<LocationResponseDto>> CreateLocation(CreateLocationRequestDto createLocationDto)
        {
            try
            {
                var location = await _locationService.CreateLocationAsync(createLocationDto);
                return CreatedAtAction(nameof(GetLocation), new { id = location.Id }, location);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating location");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Update a location
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<LocationResponseDto>> UpdateLocation(int id, UpdateLocationRequestDto updateLocationDto)
        {
            try
            {
                var location = await _locationService.UpdateLocationAsync(id, updateLocationDto);
                if (location == null)
                {
                    return NotFound($"Location with ID {id} not found");
                }
                return Ok(location);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating location {LocationId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Delete a location (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLocation(int id)
        {
            try
            {
                var result = await _locationService.DeleteLocationAsync(id);
                if (!result)
                {
                    return NotFound($"Location with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting location {LocationId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}