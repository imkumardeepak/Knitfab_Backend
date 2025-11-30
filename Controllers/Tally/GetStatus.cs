using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TallyERPWebApi.Model;

namespace TallyERPWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetStatus : ControllerBase
    {
        private readonly ILogger<GetStatus> _logger;
        private readonly TallyService _tallyService;

        public GetStatus(ILogger<GetStatus> logger, TallyService tallyService)
        {
            _logger = logger;
            _tallyService = tallyService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                bool result = await _tallyService.GetTestConnection();
                if (!result)
                {
                    _logger.LogWarning("Tally Server is not running");
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Tally Server is not running!!!"
                    });
                }

                // Return success response with company data
                return Ok(new ApiResponse<List<string>>
                {
                    Success = true,
                    Message = "Tally Server is running!!!",                   
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching company information from Tally.");
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "An internal server error occurred. Please try again later."
                });
            }

        }
    }
}
