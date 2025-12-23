using AvyyanBackend.Data;
using AvyyanBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TallyERPWebApi.Model;

public class TallyBackgroundService : BackgroundService
{
	private readonly ILogger<TallyBackgroundService> _logger;
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly TallyService _tallyService;
	private readonly IServiceScopeFactory _serviceScopeFactory;

	public TallyBackgroundService(
		ILogger<TallyBackgroundService> logger,
		IHttpClientFactory httpClientFactory,
		TallyService tallyService,
		IServiceScopeFactory serviceScopeFactory)
	{
		_logger = logger;
		_httpClientFactory = httpClientFactory;
		_tallyService = tallyService;
		_serviceScopeFactory = serviceScopeFactory;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("Tally Background Service is starting.");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				// Call the method to fetch data from Tally
				await FetchDataFromTally();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while fetching data from Tally.");
			}

			// Wait before running the task again (e.g., 30 minutes)
			await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
		}

		_logger.LogInformation("Tally Background Service is stopping.");
	}

	private async Task FetchDataFromTally()
	{
		// Create a new scope for each execution
		using var scope = _serviceScopeFactory.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

		bool result = await _tallyService.GetTestConnection();
		if (!result)
		{
			_logger.LogWarning("Tally Server is not running");
			return;
		}

		// Path to the XML file
		string xmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TallyXML", "GetVoucher.xml");

		// Check if the XML file exists
		if (!System.IO.File.Exists(xmlFilePath))
		{
			_logger.LogWarning("The specified XML file does not exist: {FilePath}", xmlFilePath);
			return;
		}

		// Get the current company from Tally
		List<Voucher> vouchers = await _tallyService.GetVoucherAsync(xmlFilePath);

		if (vouchers == null || !vouchers.Any())
		{
			_logger.LogInformation("No vouchers found to process.");
			return;
		}
	}
}