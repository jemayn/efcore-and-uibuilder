using System.Collections;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TwentyFourDays.Persistence.DbContexts;
using TwentyFourDays.Persistence.Models;
using Umbraco.Cms.Persistence.EFCore.Scoping;

namespace TwentyFourDays.Persistence.Repositories;

public class MovieRepository
{
    private readonly IEFCoreScopeProvider<MovieContext> _scopeProvider;

    public MovieRepository(IEFCoreScopeProvider<MovieContext> scopeProvider)
    {
        _scopeProvider = scopeProvider;
    }

    public async Task Insert(Movie movie)
    {
        using var scope = _scopeProvider.CreateScope();
        
        await scope.ExecuteWithContextAsync<Task>(async db =>
        {
            var moviesFromDb = db.Movies.ToList();
            
            var movieExists = moviesFromDb.FirstOrDefault(x => x.Id == movie.Id);

            if (movieExists is not null)
            {
                return;
            }
            
            var movieGenresFromDb = db.MovieGenres.ToList();
            var movieGenreList = new List<MovieGenre>();

            foreach (var movieGenre in movie.Genres)
            {
                // If a genre of the same name already exists then we just use that as to not have duplicates
                var exists = movieGenresFromDb.FirstOrDefault(x => x.Name == movieGenre.Name);
                movieGenreList.Add(exists ?? movieGenre);
            }

            movie.Genres = movieGenreList;
            
            db.Movies.Add(movie);
            await db.SaveChangesAsync();
        });

        scope.Complete();
    }

    public async Task<Movie?> GetById(int id)
    {
        using var scope = _scopeProvider.CreateScope();
        
        var movie = await scope.ExecuteWithContextAsync(async db =>
        {
            return db.Movies.Include(x => x.Genres).FirstOrDefault(x => x.Id == id);
        });

        scope.Complete();

        return movie;
    }
    
    public async Task<Movie?> InsertOrUpdate(Movie movie)
    {
        using var scope = _scopeProvider.CreateScope();

        var result = await scope.ExecuteWithContextAsync<Movie>(async db =>
        {
            var movieGenresFromDb = db.MovieGenres.ToList();
            var moviesFromDb = db.Movies.Include(x => x.Genres).ToList();
            var movieFromDb = moviesFromDb.FirstOrDefault(x => x.Id == movie.Id);
            var movieGenres = new List<MovieGenre>();

            if (movieFromDb is not null)
            {
                // Set all property values to those of the incoming movie instead.
                // Can't just save movie as it counts as a "new object", need to instead update the corresponding obj from the db
                db.Entry(movieFromDb).CurrentValues.SetValues(movie);
                
                foreach (var genre in movie.Genres)
                {
                    // We need to add the genre objs from the database otherwise it will insert duplicates with different ids
                    var genreFromDb = movieGenresFromDb.FirstOrDefault(x => x.Id.ToString() == genre.Name);
                    movieGenres.Add(genreFromDb ?? genre);
                }

                movieFromDb.Genres = movieGenres;

                db.Movies.Update(movieFromDb);
                await db.SaveChangesAsync();
                return movieFromDb;
            }
            else
            {
                foreach (var genre in movie.Genres)
                {
                    // We need to add the genre objs from the database otherwise it will insert duplicates with different ids
                    var genreFromDb = movieGenresFromDb.FirstOrDefault(x => x.Id.ToString() == genre.Name);
                    movieGenres.Add(genreFromDb ?? genre);
                }

                movie.Genres = movieGenres;

                db.Movies.Update(movie);
                await db.SaveChangesAsync();
                return movie;
            }
        });

        scope.Complete();

        return result;
    }
    
    public async Task Delete(Movie movie)
    {
        using var scope = _scopeProvider.CreateScope();

        await scope.ExecuteWithContextAsync<Task>(async db =>
        {
            db.Movies.Remove(movie);
            await db.SaveChangesAsync();
        });

        scope.Complete();
    }
    
    public async Task<(int TotalResults, IEnumerable<Movie>? Movies)> GetAll(
        Expression<Func<Movie, bool>>? whereClause,
        Expression<Func<Movie, object>>? orderBy,
        bool ascending,
        int? skip = null,
        int? take = null)
    {
        using var scope = _scopeProvider.CreateScope();

        var totalResults = 0;

        var items = await scope.ExecuteWithContextAsync(async db =>
        {
            var movies = db.Movies.AsQueryable();

            if (whereClause is not null)
            {
                movies = movies.Where(whereClause);
            }
            
            if (orderBy is not null)
            {
                movies = ascending ? movies.OrderBy(orderBy) : movies.OrderByDescending(orderBy);
            }
            
            totalResults = movies.Count();

            if (skip is not null && take is not null)
            {
                movies = movies.Skip(skip.Value).Take(take.Value);
            }

            return movies;
        });

        scope.Complete();

        return (totalResults, items.ToList());
    }
    
    public async Task<IEnumerable<MovieGenre>> GetAllGenres()
    {
        using var scope = _scopeProvider.CreateScope();
        
        var items = await scope.ExecuteWithContextAsync(async db => db.MovieGenres);

        scope.Complete();

        return items.ToList();
    }
    
    
}