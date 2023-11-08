using Microsoft.EntityFrameworkCore;
using TwentyFourDays.Persistence.Models;

namespace TwentyFourDays.Persistence.DbContexts;

public class MovieContext : DbContext
{
    public MovieContext(DbContextOptions<MovieContext> options) : base(options)
    {
    }
    
    public required DbSet<Movie> Movies { get; set; }
    public required DbSet<MovieGenre> MovieGenres { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Movie>()
            .HasMany(e => e.Genres)
            .WithMany(e => e.Movies)
            .UsingEntity(e => e.ToTable("movieToMovieGenre"))
            .HasKey(e => e.Id);
    }
}