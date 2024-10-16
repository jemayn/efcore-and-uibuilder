using OfficeOpenXml;
using TwentyFourDays.Persistence.Models;
using TwentyFourDays.Persistence.Repositories;
using Umbraco.Cms.Core.IO;
using Umbraco.UIBuilder.Configuration.Actions;
using Umbraco.UIBuilder.Configuration.Builders;

namespace TwentyFourDays.UiBuilder.Actions;

public class MovieImportAction : Umbraco.UIBuilder.Configuration.Actions.Action<MovieImportSettings, ActionResult>
{
    private readonly MediaFileManager _mediaFileManager;
    private readonly MovieRepository _movieRepository;
    public override string Alias => "importMovies";
    public override string Name => "Import Movies";
    public override string Icon => "icon-sharing-iphone";

    public MovieImportAction(MediaFileManager mediaFileManager, MovieRepository movieRepository)
    {
        _mediaFileManager = mediaFileManager;
        _movieRepository = movieRepository;
    }
    
    public override void Configure(SettingsConfigBuilder<MovieImportSettings> settingsConfig)
    {
        settingsConfig
            .AddFieldset("Import", fieldsetConfig => fieldsetConfig
                .AddField(f => f.FilePath)
                .SetDataType(-90)
                .AddField(f => f.OverwriteExisting));
    }

    public override ActionResult Execute(string collectionAlias, object[] entityIds, MovieImportSettings? settings)
    {
        if (settings == null || string.IsNullOrWhiteSpace(settings.FilePath))
        {
            return new ActionResult(true);
        }

        var fileStream = _mediaFileManager.FileSystem.OpenFile(settings.FilePath);
        if (fileStream == null)
        {
            return new ActionResult(false);
        }

        using var package = new ExcelPackage(fileStream);

        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        if (worksheet is null)
        {
            return new ActionResult(false);
        }

        var movies = MapToMovieModel(worksheet);

        foreach (var movie in movies)
        {
            _movieRepository.InsertOrUpdate(movie).GetAwaiter().GetResult();
        }
        
        return new ActionResult(true);
    }

    private IEnumerable<Movie> MapToMovieModel(ExcelWorksheet worksheet)
    {
        var movies = new List<Movie>();
        for (var row = 2; row <= worksheet.Dimension.Rows; row++) // Row 1 is the header so we start on row 2 for the first movie
        {
            var movie = new Movie
            {
                Id = int.TryParse(worksheet.Cells[row, 1].Value?.ToString(), out var id) ? id : 0,
                Name = worksheet.Cells[row, 2].Value?.ToString() ?? "",
                ReleaseYear = int.TryParse(worksheet.Cells[row, 3].Value?.ToString(), out var releaseYear) ? releaseYear : 0,
                MainActor = worksheet.Cells[row, 4].Value?.ToString(),
                Genres = GetGenres(worksheet.Cells[row, 5].Value?.ToString())
            };

            movies.Add(movie);
        }

        return movies;
    }

    private List<MovieGenre> GetGenres(string? genres)
    {
        var movieGenres = new List<MovieGenre>();
        
        if (string.IsNullOrWhiteSpace(genres))
        {
            return movieGenres;
        }
        
        var genreArray = genres.Split(',');

        foreach (var genre in genreArray)
        {
            movieGenres.Add(new MovieGenre
            {
                Name = genre.Trim()
            });
        }

        return movieGenres;
    }
}

public class MovieImportSettings
{
    public string FilePath { get; set; }
    public bool OverwriteExisting { get; set; }
}