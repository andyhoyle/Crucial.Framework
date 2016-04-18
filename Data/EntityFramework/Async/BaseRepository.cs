using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;
using Crucial.Framework.Logging;

namespace Crucial.Framework.Data.EntityFramework.Async
{
    /// <summary>
    /// Represents a repository of items.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of item in the repository.</typeparam>
    public class BaseRepository<TEntity> : Crucial.Framework.DesignPatterns.Repository.Async.IGenericRepository<TEntity>
            where TEntity : Crucial.Framework.BaseEntities.ProviderEntityBase
    {
        private ILogger _logger;
        private readonly IContextProvider<IDbContextAsync> _contextProvider;

        public BaseRepository(IContextProvider<IDbContextAsync> contextProvider, ILogger logger)
        {
            _contextProvider = contextProvider;
            _logger = logger;
        }

        public async Task<IEnumerable<TEntity>> GetAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> queryShaper, CancellationToken cancellationToken)
        {
            var query = queryShaper(_contextProvider.DbContext.Set<TEntity>());
            return await query.ToListAsync(cancellationToken);   
        }

        public async Task<IEnumerable<TEntity>> GetAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> queryShaper, CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[] include)
        {
            var query = queryShaper(_contextProvider.DbContext.Set<TEntity>());
            include.Select(_ => query.Include(_));
            return await query.ToListAsync(cancellationToken);   
        }

        public async Task<TResult> GetAsync<TResult>(Func<IQueryable<TEntity>, TResult> queryShaper, CancellationToken cancellationToken)
        {
            var factory = Task<TResult>.Factory;
            return await factory.StartNew(() => queryShaper(_contextProvider.DbContext.Set<TEntity>()), cancellationToken);
        }

        public async Task<TEntity> Create(TEntity entity)
        {
            var output = _contextProvider.DbContext.Set<TEntity>();
            TEntity result = output.Add(entity);

            try
            {
                await _contextProvider.DbContext.SaveChangesAsync();
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                _logger.LogException(ex);
                throw;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var er in ex.EntityValidationErrors)
                {
                    sb.Append(er.ToString());
                }

                _logger.Fatal(sb.ToString());
                _logger.LogException(ex);
                throw;
            }

            return result as TEntity;
        }

        public async virtual Task<bool> Delete(TEntity entity)
        {
            _contextProvider.DbContext.Set<TEntity>().Attach(entity);
            _contextProvider.DbContext.Entry(entity).State = System.Data.Entity.EntityState.Deleted;

            try
            {
                await _contextProvider.DbContext.SaveChangesAsync();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                _logger.LogException(ex);
                throw new Exception("EF Validation failed, see inner exception for details", ex);
            }

            return true;
        }
        
        public async Task<bool> Update(TEntity entity)
        {
            _contextProvider.DbContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;

            try
            {
                await _contextProvider.DbContext.SaveChangesAsync();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                _logger.LogException(ex);

                _contextProvider.DbContext.Entry(entity).State = System.Data.Entity.EntityState.Unchanged;
                throw;
            }

            return true;
        }
    }
}
