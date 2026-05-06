using Microsoft.EntityFrameworkCore;
using SpendSmart.Budget.API.Data;

namespace SpendSmart.Budget.API.HostedServices
{
    public class BudgetResetWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BudgetResetWorker> _logger;

        public BudgetResetWorker(IServiceProvider serviceProvider, ILogger<BudgetResetWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Run near midnight
                var now = DateTime.UtcNow;
                
                // Extremely simple "1st of month" logic stub for worker
                if (now.Day == 1 && now.Hour == 0 && now.Minute == 0)
                {
                    _logger.LogInformation("Resetting monthly budgets at {time}", DateTimeOffset.Now);
                    
                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<BudgetDbContext>();
                    
                    // Reset all active monthly budgets
                    await dbContext.Budgets
                        .Where(b => b.Period == "MONTHLY" && b.IsActive)
                        .ExecuteUpdateAsync(s => s.SetProperty(b => b.SpentAmount, 0m), stoppingToken);
                        
                    // Simplified gamification checking could run here as well.
                }

                // Wait for a reasonable polling interval, e.g. 1 minute
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
