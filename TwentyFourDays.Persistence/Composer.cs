using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TwentyFourDays.Persistence.DbContexts;
using TwentyFourDays.Persistence.Migrations;
using TwentyFourDays.Persistence.Services;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;

namespace TwentyFourDays.Persistence;

public class Composer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddUmbracoDbContext<MovieContext>((serviceProvider, options) =>
        {
            options.UseUmbracoDatabaseProvider(serviceProvider);
        });

        builder.AddNotificationAsyncHandler<UmbracoApplicationStartedNotification, MovieMigration>();
        
        builder.Services.AddTransient<MoviesImportService>();
    }
}