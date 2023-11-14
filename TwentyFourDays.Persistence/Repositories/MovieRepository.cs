﻿using System.Linq.Expressions;
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
    
    public async Task<IEnumerable<Movie>?> GetAll(
        Expression<Func<Movie, bool>>? whereClause,
        Expression<Func<Movie, object>>? orderBy,
        bool ascending,
        int? skip = null,
        int? take = null)
    {
        using var scope = _scopeProvider.CreateScope();

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

            if (skip is not null && take is not null)
            {
                movies = movies.Skip(skip.Value).Take(take.Value);
            }

            return movies;
        });

        scope.Complete();

        return items.ToList();
    }
}