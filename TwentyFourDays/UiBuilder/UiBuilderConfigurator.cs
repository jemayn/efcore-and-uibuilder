using TwentyFourDays.Persistence.Models;
using Umbraco.UIBuilder.Configuration;
using Umbraco.UIBuilder.Configuration.Builders;

namespace TwentyFourDays.UiBuilder;

public class UiBuilderConfigurator : IConfigurator
{
    public void Configure(UIBuilderConfigBuilder builder)
    {
        builder.AddSection("Movies", sectionConfig => sectionConfig
            .Tree(treeConfig => treeConfig
                .AddCollection<Movie>(
                    x => x.Id,
                    "Movie",
                    "Movies",
                    "List of movies",
                    "icon-movie",
                    "icon-movie",
                    collectionConfig => collectionConfig
                        .SetRepositoryType<MovieUiBuilderRepository>())));
    }
}