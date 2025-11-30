using Microsoft.AspNetCore.Mvc;
using TallyERPWebApi.Model;
using TallyERPWebApi.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TallyERPWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockItemController : ControllerBase
    {
        private readonly ILogger<StockItemController> _logger;
        private readonly TallyService _tallyService;
        private readonly PostTallyService _postTallyService;

        public StockItemController(ILogger<StockItemController> logger, TallyService tallyService, PostTallyService postTallyService)
        {
            _logger = logger;
            _tallyService = tallyService;
            _postTallyService = postTallyService;
        }
        
        // GET: api/StockItem
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
                string xmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TallyXML", "GetStockItem.xml");

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
                
                // Get stock items from Tally
                List<StockItem> stockItems = await _tallyService.GetStockItem(xmlFilePath);

                _logger.LogInformation("Successfully fetched stock items from Tally. Count: {Count}", stockItems.Count);

                // Return success response with stock items data
                return Ok(new ApiResponse<List<StockItem>>
                {
                    Success = true,
                    Message = "Stock items fetched successfully.",
                    Data = stockItems
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching stock items from Tally.");
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "An internal server error occurred. Please try again later."
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Savestockitem(StockItem stockItem)
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
                string xmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TallyXML", "CreateStockItem.xml");

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
                string response = await _postTallyService.SaveStockItem(xmlFilePath, stockItem);

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