using TwentyFourDays.Persistence.Repositories;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Community.Contentment.DataEditors;

namespace TwentyFourDays.Contentment;

public class GenreDataSource : IDataListSource
{
    private readonly MovieRepository _movieRepository;
    public string Name => "Movie Genre DataSource";
    public string Description => "List of genres for movies";
    public string Icon => "icon-movie-alt";
    public Dictionary<string, object> DefaultValues => default;
    public IEnumerable<ConfigurationField> Fields => default;
    public string Group => "Custom data sources";
    public OverlaySize OverlaySize => OverlaySize.Medium;

    public GenreDataSource(MovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }
    
    public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
    {
        var genres = _movieRepository.GetAllGenres().GetAwaiter().GetResult();

        var dataList = new List<DataListItem>();

        foreach (var genre in genres)
        {
            dataList.Add(new DataListItem()
            {
                Name = genre.Name,
                Value = genre.Id.ToString()
            });
        }

        return dataList;
    }
}