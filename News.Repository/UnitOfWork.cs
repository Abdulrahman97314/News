using News.Core;
using News.Core.Entities;
using News.Core.Repositories;
using News.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News.Repository
{
    public class UnitOfWork :IUnitOfWork
    {
        private readonly NewsDbContext Context;
        private readonly Dictionary<Type, object> Repositories = new Dictionary<Type, object>();

        public UnitOfWork(NewsDbContext storeDbContext)
        {
            this.Context = storeDbContext;
        }

        public async Task<int> CompleteAsync()
        {
            return await Context.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await Context.DisposeAsync();
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            var type = typeof(TEntity);

            if (!Repositories.ContainsKey(type))
            {
                var repository = new GenericRepository<TEntity>(Context);
                Repositories.Add(type, repository);
            }

            return (IGenericRepository<TEntity>)Repositories[type];
        }
    }
}
