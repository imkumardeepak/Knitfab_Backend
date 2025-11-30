using AvyyanBackend.Data;
using AvyyanBackend.DTOs.Machine;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models.ProAllot;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace AvyyanBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MachineController : ControllerBase
    {
        private readonly IMachineManagerService _machineManagerService;
        private readonly ILogger<MachineController> _logger;
       
        private readonly IConfiguration _configuration;

        public MachineController(IMachineManagerService machineManagerService, ILogger<MachineController> logger, ApplicationDbContext context, IConfiguration configuration)
        {
            _machineManagerService = machineManagerService;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Get all machines
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MachineResponseDto>>> GetMachines()
        {
            try
            {
                var machines = await _machineManagerService.GetAllMachinesAsync();
                return Ok(machines);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting machines");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get machine by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<MachineResponseDto>> GetMachine(int id)
        {
            try
            {
                var machine = await _machineManagerService.GetMachineByIdAsync(id);
                if (machine == null)
                {
                    return NotFound($"Machine with ID {id} not found");
                }
                return Ok(machine);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting machine {MachineId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Search machines by name and/or dia
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<MachineResponseDto>>> SearchMachines(
            [FromQuery] string? machineName,
            [FromQuery] decimal? dia,
            [FromQuery] bool? isActive)
        {
            try
            {
                var searchDto = new MachineSearchRequestDto
                {
                    MachineName = machineName,
                    Dia = dia,
                    IsActive = isActive
                };
                var machines = await _machineManagerService.SearchMachinesAsync(searchDto);
                return Ok(machines);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching machines");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Create a new machine
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<MachineResponseDto>> CreateMachine(CreateMachineRequestDto createMachineDto)
        {
            try
            {
                var machine = await _machineManagerService.CreateMachineAsync(createMachineDto);
                return CreatedAtAction(nameof(GetMachine), new { id = machine.Id }, machine);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating machine");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Update a machine
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<MachineResponseDto>> UpdateMachine(int id, UpdateMachineRequestDto updateMachineDto)
        {
            try
            {
                var machine = await _machineManagerService.UpdateMachineAsync(id, updateMachineDto);
                if (machine == null)
                {
                    return NotFound($"Machine with ID {id} not found");
                }
                return Ok(machine);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating machine {MachineId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
        /// <summary>
        /// Delete a machine (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMachine(int id)
        {
            try
            {
                var result = await _machineManagerService.DeleteMachineAsync(id);
                if (!result)
                {
                    return NotFound($"Machine with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting machine {MachineId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }




        ////  Sticker Printing Logic
        [HttpPost("generate-qr/{id}")]
        public async Task<IActionResult> GenerateQRCode(int id)
        {
            try
            {
                // Get machine details from database
                var machine = await _machineManagerService.GetMachineByIdAsync(id);

                if (machine == null)
                {
                    return NotFound($"Machine with ID {id} not found");
                }

                string filepath = Path.Combine("wwwroot", "Sticker", "MC_Sticker.prn");

                if (!System.IO.File.Exists(filepath))
                {
                    return StatusCode(500, "QR template file not found");
                }

                string printerName = _configuration["Printers:Printer_IP"];
                string fileContent = System.IO.File.ReadAllText(filepath);

                // Replace placeholders with actual machine data
                fileContent = fileContent
                    .Replace("<MCCODE>", machine.MachineName.Trim())
                    .Replace("<MCDIA>", string.Format("{0:0}", machine.Dia).Trim());

               

                // Send to printer
                var printerIp = IPAddress.Parse(printerName);
                var printerPort = 9100;

                // Check if printer is reachable
                Ping ping = new Ping();
                PingReply reply = ping.Send(printerIp, 1000);

                if (reply.Status != IPStatus.Success)
                {
                    return StatusCode(500, "Printer is not reachable");
                }

                // Send print job
                using (var client = new TcpClient())
                {
                    client.Connect(printerIp, printerPort);
                    byte[] prnData = Encoding.ASCII.GetBytes(fileContent);

                    var stream = client.GetStream();
                    await stream.WriteAsync(prnData, 0, prnData.Length);
                    await stream.FlushAsync();
                }

                // Return success response with QR code image if needed
                return Ok(new
                {
                    success = true,
                    message = $"QR code generated for {machine.MachineName}",
                  
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating QR code: {ex.Message}");
            }
        }

        /// <summary>
        /// Create multiple machines (bulk upload)
        /// </summary>
        [HttpPost("bulk")]
        public async Task<ActionResult<IEnumerable<MachineResponseDto>>> CreateMultipleMachines(BulkCreateMachineRequestDto bulkCreateDto)
        {
            try
            {
                var machines = await _machineManagerService.CreateMultipleMachinesAsync(bulkCreateDto);
                return Ok(machines);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating multiple machines");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}