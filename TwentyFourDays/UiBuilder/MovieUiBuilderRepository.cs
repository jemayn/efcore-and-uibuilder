using System.Linq.Expressions;
using TwentyFourDays.Persistence.Models;
using TwentyFourDays.Persistence.Repositories;
using Umbraco.Cms.Core.Models;
using Umbraco.UIBuilder;
using Umbraco.UIBuilder.Persistence;

namespace TwentyFourDays.UiBuilder;

public class MovieUiBuilderRepository : Repository<Movie, int>
{
    private readonly MovieRepository _movieRepository;

    public MovieUiBuilderRepository(RepositoryContext context, MovieRepository movieRepository) : base(context)
    {
        _movieRepository = movieRepository;
    }

    protected override int GetIdImpl(Movie entity)
    {
        return entity.Id;
    }

    protected override Movie GetImpl(int id)
    {
        return _movieRepository.GetById(id).GetAwaiter().GetResult();
    }

    protected override IEnumerable<TJunctionEntity> GetRelationsByParentIdImpl<TJunctionEntity>(int parentId, string relationAlias)
    {
        throw new NotImplementedException();
    }

    protected override Movie SaveImpl(Movie entity)
    {
        return _movieRepository.InsertOrUpdate(entity).GetAwaiter().GetResult();
    }

    protected override TJunctionEntity SaveRelationImpl<TJunctionEntity>(TJunctionEntity entity)
    {
        throw new NotImplementedException();
    }

    protected override void DeleteImpl(int id)
    {
        var movie = _movieRepository.GetById(id).GetAwaiter().GetResult();

        if (movie is null)
        {
            return;
        }
        
        _movieRepository.Delete(movie).GetAwaiter().GetResult();
    }

    protected override IEnumerable<Movie> GetAllImpl(
        Expression<Func<Movie, bool>> whereClause,
        Expression<Func<Movie, object>> orderBy,
        SortDirection orderByDirection)
    {
        return _movieRepository.GetAll(whereClause, orderBy, orderByDirection == SortDirection.Ascending)
            .GetAwaiter()
            .GetResult().Movies ?? new List<Movie>();
    }

    protected override PagedResult<Movie> GetPagedImpl(
        int pageNumber, 
        int pageSize,
        Expression<Func<Movie, bool>> whereClause, 
        Expression<Func<Movie, object>> orderBy,
        SortDirection orderByDirection)
    {
        var resultTuple = _movieRepository.GetAll(
                whereClause, 
                orderBy, 
                orderByDirection == SortDirection.Ascending, 
                (pageNumber - 1) * pageSize,
                pageSize)
            .GetAwaiter()
            .GetResult();

        return new PagedResult<Movie>(resultTuple.TotalResults, pageNumber, pageSize)
        {
            Items = resultTuple.Movies
        };
    }

    protected override long GetCountImpl(Expression<Func<Movie, bool>> whereClause)
    {
        return _movieRepository.GetAll(whereClause, null, true).GetAwaiter().GetResult().TotalResults;
    }
}