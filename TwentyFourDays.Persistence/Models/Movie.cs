using System.ComponentModel.DataAnnotations.Schema;

namespace TwentyFourDays.Persistence.Models;

[Table("movie")]
public class Movie
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int ReleaseYear { get; set; }
    public string? MainActor { get; set; }
    public List<MovieGenre> Genres { get; set; }
}

[Table("movieGenre")]
public class MovieGenre
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<Movie> Movies { get; set; }
}