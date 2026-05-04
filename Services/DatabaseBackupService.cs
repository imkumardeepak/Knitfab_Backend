using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AvyyanBackend.Services
{
    public class DatabaseBackupService : BackgroundService
    {
        private readonly ILogger<DatabaseBackupService> _logger;
        private readonly IConfiguration _configuration;

        public DatabaseBackupService(ILogger<DatabaseBackupService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Database Backup Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var backupSettings = _configuration.GetSection("BackupSettings");
                    var backupTimeStr = backupSettings["BackupTime"] ?? "01:00:00";
                    
                    if (!TimeSpan.TryParse(backupTimeStr, out var backupTime))
                    {
                        _logger.LogError("Invalid BackupTime format in configuration. Using default 01:00 AM.");
                        backupTime = new TimeSpan(1, 0, 0);
                    }

                    var now = DateTime.Now;
                    var nextBackupDateTime = now.Date.Add(backupTime);

                    if (now > nextBackupDateTime)
                    {
                        nextBackupDateTime = nextBackupDateTime.AddDays(1);
                    }

                    var delay = nextBackupDateTime - now;
                    _logger.LogInformation("Next database backup scheduled at: {Time}. Waiting {Delay} hours.", nextBackupDateTime, delay.TotalHours);

                    await Task.Delay(delay, stoppingToken);

                    if (!stoppingToken.IsCancellationRequested)
                    {
                        await PerformBackupAsync();
                    }
                }
                catch (TaskCanceledException)
                {
                    // Task was cancelled, exit loop
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while scheduling or performing the database backup.");
                    // Wait for an hour before retrying in case of an error to prevent tight loops
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
            }

            _logger.LogInformation("Database Backup Service is stopping.");
        }

        private async Task PerformBackupAsync()
        {
            _logger.LogInformation("Starting database backup process...");

            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    _logger.LogError("DefaultConnection not found in configuration. Cannot perform backup.");
                    return;
                }

                // Extract connection details from string
                // Example format: Host=localhost;Database=AvyyanKnitfab_Dev;Username=postgres;Password=system;Port=5432;
                var dict = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries)
                                           .Select(p => p.Split('=', 2))
                                           .ToDictionary(p => p[0].Trim(), p => p[1].Trim(), StringComparer.OrdinalIgnoreCase);

                if (!dict.TryGetValue("Host", out var host) ||
                    !dict.TryGetValue("Database", out var database) ||
                    !dict.TryGetValue("Username", out var username) ||
                    !dict.TryGetValue("Password", out var password))
                {
                    _logger.LogError("Connection string is missing required parameters (Host, Database, Username, Password).");
                    return;
                }

                if (!dict.TryGetValue("Port", out var port))
                {
                    port = "5432";
                }

                var backupSettings = _configuration.GetSection("BackupSettings");
                var backupDir = backupSettings["BackupDirectory"] ?? "Data/Backups";
                var pgDumpPath = backupSettings["PgDumpPath"] ?? "pg_dump";

                if (!Directory.Exists(backupDir))
                {
                    Directory.CreateDirectory(backupDir);
                }

                var dateStr = DateTime.Now.ToString("dd-MM-yyyy");
                var fileName = $"{database}_{dateStr}.sql";
                var filePath = Path.Combine(backupDir, fileName);

                var processStartInfo = new ProcessStartInfo
                {
                    FileName = pgDumpPath,
                    Arguments = $"-h {host} -p {port} -U {username} -F c -b -v -f \"{filePath}\" {database}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                // Provide password to pg_dump via environment variable
                processStartInfo.EnvironmentVariables["PGPASSWORD"] = password;

                using var process = new Process { StartInfo = processStartInfo };
                
                process.Start();

                // Read output and error asynchronously
                var outputTask = process.StandardOutput.ReadToEndAsync();
                var errorTask = process.StandardError.ReadToEndAsync();

                // Wait for process to exit with a timeout (e.g., 1 hour)
                if (process.WaitForExit(3600000))
                {
                    await Task.WhenAll(outputTask, errorTask);
                    var errorOutput = await errorTask;

                    if (process.ExitCode == 0)
                    {
                        _logger.LogInformation("Database backup completed successfully. Saved to: {FilePath}", filePath);
                    }
                    else
                    {
                        _logger.LogError("Database backup process failed with exit code {ExitCode}. Error details: {ErrorOutput}", process.ExitCode, errorOutput);
                    }
                }
                else
                {
                    _logger.LogError("Database backup process timed out after 1 hour. Terminating process.");
                    process.Kill();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during the backup execution.");
            }
        }
    }
}
