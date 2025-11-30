using Microsoft.AspNetCore.Mvc;
using TallyERPWebApi.Model;
using TallyERPWebApi.Service;


namespace TallyERPWebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AllLedgerController : ControllerBase
	{
		private readonly ILogger<AllLedgerController> _logger;
		private readonly TallyService _tallyService;
		private PostTallyService _postTallyService;

		public AllLedgerController(ILogger<AllLedgerController> logger, TallyService tallyService, PostTallyService postTallyService)
		{
			_logger = logger;
			_tallyService = tallyService;
			_postTallyService = postTallyService;
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

				// Path to the XML file
				string xmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TallyXML", "GetLedger.xml");

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
				List<Ledger> currentCompany = await _tallyService.GetAllLedger(xmlFilePath);

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
				return Ok(new ApiResponse<List<Ledger>>
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

		[Route("GetCustomerData")]
        [HttpGet]
        public async Task<IActionResult> GetCustomerData()
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
                string xmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TallyXML", "GetCustomer.xml");

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
                List<Ledger> currentCompany = await _tallyService.GetCustomerData(xmlFilePath);

                // Check if the result is valid
                if (currentCompany.Count == 0)
                {
                    _logger.LogWarning("The current company returned by Tally is null or empty.");
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "No current company found in Tally."
                    });
                }

                _logger.LogInformation("Successfully fetched current company: {CurrentCompany}", currentCompany);

                // Return success response with company data
                return Ok(new ApiResponse<List<Ledger>>
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

        [Route("GetSupplierData")]
        [HttpGet]
        public async Task<IActionResult> GetSupplierData()
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
                string xmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TallyXML", "GetSupplier.xml");

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
                List<Ledger> currentCompany = await _tallyService.GetSupplierData(xmlFilePath);

                // Check if the result is valid
                if (currentCompany.Count == 0)
                {
                    _logger.LogWarning("The current company returned by Tally is null or empty.");
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "No current company found in Tally."
                    });
                }

                _logger.LogInformation("Successfully fetched current company: {CurrentCompany}", currentCompany);

                // Return success response with company data
                return Ok(new ApiResponse<List<Ledger>>
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

        [Route("GetClosingBalace")]
        [HttpGet]
        public async Task<IActionResult> GetClosingBalace(string customername)
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

                string xmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TallyXML", "checkClosingBalace.xml");

                if (!System.IO.File.Exists(xmlFilePath))
                {
                    _logger.LogWarning("The specified XML file does not exist: {FilePath}", xmlFilePath);
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "The specified XML file does not exist."
                    });
                }

                // Call GetClosingBalace and expect a double value
                double closingBalance = await _tallyService.GetClosingBalace(xmlFilePath, customername);

                string val = closingBalance.ToString();

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = "Closing balance fetched successfully.",
                    Data = val
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

/*************  ✨ Windsurf Command 🌟  *************/
        [Route("GetSupplier")]
        [HttpGet]
        public async Task<IActionResult> GetSupplier()
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
                string xmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TallyXML", "GetSupplier.xml");

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
                List<string> currentCompany = await _tallyService.GetSuppliers(xmlFilePath);

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
/*******  7dfa7842-3b64-41f4-9fb4-053130b08f5e  *******/

            [Route("GetCustomer")]
            [HttpGet]
            public async Task<IActionResult> GetCustomer()
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
                    string xmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TallyXML", "GetCustomer.xml");

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
                    List<string> currentCompany = await _tallyService.GetCustomers(xmlFilePath);

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

		[HttpPost]
		public async Task<IActionResult> SaveLedger(Ledger ledger)
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
				string xmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TallyXML", "CreateLedeger.xml");

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
				string response = await _postTallyService.SaveLedeger(xmlFilePath, ledger);

				/*// Check if the result is valid
				if (string.IsNullOrEmpty(response))
				{
					_logger.LogWarning("The current company returned by Tally is null or empty.");
					return NotFound(new ApiResponse<string>
					{
						Success = false,
						Message = "No current company found in Tally."
					});
				}*/

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
