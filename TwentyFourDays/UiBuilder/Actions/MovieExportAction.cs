using OfficeOpenXml;
using TwentyFourDays.Persistence.Models;
using TwentyFourDays.Persistence.Repositories;
using TwentyFourDays.UiBuilder.Models;
using Umbraco.UIBuilder.Configuration.Actions;

namespace TwentyFourDays.UiBuilder.Actions;

public class MovieExportAction : Umbraco.UIBuilder.Configuration.Actions.Action<FileActionResult>
{
    private readonly MovieRepository _movieRepository;
    public override string Alias => "exportMovies";
    public override string Name => "Export Movies";
    public override string Icon => "icon-sharing-iphone";
    
    public MovieExportAction(MovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }
    
    public override FileActionResult Execute(string collectionAlias, object[] entityIds)
    {
        try
        {
            var allMovies = _movieRepository.GetAll(null, null, true).GetAwaiter().GetResult();
            if (allMovies.Movies == null) return new FileActionResult(false);
            var movieCsvModels = MapToCsvModels(allMovies.Movies);

            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells.LoadFromCollection(movieCsvModels, true);
                package.Save();
            }

            stream.Position = 0;

            return new FileActionResult(true, stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "movies.xlsx");

        }
        catch
        {
            return new FileActionResult(false);
        }
    }

    private IEnumerable<CsvMovieModel> MapToCsvModels(IEnumerable<Movie> allMovies)
    {
        foreach (var movie in allMovies)
        {
            var genres = string.Empty;

            if (movie.Genres != null && movie.Genres.Count != 0)
            {
                genres = string.Join(", ", movie.Genres.Select(x => x.Name));
            }
            
            yield return new CsvMovieModel
            {
                Id = movie.Id,
                Name = movie.Name,
                ReleaseYear = movie.ReleaseYear,
                MainActor = movie.MainActor,
                Genres = genres
            };
        }
    }
}