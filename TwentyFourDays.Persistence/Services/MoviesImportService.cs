using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using TwentyFourDays.Persistence.Models;
using TwentyFourDays.Persistence.Repositories;
using Umbraco.Cms.Core.Extensions;

namespace TwentyFourDays.Persistence.Services;

public class MoviesImportService
{
    private readonly ILogger<MoviesImportService> _logger;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly MovieRepository _movieRepository;

    public MoviesImportService(ILogger<MoviesImportService> logger, IHostEnvironment hostEnvironment, MovieRepository movieRepository)
    {
        _logger = logger;
        _hostEnvironment = hostEnvironment;
        _movieRepository = movieRepository;
    }

    public async Task Import()
    {
        _logger.LogInformation("Starting movies import");
        var excelPath = Path.Combine(_hostEnvironment.MapPathContentRoot(Umbraco.Cms.Core.Constants.SystemDirectories.Data), "chatgpt-25-christmas-movies.xlsx");
        
        if (!File.Exists(excelPath))
        {
            _logger.LogError($"Couldn't find movies file on path: {excelPath}");
            return;
        }

        // Adapted from https://www.c-sharpcorner.com/article/using-epplus-to-import-and-export-data-in-asp-net-core/
        await using var fileStream = File.Open(excelPath, FileMode.Open);

        using var package = new ExcelPackage(fileStream);

        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        if (worksheet is null)
        {
            _logger.LogError("Couldn't find worksheet");
            return;
        }

        var movies = MapMoviesFromExcel(worksheet);

        foreach (var movie in movies)
        {
            await _movieRepository.Insert(movie);
        }
        
        _logger.LogInformation("Import complete");
    }
    
    private static IEnumerable<Movie> MapMoviesFromExcel(ExcelWorksheet worksheet)
    {
        var dealers = new List<Movie>();
        var rowCount = worksheet.Dimension.Rows;

        // Starts on row 2 as row 1 are headers
        for (var row = 2; row <= rowCount; row++)
        {
            var name = worksheet.Cells[row, 1].Value?.ToString()?.Trim() ?? "";
            int.TryParse(worksheet.Cells[row, 2].Value?.ToString()?.Trim(), out var releaseYear);
            var genres = GetGenres(worksheet.Cells[row, 3].Value?.ToString()?.Trim());
            var mainActor = worksheet.Cells[row, 4].Value?.ToString()?.Trim() ?? "";
            
            dealers.Add(new Movie
            {
                Name = name,
                ReleaseYear = releaseYear,
                Genres = genres,
                MainActor = mainActor
            });
        }

        return dealers;
    }

    private static List<MovieGenre> GetGenres(string? value)
    {
        var movieGenres = new List<MovieGenre>();

        if (string.IsNullOrWhiteSpace(value))
        {
            return movieGenres;
        }

        var genres = value.Split(',');

        foreach (var genre in genres)
        {
            var trimmedGenre = genre.Trim();

            if (string.IsNullOrWhiteSpace(trimmedGenre))
            {
                continue;
            }
            
            movieGenres.Add(new MovieGenre
            {
                Name = trimmedGenre
            });
        }
        
        return movieGenres;
    }
}