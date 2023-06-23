using System.Linq.Expressions;

namespace News.Core.Specifications
{
    public interface ISpecifications<T>
    {
        Expression<Func<T,bool>> Criteria { get; }
        List<Expression<Func<T, object>>> Includes { get; }
        Expression<Func<T, object>> OrderBy { get; }
        Expression<Func<T, object>> OrderDescending { get; }
        int Take { get; }
        int Skip { get; }
        bool IsPagingEnable { get; }
    } 
}
