using AvyyanBackend.Data;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TallyERPWebApi.Model;
using TallyERPWebApi.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TallyERPWebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class VouchersController : ControllerBase
	{
		private readonly ILogger<VouchersController> _logger;
		private readonly TallyService _tallyService;
		private readonly PostTallyService _postTallyService;
		private readonly ApplicationDbContext _context;
		private readonly ISalesOrderService _salesOrderService;
		
		public VouchersController(
			ILogger<VouchersController> logger, 
			TallyService tallyService, 
			PostTallyService postTallyService, 
			ApplicationDbContext context,
			ISalesOrderService salesOrderService)
		{
			_logger = logger;
			_tallyService = tallyService;
			_postTallyService = postTallyService;
			_context = context;
			_salesOrderService = salesOrderService;
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
				string xmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TallyXML", "GetVoucher.xml");

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

				List<Voucher> vouchers = await _tallyService.GetVoucherAsync(xmlFilePath);

				if (vouchers == null || !vouchers.Any())
				{
					_logger.LogInformation("No vouchers found to process.");
					return NotFound(new ApiResponse<string>
					{ 
					 Data = "No vouchers found to process.",
					Success = false,
					Message = "No vouchers found to process."
					});
				}

				// Get existing voucher numbers in bulk for faster checking
				//var existingVoucherNumbers = await _context.SalesOrders
				//	.Where(so => vouchers.Select(v => v.VoucherNumber).Contains(so.VoucherNumber))
				//	.Select(so => so.VoucherNumber)
				//	.ToListAsync();

				//var newVouchers = new List<SalesOrder>();
				//int processedCount = 0;
				//int skippedCount = 0;

				//foreach (var voucher in vouchers)
				//{
				//	// Skip if voucher number already exists
				//	if (existingVoucherNumbers.Contains(voucher.VoucherNumber))
				//	{
				//		skippedCount++;
				//		continue;
				//	}

				//	var dbVoucher = VoucherMapper.MapToDatabaseModel(voucher);
				//	if (dbVoucher != null)
				//	{
				//		newVouchers.Add(dbVoucher);
				//		existingVoucherNumbers.Add(dbVoucher.VoucherNumber); // Prevent duplicates in same batch
				//	}
					
				//}

				//if (newVouchers.Any())
				//{
				//	await _context.SalesOrders.AddRangeAsync(newVouchers);
				//	var savedCount = await _context.SaveChangesAsync();
				//	processedCount = savedCount;
				//}

				//_logger.LogInformation("Voucher processing completed. Processed: {Processed}, Skipped: {Skipped}, Total: {Total}",
				//	processedCount, skippedCount, vouchers.Count);

				// Return success response with company data
				return Ok(new ApiResponse<List<Voucher>>
				{
					Success = true,
					Message = $"Voucher processing completed. Processed: , Total: {vouchers.Count}",
					Data = vouchers
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while fetching company information from Tally.");
				return StatusCode(500, new ApiResponse<string>
				{
					Success = false,
					Message = ex.ToString()
				});
			}
		}

        [Route("GetVoucherTypeData")]
        [HttpGet]
        public async Task<IActionResult> GetVoucherTypeData()
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
                string xmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TallyXML", "GetVoucherTypeData.xml");

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
                List<getVouchers> currentCompany = await _tallyService.GetVoucherTypeData(xmlFilePath);

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
                return Ok(new ApiResponse<List<getVouchers>>
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

        [Route("GetInvoiceNo")]
		[HttpGet]
		public async Task<IActionResult> GetInvoiceNo(string vouchername)
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
				string xmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TallyXML", "GetInvoiceNo.xml");

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
				List<string> group = await _tallyService.GetInvoiceNo(xmlFilePath,vouchername);

				// Check if the result is valid
				if (group.Count == 0)
				{
					_logger.LogWarning("The current company returned by Tally is null or empty.");
					return NotFound(new ApiResponse<string>
					{
						Success = false,
						Message = "No current company found in Tally."
					});
				}

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

        [Route("GetVoucherType")]
		[HttpGet]
		public async Task<IActionResult> GetVoucherType()
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
				string xmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TallyXML", "GetVoucherType.xml");

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
				List<string> group = await _tallyService.GetVoucherType(xmlFilePath);

				// Check if the result is valid
				if (group.Count == 0)
				{
					_logger.LogWarning("The current company returned by Tally is null or empty.");
					return NotFound(new ApiResponse<string>
					{
						Success = false,
						Message = "No current company found in Tally."
					});
				}

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

        [Route("SaveVoucher")]
        [HttpPost]
		public async Task<IActionResult> SaveVoucher(VoucherSO voucher)
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
				string xmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TallyXML", "CreateVoucher.txt");

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
				string response = await _postTallyService.SaveVoucher(xmlFilePath, voucher);

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

        [Route("SavePO_Order")]
        [HttpPost]
		public async Task<IActionResult> SavePO_Order(POorderVoucher voucher)
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
				string xmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TallyXML", "Create_PO_ORDER.xml");

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
				string response = await _postTallyService.SavePO_Order(xmlFilePath, voucher);

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

        [Route("SaveSO_Order")]
        [HttpPost]
		public async Task<IActionResult> SaveSO_Order(SOorderVoucher voucher)
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
				string xmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TallyXML", "Create_SO_ORDER.xml");

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
				string response = await _postTallyService.SaveSO_Order(xmlFilePath, voucher);

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

        [Route("SaveSO_OrderInvoice")]
        [HttpPost]
        public async Task<IActionResult> SaveSO_OrderInvoice(Invoice voucher)
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
                string xmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TallyXML", "CreateSO_INVOICE.xml");

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
                string response = await _postTallyService.SaveSO_OrderInvoice(xmlFilePath, voucher);

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

        [Route("SavePO_OrderInvoice")]
        [HttpPost]
        public async Task<IActionResult> SavePO_OrderInvoice(Invoice voucher)
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
                string xmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TallyXML", "CreatePO_INVOICE.xml");

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
                string response = await _postTallyService.SavePO_OrderInvoice(xmlFilePath, voucher);

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

        [Route("Save_Delievery")]
        [HttpPost]
        public async Task<IActionResult> Save_Delievery(Invoice voucher)
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
                string xmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TallyXML", "Save_Delievery.xml");

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
                string response = await _postTallyService.Save_Delievery(xmlFilePath, voucher);

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
        
        //[Route("Save_Invoice")]
        //[HttpPost]
        //public async Task<IActionResult> Save_Invoice(Invoice voucher)
        //{
        //    try
        //    {
        //        bool result = await _tallyService.GetTestConnection();
        //        if (!result)
        //        {
        //            _logger.LogWarning("Tally Server is not running");
        //            return NotFound(new ApiResponse<string>
        //            {
        //                Success = false,
        //                Message = "Tally Server is not running!!!"
        //            });
        //        }

        //        // Path to the XML file
        //        string xmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TallyXML", "Invoice.xml");

        //        // Check if the XML file exists
        //        if (!System.IO.File.Exists(xmlFilePath))
        //        {
        //            _logger.LogWarning("The specified XML file does not exist: {FilePath}", xmlFilePath);
        //            return NotFound(new ApiResponse<string>
        //            {
        //                Success = false,
        //                Message = "The specified XML file does not exist."
        //            });
        //        }
        //        // Get the current company from Tally
        //        string response = await _postTallyService.Save_Invoice(xmlFilePath, voucher);

        //        // Check if the result is valid
        //        if (string.IsNullOrEmpty(response))
        //        {
        //            _logger.LogWarning("The current company returned by Tally is null or empty.");
        //            return NotFound(new ApiResponse<string>
        //            {
        //                Success = false,
        //                Message = "No current company found in Tally."
        //            });
        //        }

        //        _logger.LogInformation("Successfully fetched current company: {CurrentCompany}", response);

        //        // Return success response with company data
        //        return Ok(new
        //        {
        //            Success = true,
        //            Message = "Current company fetched successfully.",
        //            Data = response
        //        });
        //    }
        //    catch (Exception ex)
        //    {

        //        _logger.LogError(ex, "An error occurred while fetching company information from Tally.");
        //        return StatusCode(500, new ApiResponse<string>
        //        {
        //            Success = false,
        //            Message = "An internal server error occurred. Please try again later."
        //        });
        //    }
        //}

    }
}