using EpsiCodeAPI.Hubs;
using EpsiCodeAPI.Services;
using Microsoft.AspNetCore.SignalR;

namespace EpsiCodeAPI.Jobs
{
    public class BookSyncJob : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BookSyncJob> _logger;
        private readonly IHubContext<SyncHub> _hubContext;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(2);

        public static string LastRunTime { get; private set; } = "Never";

        public BookSyncJob(IServiceScopeFactory scopeFactory, ILogger<BookSyncJob> logger, IHubContext<SyncHub> hubContext)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Book Sync Job started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Starting automatic book fetch at: {time}", DateTimeOffset.Now);

                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var bookService = scope.ServiceProvider.GetRequiredService<BookService>();
                        await bookService.SyncBooksAsync();
                    }

                    _logger.LogInformation("Automatic fetch completed successfully.");


                    LastRunTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");

                    await _hubContext.Clients.All.SendAsync("UpdateSyncStatus", LastRunTime, stoppingToken);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during automatic book fetch.");
                }

                // Wait for the next interval
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
