﻿using TwentyFourDays.Persistence.Models;
using Umbraco.UIBuilder;
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
                        .SetRepositoryType<MovieUiBuilderRepository>()

                        .SetNameProperty(x => x.Name)

                        .ListView(listViewConfig => listViewConfig
                            .AddField(x => x.ReleaseYear)
                            .AddField(x => x.MainActor)
                            .SetPageSize(25))
                        
                        .Editor(editorConfig => editorConfig
                            .AddTab("Content", tabConfig => tabConfig
                                .AddFieldset("General", fieldsetConfig => fieldsetConfig
                                    .AddField(x => x.ReleaseYear)
                                    .AddField(x => x.MainActor)
                                    .AddField(x => x.Genres)
                                        .SetDataType("Movie Genre List Picker")
                                        .SetValueMapper<MovieGenreValueMapper>())))
                        
                        .AddSearchableProperty(x => x.MainActor)
                        .SetSortProperty(x =>x.ReleaseYear, SortDirection.Descending)
                    )));
    }
}