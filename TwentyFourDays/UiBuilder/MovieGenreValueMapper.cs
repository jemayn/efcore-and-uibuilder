using TwentyFourDays.Persistence.Models;
using Umbraco.UIBuilder.Mapping;

namespace TwentyFourDays.UiBuilder;

public class MovieGenreValueMapper : ValueMapper
{
    public override object ModelToEditor(object input)
    {
        if (input is List<MovieGenre> movieGenres)
        {
            if (movieGenres.Count == 1)
            {
                return movieGenres.Single().Id;
            }
            var res = movieGenres.Select(x => x.Id).ToArray();

            // Contentment checkbox list expects data as - ["val1", "val2"] - so have to convert it to this format
            return $"[\"{string.Join("\", \"", res)}\"]"; 
        }

        return input;
    }

    public override object EditorToModel(object input)
    {
        var movieGenres = new List<MovieGenre>();

        if (input is not string inputString)
        {
            return movieGenres;
        }

        var movieGenreStrings = inputString
            .Trim('[')
            .Trim(']')
            .Replace("\"", "")
            .Split(',');

        if (!movieGenreStrings.Any())
        {
            return movieGenres;
        }

        foreach (var movieGenreString in movieGenreStrings)
        {
            movieGenres.Add(new MovieGenre
            {
                Name = movieGenreString.Trim()
            });
        }

        return movieGenres;
    }
}