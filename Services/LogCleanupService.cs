using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AvyyanBackend.Services
{
    public class LogCleanupService : BackgroundService
    {
        private readonly ILogger<LogCleanupService> _logger;
        private readonly string _logDirectory;

        public LogCleanupService(ILogger<LogCleanupService> logger)
        {
            _logger = logger;
            _logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Log Cleanup Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupOldLogFiles();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while cleaning up log files.");
                }

                // Wait for 8 hour before running the cleanup again
                await Task.Delay(TimeSpan.FromHours(8), stoppingToken);
            }

            _logger.LogInformation("Log Cleanup Service is stopping.");
        }

        private async Task CleanupOldLogFiles()
        {
            _logger.LogInformation("Starting log file cleanup process.");

            if (!Directory.Exists(_logDirectory))
            {
                _logger.LogWarning("Log directory does not exist: {LogDirectory}", _logDirectory);
                return;
            }

            var logFiles = Directory.GetFiles(_logDirectory, "*.log");
            var cutoffDate = DateTime.Now.AddDays(-2);
            var deletedCount = 0;

            foreach (var file in logFiles)
            {
                try
                {
                    var fileInfo = new FileInfo(file);
                    
                    // Skip if file is newer than 2 days
                    if (fileInfo.LastWriteTime >= cutoffDate)
                        continue;

                    // Extract date from filename if possible
                    var dateFromFilename = ExtractDateFromFilename(fileInfo.Name);
                    if (dateFromFilename.HasValue && dateFromFilename.Value >= cutoffDate)
                        continue;

                    // Delete the file
                    File.Delete(file);
                    _logger.LogInformation("Deleted old log file: {FileName}", fileInfo.Name);
                    deletedCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to delete log file: {FileName}", file);
                }
            }

            _logger.LogInformation("Log file cleanup completed. Deleted {DeletedCount} files.", deletedCount);
        }

        private DateTime? ExtractDateFromFilename(string filename)
        {
            // Try to extract date from filename pattern like "avyyan-knitfab-20250828.log"
            var datePattern = @"(\d{8})"; // Matches 8 consecutive digits (YYYYMMDD)
            var match = Regex.Match(filename, datePattern);
            
            if (match.Success && DateTime.TryParseExact(match.Value, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var date))
            {
                return date;
            }

            return null;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Log Cleanup Service is stopping.");
            await base.StopAsync(cancellationToken);
        }
    }
}