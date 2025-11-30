using Microsoft.AspNetCore.Mvc;
using TallyERPWebApi.Model;
using TallyERPWebApi.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TallyERPWebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class StockGroupController : ControllerBase
	{
		private readonly ILogger<StockGroupController> _logger;
		private readonly TallyService _tallyService;
		private readonly PostTallyService _postTallyService;

		public StockGroupController(ILogger<StockGroupController> logger, TallyService tallyService, PostTallyService postTallyService)
		{
			_logger = logger;
			_tallyService = tallyService;
			_postTallyService = postTallyService;
		}
		// GET: api/<GetStockGroupController>
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

				// Path to the XML file
				string xmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TallyXML", "GetStockGroup.xml");

				// Check if the XML file exists
				if (!System.IO.File.Exists(xmlFilePath))
				{
					_logger.LogWarning("The specified XML file does not exist: {FilePath}", xmlFilePath);
					return NotFound(new ApiResponse<string>
					{
						Success = false,
						Message = "The specified XML file does not exist."
					});
				}

				// Get the current company from Tally
				List<string> group = await _tallyService.GetStockGroup(xmlFilePath);

				/*// Check if the result is valid
				if (group.Count == 0)
				{
					_logger.LogWarning("The current company returned by Tally is null or empty.");
					return NotFound(new ApiResponse<string>
					{
						Success = true,
						Message = "No current company found in Tally."
						//Data = group.Count
					});
				}*/

				_logger.LogInformation("Successfully fetched current company: {CurrentCompany}", group);

				// Return success response with company data
				return Ok(new ApiResponse<List<string>>
				{
					Success = true,
					Message = "Current company fetched successfully.",
					Data = group
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

		[HttpPost]
		public async Task<IActionResult> Savestockgroup(string stockgroup)
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

				// Path to the XML file
				string xmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TallyXML", "CreateStockGroup.xml");

				// Check if the XML file exists
				if (!System.IO.File.Exists(xmlFilePath))
				{
					_logger.LogWarning("The specified XML file does not exist: {FilePath}", xmlFilePath);
					return NotFound(new ApiResponse<string>
					{
						Success = false,
						Message = "The specified XML file does not exist."
					});
				}
				// Get the current company from Tally
				string response = await _postTallyService.SaveStockGroup(xmlFilePath, stockgroup);

				// Check if the result is valid
				if (string.IsNullOrEmpty(response))
				{
					_logger.LogWarning("The current company returned by Tally is null or empty.");
					return NotFound(new ApiResponse<string>
					{
						Success = false,
						Message = "No current company found in Tally."
					});
				}

				_logger.LogInformation("Successfully fetched current company: {CurrentCompany}", response);

				// Return success response with company data
				return Ok(new
				{
					Success = true,
					Message = "Current company fetched successfully.",
					Data = response
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
