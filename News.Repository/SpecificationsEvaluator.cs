using Microsoft.EntityFrameworkCore;
using News.Core.Entities;
using News.Core.Specifications;


namespace News.Repository
{
    public class SpecificationsEvaluator<TEntity> where TEntity : BaseEntity
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecifications<TEntity> specifications)
        {
            var query = inputQuery;
            if (specifications.Criteria != null)
                query = query.Where(specifications.Criteria);

            if (specifications.OrderBy != null)
                query = query.OrderBy(specifications.OrderBy);

            if (specifications.OrderDescending != null)
                query = query.OrderByDescending(specifications.OrderDescending);

            if (specifications.IsPagingEnable)
                query = query.Skip(specifications.Skip).Take(specifications.Take);

            query = specifications.Includes.Aggregate(query, (current, include) => current.Include(include));
            return query;
        }
    }
}
