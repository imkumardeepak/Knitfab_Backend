using Microsoft.AspNetCore.Mvc;
using TallyERPWebApi.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TallyERPWebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountGroupController : ControllerBase
	{

		private readonly ILogger<AccountGroupController> _logger;
		private readonly TallyService _tallyService;

		public AccountGroupController(ILogger<AccountGroupController> logger, TallyService tallyService)
		{
			_logger = logger;
			_tallyService = tallyService;
		}
		// GET: api/<GetAccountGroupController>
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
				string xmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TallyXML", "GetAccountGroup.xml");

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
				List<string> currentCompany = await _tallyService.GetAccountGroup(xmlFilePath);

				/*// Check if the result is valid
				if (currentCompany.Count == 0)
				{
					_logger.LogWarning("The current company returned by Tally is null or empty.");
					return NotFound(new ApiResponse<string>
					{
						Success = false,
						Message = "No current company found in Tally."
					});
				}*/

				_logger.LogInformation("Successfully fetched current company: {CurrentCompany}", currentCompany);

				// Return success response with company data
				return Ok(new ApiResponse<List<string>>
				{
					Success = true,
					Message = "Current company fetched successfully.",
					Data = currentCompany
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
