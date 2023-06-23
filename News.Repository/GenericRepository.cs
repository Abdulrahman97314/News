using Microsoft.EntityFrameworkCore;
using News.Core.Entities;
using News.Core.Repositories;
using News.Core.Specifications;
using News.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News.Repository
{
    internal class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly NewsDbContext context;

        public GenericRepository(NewsDbContext context)
        {
            this.context = context;
        }
        public async Task AddAsync(T entity)
           => await context.Set<T>().AddAsync(entity);

        public async Task<int> CountWithSpecAsync(ISpecifications<T> specifications)
        => await SpecificationsEvaluator<T>.GetQuery(context.Set<T>(), specifications).CountAsync();

        public void Delete(T entity)
           => context.Set<T>().Remove(entity);
        public async Task<IReadOnlyList<T>> GetAllAsync()
           => await context.Set<T>().AsNoTracking().ToListAsync();
        public async Task<T> GetByIdAsync(int id)
           => await context.Set<T>().AsNoTracking().SingleOrDefaultAsync(e=> e.Id == id);

        public async Task<IReadOnlyList<T>> GetEntitiesWithSpecifications(ISpecifications<T> specifications)
           => await SpecificationsEvaluator<T>.GetQuery(context.Set<T>(), specifications).ToListAsync();

        public async Task<T> GetEntityWithSpecifications(ISpecifications<T> specifications)
            => await SpecificationsEvaluator<T>.GetQuery(context.Set<T>(), specifications).FirstOrDefaultAsync();

        public void Update(T entity)
           => context.Set<T>().Update(entity);
    }
}
