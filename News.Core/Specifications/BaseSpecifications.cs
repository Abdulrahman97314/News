using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace News.Core.Specifications
{
    public class BaseSpecifications<T> : ISpecifications<T>
    {
        public BaseSpecifications(Expression<Func<T, bool>> criteriaExpression)
        {
            Criteria = criteriaExpression;
        }
        public Expression<Func<T, bool>> Criteria { get; private set; }

        public List<Expression<Func<T, object>>> Includes { get; private set; } = new List<Expression<Func<T, object>>>();

        public Expression<Func<T, object>> OrderBy { get; private set; }

        public Expression<Func<T, object>> OrderDescending { get; private set; }
        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool IsPagingEnable { get; private set; }
        protected void AddInclude(Expression<Func<T, object>> includeExpression)
            => Includes.Add(includeExpression);
        protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
             => OrderBy = orderByExpression;
        protected void AddOrderDescending(Expression<Func<T, object>> orderDescendingExpression)
            => OrderDescending = orderDescendingExpression;
        protected void ApplayPaging(int take, int skip)
        {
            Take = take;
            Skip = skip;
            IsPagingEnable= true;
        }
    }
}
