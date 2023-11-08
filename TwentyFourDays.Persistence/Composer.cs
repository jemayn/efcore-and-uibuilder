using Microsoft.Extensions.DependencyInjection;
using TwentyFourDays.Persistence.Services;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace TwentyFourDays.Persistence;

public class Composer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddTransient<MoviesImportService>();
    }
}