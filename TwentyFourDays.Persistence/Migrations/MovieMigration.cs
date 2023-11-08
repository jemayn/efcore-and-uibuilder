using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TwentyFourDays.Persistence.DbContexts;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace TwentyFourDays.Persistence.Migrations;

public class MovieMigration : INotificationAsyncHandler<UmbracoApplicationStartedNotification>
{
    private readonly MovieContext _movieContext;
    private readonly ILogger<MovieMigration> _logger;

    public MovieMigration(MovieContext movieContext, ILogger<MovieMigration> logger)
    {
        _movieContext = movieContext;
        _logger = logger;
    }
    
    public async Task HandleAsync(UmbracoApplicationStartedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting movie migrations");
        var pendingMigrations = await _movieContext.Database.GetPendingMigrationsAsync(cancellationToken);
        
        if (pendingMigrations.Any())
        {
            _logger.LogInformation($"Found pending movie migrations: {string.Join(',', pendingMigrations)}");
            await _movieContext.Database.MigrateAsync(cancellationToken);
        }
        _logger.LogInformation("Completed movie migrations");
    }
}