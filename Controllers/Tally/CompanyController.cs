using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using TallyERPWebApi.Model;

namespace TallyERPWebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CompanyController : ControllerBase
	{
		private readonly ILogger<CompanyController> _logger;
		private readonly TallyService _tallyService;

		public CompanyController(ILogger<CompanyController> logger, TallyService tallyService)
		{
			_logger = logger;
			_tallyService = tallyService;
		}

		[HttpGet]
		public async Task<IActionResult> Get(){
		
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
				string xmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TallyXML", "GetCurrentComapny.xml");

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
				 List<string> currentCompany = await _tallyService.GetOpenCompaniesAsync(xmlFilePath);

                // Check if the result is valid
                /*// Check if any companies are found
                if (currentCompany == null || !currentCompany.Any())
                {
                    _logger.LogWarning("No open companies found in Tally.");
                    return NotFound(new ApiResponse<List<string>>
                    {
                        Success = false,
                        Message = "No open companies found in Tally."
                    });
                }*/

                _logger.LogInformation("Successfully fetched current company: {CurrentCompany}", currentCompany);

                // Return success response with company data
                return Ok(new ApiResponse<List<string>>
                {
                    Success = true,
                    Message = "Open companies fetched successfully.",
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
