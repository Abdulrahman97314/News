using News.Core.Entities;
using News.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News.Core.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        public Task<IReadOnlyList<T>> GetAllAsync();
        public Task<T> GetByIdAsync(int id);
        public void Delete(T entity);
        public Task AddAsync(T entity);
        public void Update(T entity);
        Task<T> GetEntityWithSpecifications(ISpecifications<T> specifications);
        Task<IReadOnlyList<T>> GetEntitiesWithSpecifications(ISpecifications<T> specifications);
        Task<int> CountWithSpecAsync(ISpecifications<T> specifications);
    }
}
